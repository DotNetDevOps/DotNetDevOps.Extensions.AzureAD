using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace DotNetDevOps.Extensions.AzureAD.Publisher
{
    public static class CustomSerializer
    {
         public static JsonSerializerOptions serializer = new JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
        public static byte[] SerializeToUtf8Bytes<T>(T arg)
        {
            return JsonSerializer.SerializeToUtf8Bytes(arg, serializer);
            
        }

        public static T Deserialize<T>(byte[] binaryValue)
        {
            return JsonSerializer.Deserialize<T>(binaryValue.AsSpan(), serializer);
          }
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, serializer);
           
        }

        public static string AsHash(this string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
