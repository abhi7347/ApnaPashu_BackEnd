using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Roles;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;

namespace APNAPASHU.Service.Web.Admin
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<JsonModel<List<RoleResponseModel>>> GetAllAsync(FilterDto filter)
        {
            var data = await _roleRepository.GetAllAsync(filter);
            return new JsonModel<List<RoleResponseModel>>(data, "Fetched successfully", 200);
        }

        public async Task<JsonModel<RoleResponseModel>> GetByIdAsync(int id)
        {
            var data = await _roleRepository.GetByIdAsync(id);
            if (data == null)
            {
                return new JsonModel<RoleResponseModel>(null, "Not Found", 404);
            }
            return new JsonModel<RoleResponseModel>(data, "Fetched successfully", 200);
        }

        public async Task<JsonModel<object>> UpsertAsync(RoleUpsertModel model, int userId)
        {
            var result = await _roleRepository.UpsertAsync(model, userId);
            bool isSuccess = string.Equals(result?.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) 
                             || result?.StatusCode == "200" 
                             || (string.IsNullOrEmpty(result?.StatusCode) && result?.Message?.Contains("success", StringComparison.OrdinalIgnoreCase) == true);
            int statusCode = isSuccess ? 200 : 400;
            return new JsonModel<object>(result, result?.Message, statusCode);
        }

        public async Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model)
        {
            var result = await _roleRepository.UpdateStatusAsync(model);
            int statusCode = string.Equals(result?.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) ? 200 : 400;
            return new JsonModel<object>(result, result?.Message, statusCode);
        }

        public async Task<JsonModel<object>> DeleteAsync(string ids, int userId)
        {
            var result = await _roleRepository.DeleteAsync(ids, userId);
            bool isSuccess = string.Equals(result?.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) 
                             || result?.StatusCode == "200" 
                             || (string.IsNullOrEmpty(result?.StatusCode) && result?.Message?.Contains("success", StringComparison.OrdinalIgnoreCase) == true);
            int statusCode = isSuccess ? 200 : 400;
            return new JsonModel<object>(result, result?.Message, statusCode);
        }
    }
}
