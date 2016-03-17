namespace LogApp.Security.Implementations
{
    using Interfaces;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordManager : IPasswordManager
    {
        public bool AreEqual(string password, string hashedPassword, string salt)
        {
            string passwordAfterHashing = EncryptPassword(password);
            return hashedPassword == EncryptPassword(passwordAfterHashing, salt);
        }

        public string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new System.ArgumentException(nameof(password));

            return Encrypt(password);
        }

        public string EncryptPassword(string hashedPassword, string salt)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(salt))
                throw new System.ArgumentException();


            return Encrypt(string.Concat(hashedPassword, salt));
        }

        public string GeneratePin()
        {
            Random generator = new Random();
            String pin = generator.Next(0, 1000000).ToString("D6");
            return pin;
        }

        public string GenerateRandomPassword(int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[64];
            rng.GetNonZeroBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        private string Encrypt(string value)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] raw = Encoding.Default.GetBytes(value);
                var result = sha256.ComputeHash(raw);
                StringBuilder sb = new StringBuilder();

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
