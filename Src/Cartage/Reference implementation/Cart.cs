using System;
using System.Collections.Generic;
using System.Linq;

using Pug.Application.Data;
using Pug.Application.Security;

namespace Pug.Cartage
{
	public class CartFinalized : Exception
	{
	}

	class Cart<Sp> : ICart
        where Sp : ICartInfoStore
	{
        IApplicationData<Sp> storeProviderFactory;
		ISecurityManager securityManager;

		ICartInfo cartInfo;

        public Cart(string identifier, IApplicationData<Sp> storeProviderFactory, ISecurityManager securityManager)
		{
			this.storeProviderFactory = storeProviderFactory;
			this.securityManager = securityManager;

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			this.cartInfo = storeProvider.GetCart(identifier);

			if (cartInfo == null)
			{
				storeProvider.RegisterCart(identifier, securityManager.CurrentUser.Identity.Identifier);
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

		private void CheckCartNotFinalized()
		{

			if (Info.IsFinalized)
				throw new CartFinalized();
		}

		void SetModificationAttributes()
		{
			cartInfo = new CartInfo(cartInfo.Identifier, cartInfo.Created, cartInfo.CreateUser, DateTime.Now, securityManager.CurrentUser.Identity.Identifier, cartInfo.IsFinalized);
			//cartInfo.LastModified = DateTime.Now;
			//cartInfo.LastModifyUser = securityManager.CurrentUser.Identity.Identifier;
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

			CheckCartNotFinalized();

			string lineIdentifier = GetNewIdentifier();

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();

				storeProvider.InsertLine(cartInfo.Identifier, lineIdentifier, productCode, quantity, securityManager.CurrentUser.Identity.Identifier);

				foreach (string attribute in attributes.Keys)
					storeProvider.InsertLineAttribute(cartInfo.Identifier, lineIdentifier, attribute, attributes[attribute], securityManager.CurrentUser.Identity.Identifier);

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

			CheckCartNotFinalized();

			if (quantity < 0)
				quantity = 0;

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();

				storeProvider.UpdateLine(cartInfo.Identifier, line, quantity, securityManager.CurrentUser.Identity.Identifier);

				IDictionary<string, ICartLineAttributeInfo> knownAttributes = storeProvider.GetLineAttributes(cartInfo.Identifier, line);

				foreach(string name in knownAttributes.Keys)
					if( !attributes.ContainsKey(name) )
						storeProvider.DeleteLineAttribute(cartInfo.Identifier, line, name, securityManager.CurrentUser.Identity.Identifier);

				foreach (string attribute in attributes.Values)
					storeProvider.SetLineAttribute(cartInfo.Identifier, line, attribute, attributes[attribute], securityManager.CurrentUser.Identity.Identifier);

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
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			operationContext.Add("ATTRIBUTE_NAME", name);
			operationContext.Add("ATTRIBUTE_NEW)VALUE", value);
			CheckUserAuthorization("SetLineAttribute", null, operationContext);

			CheckCartNotFinalized();

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.BeginTransaction();

				storeProvider.SetLineAttribute(cartInfo.Identifier, line, name, value, securityManager.CurrentUser.Identity.Identifier);

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

			CheckCartNotFinalized();

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.DeleteLine(cartInfo.Identifier, identifier, securityManager.CurrentUser.Identity.Identifier);

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

			ICartLineInfo lineInfo = null;
			IDictionary<string, ICartLineAttributeInfo> lineAttributes = null;
			ICartLine cartLine = null;
			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

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

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

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

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

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

		public void MarkAsFinalized()
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("FinalizeCart", null, operationContext);

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.SetCartFinalized(cartInfo.Identifier, securityManager.CurrentUser.Identity.Identifier);
				this.cartInfo = storeProvider.GetCart(this.cartInfo.Identifier);
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

		public void Clear()
		{
			IDictionary<string, string> operationContext = new Dictionary<string, string>();

			CheckUserAuthorization("ClearCart", null, operationContext);

			CheckCartNotFinalized();

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.DeleteLines(cartInfo.Identifier, securityManager.CurrentUser.Identity.Identifier);

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
