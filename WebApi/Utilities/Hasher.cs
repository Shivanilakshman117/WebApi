using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApi.Utilities
{
    public class Hasher
    {
        public static string HashString(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );
        }
        public static string EncodeId(string id)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(id));

        }

        public static string DecodeId(string encodedId)
        {
            int mod4 = encodedId.Length % 4;
            if (mod4 > 0)
            {
                encodedId += new string('=', 4 - mod4);
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedId));
        }

        }
    }
