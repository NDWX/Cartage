using System.Collections.Generic;
using Pug.Extensions;

namespace Pug.Cartage
{
	public class CartLine : ICartLine
	{
		ICartLineInfo info;
		IDictionary<string, ICartLineAttributeInfo> attributes;

		public CartLine(ICartLineInfo info, IDictionary<string, ICartLineAttributeInfo> attributes)
		{
			this.info = info;
			this.attributes = attributes;
		}

		public ICartLineInfo Info
		{
			get
			{
				return info;
			}
		}

		public IDictionary<string, ICartLineAttributeInfo> Attributes
		{
			get
			{
				return attributes.ReadOnly();
			}
		}
	}
}