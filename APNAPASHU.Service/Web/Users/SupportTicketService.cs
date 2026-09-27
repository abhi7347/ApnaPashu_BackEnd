using APNAPASHU.Common.Messages;
using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Users.SupportTicket;
using APNAPASHU.RepositoryContract.Web.Users;
using APNAPASHU.ServiceContract.Web.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace APNAPASHU.Service.Web.Users
{
    public class SupportTicketService : BaseService, ISupportTicketService
    {
        private readonly ISupportTicketRepository _repository;

        public SupportTicketService(
            ISupportTicketRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<int>> CreateTicketAsync(int userId, CreateSupportTicketDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Subject))
                {
                    return new JsonModel<int>(0, "Subject is required.", (int)HttpStatusCode.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(model.Description))
                {
                    return new JsonModel<int>(0, "Description is required.", (int)HttpStatusCode.BadRequest);
                }

                List<string> fileNames = new();
                List<IFormFile> validFiles = new();

                if (model.Attachments != null && model.Attachments.Any())
                {
                    validFiles = model.Attachments.Where(f => f.Length > 0).ToList();
                    foreach (var file in validFiles)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        fileNames.Add($"img-{Guid.NewGuid():N}{ext}");
                    }
                }

                string? imagesJson = fileNames.Any() ? JsonSerializer.Serialize(fileNames) : null;

                var ticket = new SupportTicket
                {
                    UserId = userId,
                    Subject = model.Subject.Trim(),
                    Category = string.IsNullOrWhiteSpace(model.Category) ? "General" : model.Category.Trim(),
                    Priority = string.IsNullOrWhiteSpace(model.Priority) ? "Medium" : model.Priority.Trim(),
                    Status = "Open",
                    ImagesJson = imagesJson,
                    CreatedBy = userId
                };

                int ticketId = await _repository.CreateTicketAsync(ticket, model.Description.Trim());
                if (ticketId <= 0)
                {
                    return new JsonModel<int>(0, "Failed to create support ticket.", (int)HttpStatusCode.InternalServerError);
                }

                if (validFiles.Any())
                {
                    await _uploader.UploadFilesAsync(validFiles, fileNames, $"support-tickets/{ticketId}");
                }

                return new JsonModel<int>(ticketId, "Support ticket created successfully.", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<int>(0, ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<int>> AddMessageAsync(int senderUserId, string senderRole, AddTicketMessageDto model)
        {
            try
            {
                if (model.TicketId <= 0)
                {
                    return new JsonModel<int>(0, "Invalid Ticket ID.", (int)HttpStatusCode.BadRequest);
                }

                bool hasAttachments = model.Attachments != null && model.Attachments.Any(f => f.Length > 0);
                if (string.IsNullOrWhiteSpace(model.Message) && !hasAttachments)
                {
                    return new JsonModel<int>(0, "Message or attachment is required.", (int)HttpStatusCode.BadRequest);
                }

                var ticket = await _repository.GetTicketByIdAsync(model.TicketId);
                if (ticket == null)
                {
                    return new JsonModel<int>(0, "Support ticket not found.", (int)HttpStatusCode.NotFound);
                }

                List<string> fileNames = new();
                List<IFormFile> validFiles = new();

                if (model.Attachments != null && model.Attachments.Any())
                {
                    validFiles = model.Attachments.Where(f => f.Length > 0).ToList();
                    foreach (var file in validFiles)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        fileNames.Add($"msg-{Guid.NewGuid():N}{ext}");
                    }
                }

                if (validFiles.Any())
                {
                    await _uploader.UploadFilesAsync(validFiles, fileNames, $"support-tickets/{model.TicketId}");
                }

                string? imagesJson = fileNames.Any() ? JsonSerializer.Serialize(fileNames) : null;

                var msg = new SupportTicketMessage
                {
                    TicketId = model.TicketId,
                    SenderUserId = senderUserId,
                    SenderName = senderRole == "Admin" ? "Support Admin" : (ticket.UserName ?? "User"),
                    SenderRole = senderRole,
                    Message = model.Message?.Trim() ?? string.Empty,
                    ImagesJson = imagesJson,
                    CreatedBy = senderUserId
                };

                int msgId = await _repository.AddMessageAsync(msg);
                if (msgId <= 0)
                {
                    return new JsonModel<int>(0, "Failed to add message.", (int)HttpStatusCode.InternalServerError);
                }

                return new JsonModel<int>(msgId, "Reply added successfully.", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<int>(0, ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<bool>> UpdateTicketStatusAsync(int updatedByUserId, UpdateTicketStatusDto model)
        {
            try
            {
                if (model.TicketId <= 0 || string.IsNullOrWhiteSpace(model.Status))
                {
                    return new JsonModel<bool>(false, "Invalid ticket status payload.", (int)HttpStatusCode.BadRequest);
                }

                bool result = await _repository.UpdateStatusAsync(model.TicketId, model.Status.Trim(), updatedByUserId);
                if (!result)
                {
                    return new JsonModel<bool>(false, "Failed to update ticket status.", (int)HttpStatusCode.BadRequest);
                }

                return new JsonModel<bool>(true, "Ticket status updated successfully.", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<bool>(false, ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<SupportTicketResponseDto>> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                var ticket = await _repository.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    return new JsonModel<SupportTicketResponseDto>(null, "Ticket not found.", (int)HttpStatusCode.NotFound);
                }

                await ProcessTicketImages(ticket);

                return new JsonModel<SupportTicketResponseDto>(ticket, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<SupportTicketResponseDto>(null, ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<List<SupportTicketResponseDto>>> GetUserTicketsAsync(int userId, SupportTicketFilterDto filter)
        {
            try
            {
                var list = await _repository.GetUserTicketsAsync(userId, filter);
                foreach (var ticket in list)
                {
                    await ProcessTicketImages(ticket);
                }
                return new JsonModel<List<SupportTicketResponseDto>>(list, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<List<SupportTicketResponseDto>>(new List<SupportTicketResponseDto>(), ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task ProcessTicketImages(SupportTicketResponseDto ticket)
        {
            if (ticket == null) return;

            if (!string.IsNullOrEmpty(ticket.ImagesJson))
            {
                try
                {
                    var names = ParseImageNames(ticket.ImagesJson);
                    if (names.Any())
                    {
                        ticket.ImageUrls = await _uploader.GetFileUrlsAsync(names, $"support-tickets/{ticket.Id}");
                    }
                }
                catch { }
            }

            if (ticket.Messages != null && ticket.Messages.Any())
            {
                foreach (var msg in ticket.Messages)
                {
                    if (!string.IsNullOrEmpty(msg.ImagesJson))
                    {
                        try
                        {
                            var names = ParseImageNames(msg.ImagesJson);
                            if (names.Any())
                            {
                                msg.ImageUrls = await _uploader.GetFileUrlsAsync(names, $"support-tickets/{ticket.Id}");
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private List<string> ParseImageNames(string imagesJson)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Attempt standard string list
                try
                {
                    var list = JsonSerializer.Deserialize<List<string>>(imagesJson, options);
                    if (list != null && list.Any())
                    {
                        return list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    }
                }
                catch { }

                // Attempt helper object list
                var jsonImages = JsonSerializer.Deserialize<List<ImageJsonHelper>>(imagesJson, options);
                if (jsonImages != null && jsonImages.Any())
                {
                    return jsonImages
                        .Select(x => x.Value ?? x.ImageName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x!)
                        .ToList();
                }
            }
            catch { }

            return new List<string>();
        }

        private class ImageJsonHelper
        {
            public string? ImageName { get; set; }
            public string? Value { get; set; }
        }
    }
}
