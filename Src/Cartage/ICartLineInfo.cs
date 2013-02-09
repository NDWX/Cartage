
namespace Pug.Cartage
{
	public interface ICartLineInfo
	{
		string Identifier
		{
			get;
		}

		string ProductCode
		{
			get;
		}

		decimal Quantity
		{
			get;
		}
	}
}
