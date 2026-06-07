using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Public.UserIndex;

namespace APNAPASHU.ServiceContract.Web.Public
{
    public interface IUserIndexService
    {
        Task<UserIndexResponseModel> GetIndexAnimalsAsync();
    }
}
