using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Extra
{
	public class UserListSearchRequest
	{
		public int BrandId { get; set; }

		public string Code { get; set; }

		public string Email { get; set; }

		public string Firstname { get; set; }

		public string Lastname { get; set; }

		public DateTime? MaxCreatedDate { get; set; }

		public DateTime? MinCreatedDate { get; set; }

		public int? PageNumber { get; set; }

		public byte? PageSize { get; set; }

		public int? Status { get; set; }

		public int? Type { get; set; }

		public int? UserId { get; set; }

		public int? UserStuffId { get; set; }

	}
}
