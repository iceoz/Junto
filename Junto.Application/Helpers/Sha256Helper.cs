using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Junto.Application.Helpers
{
    public static class Sha256Helper
    {
        public static string Convert(string text)
        {
            using (var sha256 = SHA256.Create())
            {

                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                var hash = BitConverter.ToString(hashedBytes).ToLower();

                return hash;
            }
        }
    }
}
