using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;


namespace APNAPASHU.Service
{
    public class R2Uploader
    {
        private readonly IMinioClient _minio;
        private readonly string _bucket;

        public R2Uploader(string accountId, string accessKey, string secretKey, string bucketName)
        {
            _bucket = bucketName;

            _minio = new MinioClient()
                .WithEndpoint($"{accountId}.r2.cloudflarestorage.com")
                .WithCredentials(accessKey, secretKey)
                .WithSSL()
                .Build();
        }

        // ==========================================
        // UPLOAD MULTIPLE FILES
        // ==========================================
        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folderName)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("Files are required");

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Folder name is required");

            var tasks = files.Select(async file =>
            {
                var ext = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var objectKey = $"{folderName}/{fileName}";

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(objectKey)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(GetContentType(ext));

                await _minio.PutObjectAsync(putObjectArgs);

                return fileName;
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        // ==========================================
        // GET MULTIPLE FILE URLS (SECURE)
        // ==========================================
        public async Task<List<string>> GetFileUrlsAsync(List<string> fileNames, string folderName, int expiryMinutes = 5)
        {
            if (fileNames == null || !fileNames.Any())
                return new List<string>();

            var tasks = fileNames.Select(async fileName =>
            {
                var objectKey = $"{folderName}/{fileName}";

                return await _minio.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs()
                        .WithBucket(_bucket)
                        .WithObject(objectKey)
                        .WithExpiry(60 * expiryMinutes)
                );
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        // ==========================================
        // DELETE MULTIPLE FILES
        // ==========================================
        public async Task DeleteFilesAsync(List<string> fileNames, string folderName)
        {
            if (fileNames == null || !fileNames.Any())
                return;

            var tasks = fileNames.Select(fileName =>
            {
                var objectKey = $"{folderName}/{fileName}";

                return _minio.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_bucket)
                        .WithObject(objectKey)
                );
            });

            await Task.WhenAll(tasks);
        }

        // ==========================================
        // HELPER
        // ==========================================
        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}