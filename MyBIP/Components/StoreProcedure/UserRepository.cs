using MyBIP.Components.StoreProcedure.Extra;
using MyBIP.Components.StoreProcedure.Required;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure
{
    public class UserRepository
    {
		public static List<UserListSearchResult> ListSearch(int brandId, int? userId, string firstname, string lastname, string email, int? type, int? userStuffId, int? status, string code, DateTime? minCreatedDate, DateTime? maxCreatedDate, int? pageNumber, byte? pageSize, out long totalItemCount)
		{
			var parameters = new List<DbQueryParameter>{
								new DbQueryParameter("BrandId",brandId),
								new DbQueryParameter("UserId",userId),
								new DbQueryParameter("Firstname",firstname),
								new DbQueryParameter("Lastname",lastname),
								new DbQueryParameter("Email",email),
								new DbQueryParameter("Type",type),
								new DbQueryParameter("UserStuffId",userStuffId),
								new DbQueryParameter("Status",status),
								new DbQueryParameter("Code",code),
								new DbQueryParameter("MinCreatedDate",minCreatedDate),
								new DbQueryParameter("MaxCreatedDate",maxCreatedDate),
								new DbQueryParameter("PageNumber",pageNumber),
								new DbQueryParameter("PageSize",pageSize),
								new DbQueryParameter("TotalItemCount",0, ParameterDirection.Output)
							};
			IDatabaseManager _databaseManager = DatabaseHelper.GetDatabaseManager("SqlServerMain");
			var response = _databaseManager.ExecuteList<UserListSearchResult>("dbo.pa_User_List_Search", parameters);
			totalItemCount = ConversionHelper.ChangeType<long>(_databaseManager.OutParameters[0].Value);
			return response;
		}
	}
}
