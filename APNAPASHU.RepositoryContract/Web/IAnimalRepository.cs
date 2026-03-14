using APNAPASHU.DataContract.Models.Animal;

namespace APNAPASHU.RepositoryContract.Web
{
    /// <summary>
    /// Web Animal Repository Interface - CRUD Template
    /// </summary>
    public interface IAnimalRepository
    {
        /// <summary>
        /// Get all animals with filtering and pagination
        /// </summary>
        Task<List<AnimalResponseDto>> GetAllAsync(AnimalFilterDto filterDto);

        /// <summary>
        /// Get animal by ID
        /// </summary>
        Task<AnimalResponseDto> GetByIdAsync(int animalId);

        /// <summary>
        /// Create animal
        /// </summary>
        Task<int> CreateAsync(AnimalUpsertDto upsertDto);

        /// <summary>
        /// Update animal (full properties)
        /// </summary>
        Task<bool> UpdateAsync(AnimalUpsertDto upsertDto);

        /// <summary>
        /// Update animal status (IsActive)
        /// </summary>
        Task<bool> UpdateStatusAsync(AnimalStatusUpdateDto statusDto);

        /// <summary>
        /// Delete animal (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int animalId);

        /// <summary>
        /// Get total count with filter
        /// </summary>
        Task<int> GetTotalCountAsync(AnimalFilterDto filterDto);
    }
}