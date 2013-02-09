using System;
using System.Collections.Generic;


namespace Pug.Cartage
{
	public interface ICart<I, L, Li, La, S> : IDisposable
		where I : ICartInfo
		where L : ICartLine<Li, La>
		where Li : ICartLineInfo
		where La : ICartLineAttributeInfo
		where S : ICartSummary
	{
		/// <summary>
		/// Info of cart
		/// </summary>
		I Info
		{
			get;
		}

		/// <summary>
		/// Add to cart product of specified quantity and attributes. Implementation may simply add new line for each call to this function or update existing line of identical product and/or attributes.
		/// </summary>
		/// <param name="product">Identifier of product to add</param>
		/// <param name="quantity">Quantity of product to add</param>
		/// <param name="attributes">Attributes of product to add</param>
		/// <returns></returns>
		string AddItems(string product, decimal quantity, IDictionary<string, string> attributes);

		/// <summary>
		/// Update quantity and attributes of specified line.
		/// </summary>
		/// <param name="line">Identifier of line to update</param>
		/// <param name="quantity">New quantity</param>
		/// <param name="attributes">New or updated line attributes</param>
		void UpdateLine(string line, decimal quantity, IDictionary<string, string> attributes);

		/// <summary>
		/// Remove specified line from cart.
		/// </summary>
		/// <param name="identifier"></param>
		void RemoveLine(string identifier);

		/// <summary>
		/// Get line of specified identifier.
		/// </summary>
		/// <param name="identifier">Line identifier</param>
		/// <returns>Object of specified line identifier</returns>
		L GetLine(string identifier);

		/// <summary>
		/// Get all lines info.
		/// </summary>
		/// <returns>Collection of lines info</returns>
		ICollection<Li> GetLines();

		/// <summary>
		/// Get summary of cart
		/// </summary>
		/// <returns>Summary object of cart</returns>
		S GetCartSummary();

		/// <summary>
		/// Delete all lines in cart.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A specialization of the ICart generic interface.
	/// </summary>
	public interface ICart : ICart<ICartInfo, ICartLine, ICartLineInfo, ICartLineAttributeInfo, ICartSummary>
	{

	}
}