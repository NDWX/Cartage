using System;


namespace Pug.Cartage
{
	/// <summary>
	/// Generic interface for a cart system
	/// </summary>
	/// <typeparam name="C">Type of cart implementation</typeparam>
	/// <typeparam name="I">Type of info implementation returned by cart (of type specified above) object</typeparam>
	/// <typeparam name="L">Type of item line implementation returned by cart object</typeparam>
	/// <typeparam name="S">Type of summary implementation returned by cart object</typeparam>
	public interface ICartage<C, I, L, Li, La, S> : IDisposable
		where C : ICart<I, L, Li, La, S>
		where I : ICartInfo
		where L : ICartLine<Li, La>
		where Li : ICartLineInfo
		where La : ICartLineAttributeInfo
		where S : ICartSummary
	{
		/// <summary>
		/// Check that cart of specified identifier exists
		/// </summary>
		/// <param name="identifier">Identifier of cart</param>
		/// <returns>Boolean value indicating whether cart exists</returns>
		bool CartExists(string identifier);

		/// <summary>
		/// Register a new cart
		/// </summary>
		/// <returns>Cart implementation object of type C</returns>
		C RegisterCart();

		/// <summary>
		/// Register a new cart with specified identifier
		/// </summary>
		/// <param name="identifier">New cart identifier</param>
		/// <returns>Cart implementation object of type C with specified identifier</returns>
		C RegisterCart(string identifier);

		/// <summary>
		/// Get cart of specified identifier
		/// </summary>
		/// <param name="identifier">Identifier of the cart</param>
		/// <returns>Cart implementation object of type C with specified identifier</returns>
		C GetCart(string identifier);

		/// <summary>
		/// Get collection of cart info implementation objects based on creation and modification timestamp
		/// </summary>
		/// <param name="creationPeriod">The period during which the cart was created. Pass null to not specify a period.</param>
		/// <param name="modificationPeriod">The period during which the cart was modified. Pass null to not specify a period.</param>
		/// <returns></returns>
		System.Collections.Generic.ICollection<I> GetCarts(Range<DateTime> creationPeriod, Range<DateTime> modificationPeriod);

		/// <summary>
		/// Delete cart with specified identifier.
		/// </summary>
		/// <param name="identifier">Identifier of cart to delete</param>
		void DeleteCart(string identifier);
	}

	/// <summary>
	/// A Specialization Cartage generic interface.
	/// </summary>
	public interface ICartage : ICartage<ICart, ICartInfo, ICartLine, ICartLineInfo, ICartLineAttributeInfo, ICartSummary>
	{
	}
}
