using System;
using System.Collections.Generic;

using Pug.Application.Data;

namespace Pug.Cartage
{
	/// <summary>
	/// Implementation of this interface should update last modification user and timestamp.
	/// </summary>
	public interface ICartInfoStore :  IApplicationDataSession, IDisposable
	{
		/// <summary>
		/// Check whether cart with specified identifier exists in the store.
		/// </summary>
		/// <param name="identifier">Identifier of the cart.</param>
		/// <returns>A boolean value indicating existence of cart with specified identifier.</returns>
		bool CartExists(string identifier);

		/// <summary>
		/// Get list of CartInfo objects based on specified creation and modification period
		/// </summary>
		/// <param name="registrationPeriod">Time period during which the cart was created, otherwise null.</param>
		/// <param name="modificationPeriod">Time period during which the cart was last modified, otherwise null.</param>
		/// <returns>A collection of CartInfo objects</returns>
		ICollection<ICartInfo> GetCarts(Range<DateTime> registrationPeriod, Range<DateTime> modificationPeriod);

		/// <summary>
		/// Register new cart with specified identifier.
		/// </summary>
		/// <param name="identifier">Identifier of the new cart</param>
		void RegisterCart(string identifier, string user);

		/// <summary>
		/// Delete cart record.
		/// </summary>
		/// <param name="identifier">Identifier of cart record</param>
		void DeleteCart(string identifier, string user);

		/// <summary>
		/// Get cart record of specified identifier.
		/// </summary>
		/// <param name="identifier">Identifier of the cart record</param>
		/// <returns>CartInfo object of specified identifier</returns>
		ICartInfo GetCart(string identifier);

		/// <summary>
		/// Insert new line item record.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="identifier">Line identifier</param>
		/// <param name="productCode">Product identifer</param>
		/// <param name="quantity">Quantity of product</param>
		void InsertLine(string cart, string identifier, string productCode, decimal quantity, string user);

		/// <summary>
		/// Insert item line attribute record.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="line">Item line identifier</param>
		/// <param name="name">Name of the attribute</param>
		/// <param name="value">Value of the attribute</param>
		void InsertLineAttribute(string cart, string line, string name, string value, string user);

		/// <summary>
		/// Whether item line record exists.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="identifier">Item line identifier</param>
		/// <returns>Boolean value indicating existence of the item line</returns>
		bool LineExists(string cart, string identifier);

		/// <summary>
		/// Get item line record of specified identifier of specified cart.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="identifier">Item line identifier</param>
		/// <returns>Info of specified cart line</returns>
		ICartLineInfo GetLine(string cart, string identifier);

		/// <summary>
		/// Get all attribute records of an item line.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="identifier">Line identifier</param>
		/// <returns>Dictionary of attribute name and its info object</returns>
		IDictionary<string, ICartLineAttributeInfo> GetLineAttributes(string cart, string identifier);

		/// <summary>
		/// Get all item line records of a cart.
		/// </summary>
		/// <param name="cart">cart identifier</param>
		/// <returns>A collection of item line info</returns>
		ICollection<ICartLineInfo> GetLines(string cart);
					   
		/// <summary>
		/// Update quantity and attributes of an item line record.
		/// </summary>
		/// <param name="cart">Identifier of a cart</param>
		/// <param name="identifier">Identifier of the line to update</param>
		/// <param name="quantity">New quantity of specified item line</param>
		void UpdateLine(string cart, string identifier, decimal quantity, string user);

		/// <summary>
		/// Update existing item line attribute record.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="line">Line identifier</param>
		/// <param name="name">Name of attribute to update</param>
		/// <param name="value">New value of the specified attribute</param>
		void SetLineAttribute(string cart, string line, string name, string value, string user);
		
		/// <summary>
		/// Delete item line attribute record.
		/// </summary>
		/// <param name="cart">Cart identifier</param>
		/// <param name="line">Line identifier</param>
		/// <param name="name">Name of attribute to delete</param>
		void DeleteLineAttribute(string cart, string line, string name, string user);
					   
		/// <summary>
		/// Delete record ofspecified line of specified cart.
		/// </summary>
		/// <param name="cart">Identifier of a cart</param>
		/// <param name="identifier">Identifier of the line to delete</param>
		void DeleteLine(string cart, string identifier, string user);

		/// <summary>
		/// Delete all item line records of a cart.
		/// </summary>
		/// <param name="cart">Identifier of a cart</param>
		void DeleteLines(string cart, string user);

		/// <summary>
		/// Set the finalized flag of a cart to true.
		/// </summary>
		/// <param name="cart"></param>
		void SetCartFinalized(string cart, string user);
	}
}
