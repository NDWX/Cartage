using System;
using System.Collections.Generic;

using Pug.Application.Data;
using Pug.Application.Security;

namespace Pug.Cartage
{
	public class Cartage<Sp> : ICartage
        where Sp : ICartInfoStore
	{
        IApplicationData<Sp> storeProviderFactory;
		ISecurityManager securityManager;

		public Cartage(IApplicationData<Sp> storeProviderFactory, ISecurityManager securityManager)
		{
			this.storeProviderFactory = storeProviderFactory;
			this.securityManager = securityManager;
		}

		void CheckUserAuthorization(string action, ICollection<string> objects, IDictionary<string, string> context)
		{
			bool userIsAuthorized = securityManager.CurrentUser.UserIsAuthorized(action, objects, context);

			if (!userIsAuthorized)
			{
				throw new NotAuthorized();
			}
		}

		string GetNewIdentifier()
		{
			byte[] binarySeed = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());

			string lineIdentifier = Pug.Base32.From(binarySeed).Replace("=", "");

			return lineIdentifier;
		}

		/// <summary>
		/// Check whether cart exists
		/// </summary>
		/// <param name="identifier">Cart identifier</param>
		/// <returns>Wehther cart exists</returns>
		public bool CartExists(string identifier)
		{
			CheckUserAuthorization("CartExists", new string[] {identifier}, null);

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			bool exists = false;

			try
			{
				exists = storeProvider.CartExists(identifier);
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return exists;
		}

		/// <summary>
		/// Register new cart.
		/// </summary>
		/// <returns>Object of new cart</returns>
		public ICart RegisterCart()
		{
			CheckUserAuthorization("RegisterCart", null, null);

			string identifier;
			ICart cart = null;

			while (cart == null)
			{
				identifier = GetNewIdentifier();

				cart = RegisterCart(identifier);
			}

			return cart;
		}

		/// <summary>
		/// Register new cart with specified identifier.
		/// </summary>
		/// <param name="identifier">Identifier of new cart</param>
		/// <returns>Object of new cart</returns>
		public ICart RegisterCart(string identifier)
		{
			CheckUserAuthorization("RegisterCart", new string[] { identifier }, null);

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				storeProvider.RegisterCart(identifier, securityManager.CurrentUser.Identity.Identifier);
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			ICart cart = new Cart<Sp>(identifier, storeProviderFactory, securityManager);

			return cart;
		}

		/// <summary>
		/// Get all carts based on current CustomerSession info and user authorization.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		public ICollection<ICartInfo> GetCarts(Range<DateTime> creationPeriod, Range<DateTime> modificationPeriod)
		{
			Dictionary<string, string> actionAuthorizationContext = new Dictionary<string,string>();
			actionAuthorizationContext.Add("parameters.creationPeriod.start", creationPeriod.Start.ToString("u"));
			actionAuthorizationContext.Add("parameters.creationPeriod.end", creationPeriod.End.ToString("u"));
			actionAuthorizationContext.Add("parameters.modificationPeriod.start", modificationPeriod.Start.ToString("u"));
			actionAuthorizationContext.Add("parameters.modificationPeriod.end", modificationPeriod.End.ToString("u"));

			CheckUserAuthorization("GetCartList", null, actionAuthorizationContext);

			ICollection<ICartInfo> carts;

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				carts = (ICollection<ICartInfo>)storeProvider.GetCarts(creationPeriod, modificationPeriod);
			}
			catch
			{
				throw;
			}
			finally
			{
				storeProvider.Dispose();
			}

			return carts;
		}

		/// <summary>
		/// Obtain specific ICart object based on CustomerSession info and user authorization.
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public ICart GetCart(string identifier = "")
		{
			CheckUserAuthorization("GetCart", new string[] { identifier }, null);

			ICart cart;

			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				if (!storeProvider.CartExists(identifier))
				{
					throw new CartNotFound(identifier);
				}

				cart = new Cart<Sp>(identifier, storeProviderFactory, securityManager);
			}
			finally
			{
				storeProvider.Dispose();
			}

			return cart;
		}

		/// <summary>
		/// Delete cart of specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		public void DeleteCart(string identifier)
		{
			CheckUserAuthorization("DeleteCart", new string[] { identifier }, null);

			ICollection<ICartLineInfo> cartLines;
			ICartInfoStore storeProvider = storeProviderFactory.GetSession();

			try
			{
				if (storeProvider.CartExists(identifier))
				{
					cartLines = storeProvider.GetLines(identifier);

					storeProvider.BeginTransaction();

					foreach (CartLineInfo line in cartLines)
						storeProvider.DeleteLine(identifier, line.Identifier, securityManager.CurrentUser.Identity.Identifier);

					storeProvider.DeleteCart(identifier, securityManager.CurrentUser.Identity.Identifier);

					storeProvider.CommitTransaction();
				}
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
		
		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
