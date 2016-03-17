using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogApp.Security.Interfaces
{
    public interface IPasswordManager
    {
        string EncryptPassword(string password);
        string EncryptPassword(string hashedPassword, string salt);
        string GenerateSalt();
        bool AreEqual(string password, string hashedPassword, string salt);
        string GenerateRandomPassword(int length = 8);
        string GeneratePin();
    }
}
