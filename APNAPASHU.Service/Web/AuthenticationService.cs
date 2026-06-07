using APNAPASHU.Common;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.ServiceContract.Web;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APNAPASHU.Service.Web
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IWebHostEnvironment _environment;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IHttpContextAccessor accessor,
            IConfiguration configuration,
            IWebHostEnvironment environment) : base(accessor, configuration)
        {
            _authenticationRepository = authenticationRepository;
            _environment = environment;
        }

        public async Task<JsonModel<SqlResponseModel>> RegisterAsync(RegisterRequestModel model)
        {
            var encryptionKey = Configuration.GetSection("JwtSettings")["EncryptionKey"] ?? string.Empty;
            var decryptedPassword = string.Empty;
            try
            {
                decryptedPassword = EncryptionDecryption.Decrypt(model.Password, encryptionKey);
            }
            catch
            {
                // Decryption failed, might be a raw password from Swagger/testing
            }

            if (string.IsNullOrEmpty(decryptedPassword))
            {
                decryptedPassword = model.Password; // fallback to raw
            }
            model.Password = decryptedPassword;

            model.Password = EncryptionDecryption.CreateHash(model.Password);

            var result = await _authenticationRepository.RegisterAsync(model);

            if (result == null)
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = null,
                    Message = "Something went wrong",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            // ? If SP failed
            if (result.StatusCode != "SUCCESS")
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // ? Success ? Send Welcome Email
            try
            {
                string htmlTemplatePath = Path.Combine(_environment.WebRootPath, "EmailTemplate/RegistrationTemplate.html");
                if (File.Exists(htmlTemplatePath))
                {
                    string emailString = await System.IO.File.ReadAllTextAsync(htmlTemplatePath);

                    emailString = emailString.Replace("#Name", model.FirstName + " " + model.LastName);
                    emailString = emailString.Replace("#Email", model.Email);
                    emailString = emailString.Replace("#CopyrightYear", DateTime.Now.Year.ToString());

                    EmailModel emailModel = new EmailModel()
                    {
                        Body = emailString,
                        To = model.Email,
                        Subject = "Welcome to ApnaPashu - Registration Successful"
                    };

                    bool emailSent = await SendEmailAsync(emailModel);

                    if (emailSent)
                    {
                        return new JsonModel<SqlResponseModel>
                        {
                            Data = result,
                            Message = result.Message,
                            StatusCode = (int)HttpStatusCode.OK
                        };
                    }
                }

                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = $"{result.Message} (email error: {ex.Message})",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
        }

        public async Task<JsonModel<LoginResponseModel>> LoginUserAsync(string email, string encryptedPassword)
        {
            var encryptionKey = Configuration.GetSection("JwtSettings")["EncryptionKey"] ?? string.Empty;
            var decryptedPassword = string.Empty;
            try
            {
                decryptedPassword = EncryptionDecryption.Decrypt(encryptedPassword, encryptionKey);
            }
            catch
            {
                // Decryption failed, might be a raw password from Swagger/testing
            }

            // Fallback to raw password if decryption resulted in empty/null
            if (string.IsNullOrEmpty(decryptedPassword))
            {
                decryptedPassword = encryptedPassword;
            }

            var user = await _authenticationRepository.LoginUserAsync(email, decryptedPassword);

            if (user == null)
            {
                return new JsonModel<LoginResponseModel>
                {
                    Data = null,
                    Message = "Invalid email or password",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }

            // The SP returns the user's current hash in user.PasswordHash
            bool isValid = EncryptionDecryption.ValidatePassword(decryptedPassword, user.PasswordHash);

            if (isValid && user.StatusCode == "SUCCESS")
            {

                // Generate JWT Token
                user.Token = GenerateJwtToken(user);

                // Generate signed URL for Profile Image if exists
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var imageUrls = await _uploader.GetFileUrlsAsync(new List<string> { user.ProfileImage }, $"users/{user.UserId}");
                    user.Image = imageUrls.FirstOrDefault();
                }

                // Clear password hash from response for security
                user.PasswordHash = string.Empty;

                return new JsonModel<LoginResponseModel>
                {
                    Data = user,
                    Message = user.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new JsonModel<LoginResponseModel>
            {
                Data = null,
                Message = "Invalid credentials",
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }

        private string GenerateJwtToken(LoginResponseModel user)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<JsonModel<SqlResponseModel>> ForgotPasswordAsync(string email)
        {
            // 1. Generate 6-digit OTP Token
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString();

            // 2. Call Repo to save token
            var result = await _authenticationRepository.ForgotPasswordAsync(email, token);

            if (result != null && result.StatusCode == "SUCCESS")
            {
                // 3. Send Email
                string htmlTemplatePath = Path.Combine(_environment.WebRootPath, "EmailTemplate/ForgotPasswordTemplate.html");
                if (File.Exists(htmlTemplatePath))
                {
                    string emailString = await System.IO.File.ReadAllTextAsync(htmlTemplatePath);
                    
                    // Dynamically build the reset link based on the request origin (e.g. localhost vs production domain)
                    var request = HttpContextAccessor.HttpContext?.Request;
                    var origin = request?.Headers["Origin"].FirstOrDefault() ?? $"{request?.Scheme}://{request?.Host}";
                    string resetLink = $"{origin}/reset-password?email={System.Net.WebUtility.UrlEncode(email)}";

                    emailString = emailString.Replace("#ResetLink", resetLink);
                    emailString = emailString.Replace("#Email", email);
                    emailString = emailString.Replace("#Token", token);
                    emailString = emailString.Replace("#CopyrightYear", DateTime.Now.Year.ToString());

                    EmailModel emailModel = new EmailModel()
                    {
                        Body = emailString,
                        To = email,
                        Subject = "ApnaPashu - Password Reset Request"
                    };

                    bool emailSent = await SendEmailAsync(emailModel);
                    if (!emailSent)
                    {
                        return new JsonModel<SqlResponseModel> { Data = null, Message = "Token generated but failed to send email.", StatusCode = (int)HttpStatusCode.InternalServerError };
                    }
                }

                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new JsonModel<SqlResponseModel>
            {
                Data = result,
                Message = result?.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        public async Task<JsonModel<SqlResponseModel>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            // 1. Hash the new password using existing methodology
            string newPasswordHash = EncryptionDecryption.CreateHash(newPassword);

            // 2. Pass hash to Repository
            var result = await _authenticationRepository.ResetPasswordAsync(email, token, newPasswordHash);

            if (result != null && result.StatusCode == "SUCCESS")
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new JsonModel<SqlResponseModel>
            {
                Data = null,
                Message = result?.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        
        public async Task<JsonModel<LoginResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel model, int userId)
        {
            try
            {
                string? passwordHash = null;
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    passwordHash = EncryptionDecryption.CreateHash(model.NewPassword);
                }

                string? imagePath = null;
                if (model.Image != null)
                {
                    var extension = Path.GetExtension(model.Image.FileName);
                    imagePath = $"{Guid.NewGuid()}{extension}";
                }

                // 1. Update Profile in DB (Returns SqlResponseModel)
                var result = await _authenticationRepository.UpdateProfileAsync(model, userId, passwordHash, imagePath);

                if (result == null || result.StatusCode != "SUCCESS")
                {
                    return new JsonModel<LoginResponseModel>(null, result?.Message ?? "Failed to update profile", (int)HttpStatusCode.BadRequest);
                }

                // 2. Fetch updated user details to hydrate the response
                var updatedUser = await _authenticationRepository.GetUserByIdAsync(userId);
                if (updatedUser == null)
                {
                    return new JsonModel<LoginResponseModel>(null, "Profile updated but failed to load fresh data", (int)HttpStatusCode.PartialContent);
                }

                // 3. Physical Upload to R2 only if DB update was successful and image was provided
                if (model.Image != null && imagePath != null)
                {
                    await _uploader.UploadFilesAsync(
                        new List<IFormFile> { model.Image },
                        new List<string> { imagePath },
                        $"users/{userId}"
                    );
                }

                // 4. Generate signed URL for Image if exists
                if (!string.IsNullOrEmpty(updatedUser.ProfileImage))
                {
                    var imageUrls = await _uploader.GetFileUrlsAsync(new List<string> { updatedUser.ProfileImage }, $"users/{userId}");
                    updatedUser.Image = imageUrls.FirstOrDefault();
                }
                
                updatedUser.PasswordHash = string.Empty;
                return new JsonModel<LoginResponseModel>(updatedUser, result.Message ?? "Profile updated successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<LoginResponseModel>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<JsonModel<LoginResponseModel>> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _authenticationRepository.GetUserByIdAsync(userId);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        var imageUrls = await _uploader.GetFileUrlsAsync(new List<string> { user.ProfileImage }, $"users/{userId}");
                        user.Image = imageUrls.FirstOrDefault();
                    }
                    user.PasswordHash = string.Empty;
                    return new JsonModel<LoginResponseModel>(user, "Profile fetched successfully", (int)HttpStatusCode.OK);
                }
                return new JsonModel<LoginResponseModel>(null, "User not found", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return new JsonModel<LoginResponseModel>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<SqlResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel model, int userId)
        {
            try
            {
                // 1. Fetch user to get current hash
                var user = await _authenticationRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new JsonModel<SqlResponseModel>(null, "User not found", (int)HttpStatusCode.NotFound);
                }

                // 2. Verify current password
                bool isCorrect = EncryptionDecryption.ValidatePassword(model.CurrentPassword, user.PasswordHash);
                if (!isCorrect)
                {
                    return new JsonModel<SqlResponseModel>(null, "Incorrect current password", (int)HttpStatusCode.BadRequest);
                }

                // 3. Hash new password
                string newHash = EncryptionDecryption.CreateHash(model.NewPassword);

                // 4. Update in DB
                var result = await _authenticationRepository.ChangePasswordAsync(userId, newHash);
                
                return new JsonModel<SqlResponseModel>(result, result?.Message ?? "Password updated successfully", 
                    (result?.StatusCode == "SUCCESS" ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest));
            }
            catch (Exception ex)
            {
                return new JsonModel<SqlResponseModel>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}

