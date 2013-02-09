
namespace Pug.Cartage
{
	public class CartLineAttributeInfo : Pug.Cartage.ICartLineAttributeInfo
	{
		string name, value;

		public CartLineAttributeInfo(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}
	}
}
