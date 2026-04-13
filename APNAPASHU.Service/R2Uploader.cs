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
        public async Task UploadFilesAsync(List<IFormFile> files, List<string> fileNames, string folderName)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("Files are required");

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Folder name is required");

            if (files.Count != fileNames.Count)
                throw new ArgumentException("Files and fileNames count mismatch");

            var tasks = files.Select(async (file, index) =>
            {
                var fileName = fileNames[index]; // use provided name
                var objectKey = $"{folderName}/{fileName}";

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(objectKey)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(file.ContentType);

                await _minio.PutObjectAsync(putObjectArgs);
            });

            await Task.WhenAll(tasks);
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
        // DELETE ENTIRE FOLDER (BY PREFIX)
        // ==========================================
        public async Task DeleteFolderAsync(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return;

            // Ensure prefix ends with / to strictly target the directory contents
            var prefix = folderName.EndsWith("/") ? folderName : $"{folderName}/";

            var listArgs = new ListObjectsArgs()
                .WithBucket(_bucket)
                .WithPrefix(prefix)
                .WithRecursive(true);

            // Minio client returns IObservable<Item> for listing
            var deleteList = new List<string>();
            var tcs = new TaskCompletionSource<bool>();

            using (var subscription = _minio.ListObjectsAsync(listArgs).Subscribe(
                item => {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        deleteList.Add(item.Key);
                    }
                },
                ex => tcs.SetException(ex),
                () => tcs.SetResult(true)
            ))
            {
                await tcs.Task;
            }

            if (deleteList.Any())
            {
                var removeArgs = new RemoveObjectsArgs()
                    .WithBucket(_bucket)
                    .WithObjects(deleteList);

                await _minio.RemoveObjectsAsync(removeArgs);
            }
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