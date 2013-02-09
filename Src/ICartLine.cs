namespace Pug.Cartage
{
	public interface ICartLine<I, A> 
		where I : ICartLineInfo
		where A : ICartLineAttributeInfo
	{
		System.Collections.Generic.IDictionary<string, A> Attributes { get; }
		I Info { get; }
	}

	public interface ICartLine : ICartLine<ICartLineInfo, ICartLineAttributeInfo>
	{
	}
}
