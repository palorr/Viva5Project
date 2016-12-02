using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace VivaWallet.Server.Web.Api.Helpers
{
    public static class HelperMethods
    {
        private static string GetUniqueKey(int maxSize = 15)
        {
            char[] chars = new char[62];
            chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string getImagePhotoName(string fileExtention)
        {
            return (GetUniqueKey(25) + (fileExtention ?? ".jpg"));
        }

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            sbText.Replace("#^data:image/[^;]+;base64,#", String.Empty);

            return Regex.Replace(sbText.ToString(), "data:image/(png|jpg|gif|jpeg|pjpeg|x-png);base64,", String.Empty);
        }
    }
}