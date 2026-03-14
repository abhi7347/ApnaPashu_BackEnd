using Dapper;
using APNAPASHU.DataContract.Models.Animal;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Mobile
{
    /// <summary>
    /// Mobile Animal Repository Implementation - CRUD Template
    /// </summary>
    public class AnimalRepository : BaseRepository, RepositoryContract.Mobile.IAnimalRepository
    {
        public AnimalRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Get all animals with filtering and pagination
        /// </summary>
        public async Task<List<AnimalResponseDto>> GetAllAsync(AnimalFilterDto filterDto)
        {
            try
            {
                int offset = (filterDto.PageNumber - 1) * filterDto.PageSize;
                string query = @"
                    SELECT AnimalId, AnimalName, Breed, Category, Age, Description, 
                           Price, Location, ContactNumber, CreatedDate, UpdatedDate, IsActive
                    FROM Animals
                    WHERE 1=1";

                // Add IsActive filter
                if (filterDto.IsActive.HasValue)
                    query += " AND IsActive = @IsActive";

                // Add category filter
                if (!string.IsNullOrWhiteSpace(filterDto.Category))
                    query += " AND Category = @Category";

                // Add location filter
                if (!string.IsNullOrWhiteSpace(filterDto.Location))
                    query += @" AND Location LIKE '%' + @Location + '%'";

                // Add search filter
                if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                    query += @" AND (AnimalName LIKE '%' + @SearchTerm + '%' 
                             OR Description LIKE '%' + @SearchTerm + '%' 
                             OR Breed LIKE '%' + @SearchTerm + '%')";

                query += @" ORDER BY CreatedDate DESC
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    if (filterDto.IsActive.HasValue)
                        parameters.Add("@IsActive", filterDto.IsActive.Value ? 1 : 0);
                    if (!string.IsNullOrWhiteSpace(filterDto.Category))
                        parameters.Add("@Category", filterDto.Category);
                    if (!string.IsNullOrWhiteSpace(filterDto.Location))
                        parameters.Add("@Location", filterDto.Location);
                    if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                        parameters.Add("@SearchTerm", filterDto.SearchTerm);
                    parameters.Add("@Offset", offset);
                    parameters.Add("@PageSize", filterDto.PageSize);

                    var result = await conn.QueryAsync<AnimalResponseDto>(query, parameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving animals: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get animal by ID
        /// </summary>
        public async Task<AnimalResponseDto> GetByIdAsync(int animalId)
        {
            try
            {
                string query = @"
                    SELECT AnimalId, AnimalName, Breed, Category, Age, Description, 
                           Price, Location, ContactNumber, CreatedDate, UpdatedDate, IsActive
                    FROM Animals
                    WHERE AnimalId = @AnimalId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AnimalId", animalId);

                    return await conn.QueryFirstOrDefaultAsync<AnimalResponseDto>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving animal: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create animal
        /// </summary>
        public async Task<int> CreateAsync(AnimalUpsertDto upsertDto)
        {
            try
            {
                string query = @"
                    INSERT INTO Animals (AnimalName, Breed, Category, Age, Description, 
                                        Price, Location, ContactNumber, CreatedDate, UpdatedDate, IsActive)
                    VALUES (@AnimalName, @Breed, @Category, @Age, @Description, 
                            @Price, @Location, @ContactNumber, @CreatedDate, @UpdatedDate, 1);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AnimalName", upsertDto.AnimalName);
                    parameters.Add("@Breed", upsertDto.Breed ?? string.Empty);
                    parameters.Add("@Category", upsertDto.Category ?? string.Empty);
                    parameters.Add("@Age", upsertDto.Age);
                    parameters.Add("@Description", upsertDto.Description ?? string.Empty);
                    parameters.Add("@Price", upsertDto.Price);
                    parameters.Add("@Location", upsertDto.Location ?? string.Empty);
                    parameters.Add("@ContactNumber", upsertDto.ContactNumber ?? string.Empty);
                    parameters.Add("@CreatedDate", DateTime.UtcNow);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    return await conn.ExecuteScalarAsync<int>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating animal: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update animal (full properties)
        /// </summary>
        public async Task<bool> UpdateAsync(AnimalUpsertDto upsertDto)
        {
            try
            {
                string query = @"
                    UPDATE Animals
                    SET AnimalName = @AnimalName, Breed = @Breed, Category = @Category, Age = @Age,
                        Description = @Description, Price = @Price, Location = @Location,
                        ContactNumber = @ContactNumber, UpdatedDate = @UpdatedDate
                    WHERE AnimalId = @AnimalId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AnimalId", upsertDto.AnimalId);
                    parameters.Add("@AnimalName", upsertDto.AnimalName);
                    parameters.Add("@Breed", upsertDto.Breed ?? string.Empty);
                    parameters.Add("@Category", upsertDto.Category ?? string.Empty);
                    parameters.Add("@Age", upsertDto.Age);
                    parameters.Add("@Description", upsertDto.Description ?? string.Empty);
                    parameters.Add("@Price", upsertDto.Price);
                    parameters.Add("@Location", upsertDto.Location ?? string.Empty);
                    parameters.Add("@ContactNumber", upsertDto.ContactNumber ?? string.Empty);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating animal: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update animal status (IsActive)
        /// </summary>
        public async Task<bool> UpdateStatusAsync(AnimalStatusUpdateDto statusDto)
        {
            try
            {
                string query = @"
                    UPDATE Animals
                    SET IsActive = @IsActive, UpdatedDate = @UpdatedDate
                    WHERE AnimalId = @AnimalId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AnimalId", statusDto.AnimalId);
                    parameters.Add("@IsActive", statusDto.IsActive ? 1 : 0);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating status: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete animal (soft delete - sets IsActive to 0)
        /// </summary>
        public async Task<bool> DeleteAsync(int animalId)
        {
            try
            {
                string query = @"
                    UPDATE Animals
                    SET IsActive = 0, UpdatedDate = @UpdatedDate
                    WHERE AnimalId = @AnimalId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AnimalId", animalId);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting animal: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get total count with filter
        /// </summary>
        public async Task<int> GetTotalCountAsync(AnimalFilterDto filterDto)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Animals WHERE 1=1";

                if (filterDto.IsActive.HasValue)
                    query += " AND IsActive = @IsActive";

                if (!string.IsNullOrWhiteSpace(filterDto.Category))
                    query += " AND Category = @Category";

                if (!string.IsNullOrWhiteSpace(filterDto.Location))
                    query += @" AND Location LIKE '%' + @Location + '%'";

                if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                    query += @" AND (AnimalName LIKE '%' + @SearchTerm + '%' 
                             OR Description LIKE '%' + @SearchTerm + '%' 
                             OR Breed LIKE '%' + @SearchTerm + '%')";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    if (filterDto.IsActive.HasValue)
                        parameters.Add("@IsActive", filterDto.IsActive.Value ? 1 : 0);
                    if (!string.IsNullOrWhiteSpace(filterDto.Category))
                        parameters.Add("@Category", filterDto.Category);
                    if (!string.IsNullOrWhiteSpace(filterDto.Location))
                        parameters.Add("@Location", filterDto.Location);
                    if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                        parameters.Add("@SearchTerm", filterDto.SearchTerm);

                    return await conn.ExecuteScalarAsync<int>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting animals: {ex.Message}", ex);
            }
        }

        private IDbConnection GetConnection()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
    }
}