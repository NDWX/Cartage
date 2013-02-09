using System;

namespace Pug.Cartage
{
	public class CartInfo : Pug.Cartage.ICartInfo
	{
		string identifier;
		DateTime created, lastModified;
		string createUser, lastModifyUser;

		public CartInfo(string identifier, DateTime createTimestamp, string createUser, DateTime lastModified, string lastModifyUser)
		{
			this.identifier = identifier;
			this.created = createTimestamp;
			this.createUser = createUser;
			this.lastModified = lastModified;
			this.lastModifyUser = lastModifyUser;
		}
		public string Identifier
		{
			get
			{
				return identifier;
			}
		}
		public DateTime Created
		{
			get
			{
				return created;
			}
		}
		public string CreateUser
		{
			get
			{
				return createUser;
			}
		}
		public DateTime LastModified
		{
			get
			{
				return lastModified;
			}
			protected internal set
			{
				this.lastModified = value;
			}
		}
		public string LastModifyUser
		{
			get
			{
				return lastModifyUser;
			}
			protected internal set
			{
				this.lastModifyUser = value;
			}
		}
	}
}
