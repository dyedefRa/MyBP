using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Extra
{
	public class UserListSearchResult
	{
		public int BrandId { get; set; }

		public string Code { get; set; }

		public DateTime CreatedDate { get; set; }
	
		public string Email { get; set; }

		public string Firstname { get; set; }

		public int Id { get; set; }

		public string Lastname { get; set; }

		public int Status { get; set; }

		public string StuffName { get; set; }

		public int Type { get; set; }

		public int? UserStuffId { get; set; }

	}
}
