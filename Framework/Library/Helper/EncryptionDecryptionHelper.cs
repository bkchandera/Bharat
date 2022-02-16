using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Library.Helper
{
    public static class EncryptionDecryptionHelper
    {
        /// <summary>
        /// This method for convert your plain string to MD5 Hash using with ASCII encoding.
        /// </summary>
        /// <param name="plain_string">Pass any plain string</param>
        public static string CreateStringHash(string plain_string)
        {
            using (var md5 = MD5.Create())
            {
                var value = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(plain_string));
                return BitConverter.ToString(value).Replace("-", "").ToString().ToLower();
            }
        }
    }
}
