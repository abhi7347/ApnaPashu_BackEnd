using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Animal;
using APNAPASHU.RepositoryContract.Mobile;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace APNAPASHU.Service.Mobile
{
    /// <summary>
    /// Mobile Animal Service Implementation - CRUD Template
    /// </summary>
    public class AnimalService : BaseService, ServiceContract.Mobile.IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;

        public AnimalService(IAnimalRepository animalRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _animalRepository = animalRepository;
        }

        /// <summary>
        /// Get all animals with filtering and pagination
        /// </summary>
        public async Task<JsonModel<AnimalListResponseDto>> GetAllAsync(AnimalFilterDto filterDto)
        {
            try
            {
                if (filterDto == null)
                    return new JsonModel<AnimalListResponseDto>(null, "Filter is required", 400, "INVALID_FILTER");

                if (filterDto.PageNumber <= 0 || filterDto.PageSize <= 0)
                    return new JsonModel<AnimalListResponseDto>(null, "Invalid pagination parameters", 400, "INVALID_PARAMS");

                var animals = await _animalRepository.GetAllAsync(filterDto);
                var totalCount = await _animalRepository.GetTotalCountAsync(filterDto);

                var response = new AnimalListResponseDto
                {
                    Data = animals,
                    TotalRecords = totalCount,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                };

                return new JsonModel<AnimalListResponseDto>(response, "Animals retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new JsonModel<AnimalListResponseDto>(null, "Error retrieving animals", 500, ex.Message);
            }
        }

        /// <summary>
        /// Create or Update animal (Upsert)
        /// </summary>
        public async Task<JsonModel<AnimalResponseDto>> UpsertAsync(AnimalUpsertDto upsertDto)
        {
            try
            {
                if (upsertDto == null)
                    return new JsonModel<AnimalResponseDto>(null, "Invalid animal data", 400, "INVALID_DATA");

                if (string.IsNullOrWhiteSpace(upsertDto.AnimalName))
                    return new JsonModel<AnimalResponseDto>(null, "Animal name is required", 400, "REQUIRED_FIELD");

                int animalId;

                if (upsertDto.AnimalId.HasValue && upsertDto.AnimalId > 0)
                {
                    // Update existing
                    var existingAnimal = await _animalRepository.GetByIdAsync(upsertDto.AnimalId.Value);
                    if (existingAnimal == null)
                        return new JsonModel<AnimalResponseDto>(null, "Animal not found", 404, "NOT_FOUND");

                    bool updated = await _animalRepository.UpdateAsync(upsertDto);
                    if (!updated)
                        return new JsonModel<AnimalResponseDto>(null, "Failed to update animal", 500, "UPDATE_FAILED");

                    animalId = upsertDto.AnimalId.Value;
                }
                else
                {
                    // Create new
                    animalId = await _animalRepository.CreateAsync(upsertDto);
                    if (animalId <= 0)
                        return new JsonModel<AnimalResponseDto>(null, "Failed to create animal", 500, "CREATE_FAILED");
                }

                var result = await _animalRepository.GetByIdAsync(animalId);
                return new JsonModel<AnimalResponseDto>(result, "Animal saved successfully", upsertDto.AnimalId.HasValue ? 200 : 201);
            }
            catch (Exception ex)
            {
                return new JsonModel<AnimalResponseDto>(null, "Error saving animal", 500, ex.Message);
            }
        }

        /// <summary>
        /// Update animal status (IsActive)
        /// </summary>
        public async Task<JsonModel<bool>> UpdateStatusAsync(AnimalStatusUpdateDto statusDto)
        {
            try
            {
                if (statusDto == null)
                    return new JsonModel<bool>(false, "Invalid status data", 400, "INVALID_DATA");

                if (statusDto.AnimalId <= 0)
                    return new JsonModel<bool>(false, "Invalid Animal ID", 400, "INVALID_ID");

                var existingAnimal = await _animalRepository.GetByIdAsync(statusDto.AnimalId);
                if (existingAnimal == null)
                    return new JsonModel<bool>(false, "Animal not found", 404, "NOT_FOUND");

                bool result = await _animalRepository.UpdateStatusAsync(statusDto);
                return result
                    ? new JsonModel<bool>(true, "Animal status updated successfully", 200)
                    : new JsonModel<bool>(false, "Failed to update status", 500, "UPDATE_FAILED");
            }
            catch (Exception ex)
            {
                return new JsonModel<bool>(false, "Error updating status", 500, ex.Message);
            }
        }

        /// <summary>
        /// Delete animal (soft delete - sets IsActive to false)
        /// </summary>
        public async Task<JsonModel<bool>> DeleteAsync(int animalId)
        {
            try
            {
                if (animalId <= 0)
                    return new JsonModel<bool>(false, "Invalid Animal ID", 400, "INVALID_ID");

                var existingAnimal = await _animalRepository.GetByIdAsync(animalId);
                if (existingAnimal == null)
                    return new JsonModel<bool>(false, "Animal not found", 404, "NOT_FOUND");

                bool result = await _animalRepository.DeleteAsync(animalId);
                return result
                    ? new JsonModel<bool>(true, "Animal deleted successfully", 200)
                    : new JsonModel<bool>(false, "Failed to delete animal", 500, "DELETE_FAILED");
            }
            catch (Exception ex)
            {
                return new JsonModel<bool>(false, "Error deleting animal", 500, ex.Message);
            }
        }
    }
}