using System;
using System.Security.Cryptography;
using System.Text;

namespace Rental.Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static Guid GetIdentifier(this object[] keys)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.Default.GetBytes(string.Concat(keys)));
            return new Guid(hash);
        }
    }
}
