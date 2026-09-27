using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Public.UserIndex;

namespace APNAPASHU.RepositoryContract.Web.Public
{
    public interface IUserIndexRepository
    {
        Task<UserIndexResponseModel> GetIndexAnimalsAsync();
    }
}
