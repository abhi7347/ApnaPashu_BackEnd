using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Animal;

namespace APNAPASHU.ServiceContract.Web
{
    /// <summary>
    /// Web Animal Service Interface - CRUD Template
    /// </summary>
    public interface IAnimalService
    {
        /// <summary>
        /// Get all animals with filtering and pagination
        /// </summary>
        Task<JsonModel<AnimalListResponseDto>> GetAllAsync(AnimalFilterDto filterDto);

        /// <summary>
        /// Create or Update animal (Upsert)
        /// </summary>
        Task<JsonModel<AnimalResponseDto>> UpsertAsync(AnimalUpsertDto upsertDto);

        /// <summary>
        /// Update animal status (IsActive)
        /// </summary>
        Task<JsonModel<bool>> UpdateStatusAsync(AnimalStatusUpdateDto statusDto);

        /// <summary>
        /// Delete animal (soft delete)
        /// </summary>
        Task<JsonModel<bool>> DeleteAsync(int animalId);
    }
}