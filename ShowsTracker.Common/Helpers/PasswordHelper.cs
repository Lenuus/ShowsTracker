using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Common.Helpers
{
    public class PasswordHelper
    {
        private const int _saltSize = 16; // 128 bits
        private const int _keySize = 32; // 256 bits
        private const int _iterations = 50000;
        private const char segmentDelimiter = ':';
        private readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;
        private readonly IConfiguration _configuration;

        public PasswordHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Hash(string input)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return string.Join(
                segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                _iterations,
                _algorithm
            );
        }

        public bool Verify(string input, string hashString)
        {
            string[] segments = hashString.Split(segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );
            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }

        public bool PasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            bool includeUpperCase = Convert.ToBoolean(_configuration["PasswordValidation:IncludeUpperCase"]);
            bool includeLowerCase = Convert.ToBoolean(_configuration["PasswordValidation:IncludeLowerCase"]);
            bool includeNumber = Convert.ToBoolean(_configuration["PasswordValidation:IncludeNumber"]);
            int minumumLength = Convert.ToInt32(_configuration["PasswordValidation:MinumumLength"]);
            bool includeSpecialCharacters = Convert.ToBoolean(_configuration["PasswordValidation:IncludeSpecialCharacters"]);

            bool passwordValid = true;
            passwordValid = password.Length > minumumLength;
            if (!passwordValid)
            {
                return false;
            }

            if (includeUpperCase)
            {
                passwordValid = password.Any(f => char.IsAsciiLetterUpper(f));
            }
            if (!passwordValid)
            {
                return false;
            }

            if (includeLowerCase)
            {
                passwordValid = password.Any(f => char.IsAsciiLetterLower(f));
            }
            if (!passwordValid)
            {
                return false;
            }

            if (includeNumber)
            {
                passwordValid = password.Any(f => char.IsNumber(f));
            }
            if (!passwordValid)
            {
                return false;
            }

            if (includeSpecialCharacters)
            {
                passwordValid = password.Any(f => char.IsSymbol(f));
            }
            if (!passwordValid)
            {
                return false;
            }
            return passwordValid;
        }
    }
}
