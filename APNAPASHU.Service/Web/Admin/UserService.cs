using APNAPASHU.Common;
using APNAPASHU.Common.Messages;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.User;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace APNAPASHU.Service.Web.Admin
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(
            IUserRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<List<UserResponseModel>>> GetAllAsync(FilterDto filterDto)
        {
            var data = await _repository.GetAllAsync(filterDto);
            var encryptionKey = Configuration.GetSection("JwtSettings")["EncryptionKey"] ?? string.Empty;
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.PasswordHash))
                    {
                        var decrypted = EncryptionDecryption.Decrypt(item.PasswordHash, encryptionKey);
                        item.Password = !string.IsNullOrEmpty(decrypted) ? decrypted : string.Empty;
                    }
                }
            }
            return new JsonModel<List<UserResponseModel>>(data, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
        }

        public async Task<JsonModel<UserResponseModel>> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
            {
                return new JsonModel<UserResponseModel>(null, ResponseMessages.NotFound, (int)HttpStatusCode.NotFound);
            }
            var encryptionKey = Configuration.GetSection("JwtSettings")["EncryptionKey"] ?? string.Empty;
            if (!string.IsNullOrEmpty(data.PasswordHash))
            {
                var decrypted = EncryptionDecryption.Decrypt(data.PasswordHash, encryptionKey);
                data.Password = !string.IsNullOrEmpty(decrypted) ? decrypted : string.Empty;
            }
            return new JsonModel<UserResponseModel>(data, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
        }

        public async Task<JsonModel<object>> UpsertAsync(UserUpsertModel model, int userId)
        {
            string? passwordHash = null;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var encryptionKey = Configuration.GetSection("JwtSettings")["EncryptionKey"] ?? string.Empty;
                var decryptedPassword = string.Empty;
                try
                {
                    decryptedPassword = EncryptionDecryption.Decrypt(model.Password, encryptionKey);
                }
                catch
                {
                    // Decryption fallback if plain text passed
                }

                if (string.IsNullOrEmpty(decryptedPassword))
                {
                    decryptedPassword = model.Password;
                }

                passwordHash = EncryptionDecryption.Encrypt(decryptedPassword, encryptionKey);
            }

            var result = await _repository.UpsertAsync(model, userId, passwordHash);

            if (result != null && result.StatusCode == "SUCCESS")
            {
                return new JsonModel<object>(null, result.Message ?? ResponseMessages.insertedSuccessfully, (int)HttpStatusCode.OK);
            }

            return new JsonModel<object>(null, result?.Message ?? ResponseMessages.Error, (int)HttpStatusCode.BadRequest);
        }

        public async Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model)
        {
            var result = await _repository.UpdateStatusAsync(model);
            if (result != null && result.StatusCode == "SUCCESS")
            {
                return new JsonModel<object>(null, result.Message ?? ResponseMessages.statusUpdated, (int)HttpStatusCode.OK);
            }

            return new JsonModel<object>(null, result?.Message ?? ResponseMessages.Error, (int)HttpStatusCode.BadRequest);
        }

        public async Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId)
        {
            if (ids == null || !ids.Any())
            {
                return new JsonModel<object>(null, ResponseMessages.Error, (int)HttpStatusCode.BadRequest);
            }

            string idsStr = string.Join(",", ids);
            var result = await _repository.DeleteAsync(idsStr, userId);

            if (result != null && result.StatusCode == "SUCCESS")
            {
                return new JsonModel<object>(null, result.Message ?? ResponseMessages.deletedSuccessfully, (int)HttpStatusCode.OK);
            }

            return new JsonModel<object>(null, result?.Message ?? ResponseMessages.Error, (int)HttpStatusCode.BadRequest);
        }
    }
}
