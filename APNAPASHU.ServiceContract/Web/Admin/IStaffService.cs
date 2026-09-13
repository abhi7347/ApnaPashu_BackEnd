using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Staff;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface IStaffService
    {
        Task<JsonModel<List<StaffResponseModel>>> GetAllAsync(FilterDto filterDto);
        Task<JsonModel<StaffResponseModel>> GetByIdAsync(int id);
        Task<JsonModel<object>> UpsertAsync(StaffUpsertModel model, int userId);
        Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model);
        Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId);
    }
}
