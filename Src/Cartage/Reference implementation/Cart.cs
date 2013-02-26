using System;
using System.Collections.Generic;
using System.Linq;

using Pug.Application.Data;
using Pug.Application.Security;

namespace Pug.Cartage
{
	class Cart<Sp> : ICart
        where Sp : ICartInfoStoreProvider
	{
        IApplicationData<Sp> storeProviderFactory;
		ISecurityManager securityManager;

		CartInfo cartInfo;

        public Cart(string identifier, IApplicationData<Sp> storeProviderFactory, ISecurityManager securityManager)
		{
			this.storeProviderFactory = storeProviderFactory;
			this.securityManager = securityManager;

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			this.cartInfo = storeProvider.GetCart(identifier);

			if (cartInfo == null)
			{
				storeProvider.RegisterCart(identifier);
				cartInfo = storeProvider.GetCart(identifier);
			}
		}

		string GetNewIdentifier()
		{
			byte[] binarySeed = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());

			string lineIdentifier = Pug.Base32.From(binarySeed).Replace("=", "");

			return lineIdentifier;
		}

		void CheckUserAuthorization(string action, ICollection<string> objects, IDictionary<string, string> context)
		{
			bool userIsAuthorized = securityManager.CurrentUser.UserIsAuthorized(action, objects, context);

			if (!userIsAuthorized)
			{
				throw new NotAuthorized();
			}
		}

		void SetModificationAttributes()
		{
			cartInfo.LastModified = DateTime.Now;
			cartInfo.LastModifyUser = securityManager.CurrentUser.Identity.Identifier;
		}

		#region ICart Members

		public ICartInfo Info
		{
			get { return cartInfo; }
		}

		public string AddItems(string productCode, decimal quantity, IDictionary<string, string> attributes)
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("AddItemsToCart", null, operationContext);

			string lineIdentifier = GetNewIdentifier();

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();

				storeProvider.InsertLine(cartInfo.Identifier, lineIdentifier, productCode, quantity);

				foreach (string attribute in attributes.Keys)
					storeProvider.InsertLineAttribute(cartInfo.Identifier, lineIdentifier, attribute, attributes[attribute]);

				SetModificationAttributes();

				storeProvider.CommitTransaction();
			}
			catch
			{
				storeProvider.RollbackTransaction();

				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return lineIdentifier;
		}

		public void UpdateLine(string line, decimal quantity, IDictionary<string, string> attributes)
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("UpdateCartLine", null, operationContext);

			if (quantity < 0)
				quantity = 0;

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();

				storeProvider.UpdateLine(cartInfo.Identifier, line, quantity);

				IDictionary<string, CartLineAttributeInfo> knownAttributes = storeProvider.GetLineAttributes(cartInfo.Identifier, line);

				foreach(string name in knownAttributes.Keys)
					if( !attributes.ContainsKey(name) )
						storeProvider.DeleteLineAttribute(cartInfo.Identifier, line, name);

				foreach (string attribute in attributes.Values)
					storeProvider.SetLineAttribute(cartInfo.Identifier, line, attribute, attributes[attribute]);

				SetModificationAttributes();

				storeProvider.CommitTransaction();
			}
			catch
			{
				storeProvider.RollbackTransaction();

				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}
		}

		public void SetLineAttribute(string line, string name, string value)
		{
			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();
				
				storeProvider.SetLineAttribute(cartInfo.Identifier, line, name, value);

				SetModificationAttributes();

				storeProvider.CommitTransaction();
			}
			catch
			{
				storeProvider.RollbackTransaction();

				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}
		}

		public void RemoveLine(string identifier)
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("RemoveCartLine", null, operationContext);

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.DeleteLine(cartInfo.Identifier, identifier);

				SetModificationAttributes();
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}
		}

		public ICartLine GetLine(string identifier)
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("RemoveCartLine", null, operationContext);

			CartLineInfo lineInfo = null;
			IDictionary<string, ICartLineAttributeInfo> lineAttributes = null;
			CartLine cartLine = null;
			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				if (storeProvider.LineExists(this.Info.Identifier, identifier))
				{
					lineInfo = storeProvider.GetLine(cartInfo.Identifier, identifier);
					lineAttributes = (IDictionary<string, ICartLineAttributeInfo>)storeProvider.GetLineAttributes(this.Info.Identifier, identifier);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return cartLine = new CartLine(lineInfo, lineAttributes);
		}

		public ICollection<ICartLineInfo> GetLines()
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("GetCartLines", null, operationContext);

			ICollection<ICartLineInfo> cartLines;

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				cartLines = (ICollection<ICartLineInfo>)storeProvider.GetLines(cartInfo.Identifier);
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return (ICollection<ICartLineInfo>)cartLines;
		}

		public ICartSummary GetCartSummary()
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("GetCartSummary", null, operationContext);

			ICollection<ICartLineInfo> cartLines;
			CartSummary summary;

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				cartLines = (ICollection<ICartLineInfo>)storeProvider.GetLines(cartInfo.Identifier);

				summary = (from cartLine in cartLines group cartLine by "" into linesSummary select new CartSummary(linesSummary.Distinct(new CartLineProductComparer()).Count(), linesSummary.Sum(l => l.Quantity))).First();
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return summary;
		}

		public void Clear()
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("ClearCart", null, operationContext);

			ICartInfoStoreProvider storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.DeleteLines(cartInfo.Identifier);

				SetModificationAttributes();
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		internal class CartLineProductComparer : IEqualityComparer<ICartLineInfo>
		{
			#region IEqualityComparer<CartLine> Members

			public bool Equals(ICartLineInfo x, ICartLineInfo y)
			{
				return x.ProductCode.Equals(y.ProductCode);
			}

			public int GetHashCode(ICartLineInfo obj)
			{
				return obj.ProductCode.GetHashCode();
			}

			#endregion
		}
	}
}
