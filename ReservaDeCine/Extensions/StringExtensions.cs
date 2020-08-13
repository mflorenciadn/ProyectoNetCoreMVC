using System.Security.Cryptography;
using System.Text;

namespace MVC.ReservaDeCine.Extensions
{
    public static class StringExtensions
    {
        public static byte[] Encriptar(this string data) =>
            new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(data));
    }

}
