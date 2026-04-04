using System.Security.Cryptography;
using System.Text;

namespace APNAPASHU.Common
{
    public class EncryptionDecryption
    {
        /// <summary>
        /// Create hashing for password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CreateHash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password can't be null");
            }

            // Create a SHA256   
            using SHA256 sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password).ToArray());

            // Convert byte array to a string   
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Password comparision.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, string passwordHash)
        {
            return CreateHash(password).Equals(passwordHash);
        }

        /// <summary>
        /// Salt creation logic for password.
        /// </summary>
        /// <returns></returns>
        public static string CreateSalt()
        {
            string randomString;
            byte[] randomArray = new byte[24];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomArray);
            randomString = Convert.ToBase64String(randomArray);

            return randomString;
        }
        
    }
}