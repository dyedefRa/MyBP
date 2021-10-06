using MyBIP.Components.StoreProcedure.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure
{
    public class UserBusinessLogic
    {
        public static ServiceResult<List<UserListSearchResult>> ListSearch(UserListSearchRequest userListSearchRequest, out long totalItemCount)
        {
            var response = ServiceResult<List<UserListSearchResult>>.Instance.ErrorResult(1102);
            response.ResultValue = UserRepository.ListSearch(userListSearchRequest.BrandId, userListSearchRequest.UserId, userListSearchRequest.Firstname, userListSearchRequest.Lastname, userListSearchRequest.Email, userListSearchRequest.Type, userListSearchRequest.UserStuffId, userListSearchRequest.Status, userListSearchRequest.Code, userListSearchRequest.MinCreatedDate, userListSearchRequest.MaxCreatedDate, userListSearchRequest.PageNumber, userListSearchRequest.PageSize, out totalItemCount);
            if (response.ResultValue != null)
            {
                var successCode = 0;
                response.SuccessResult(response.ResultValue, successCode);

            }
            return response;
        }
    }
}
