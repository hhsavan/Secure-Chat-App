using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ECC
{
    /// <summary>
    /// Common objects to more ECC.NET classes.
    /// </summary>
    internal static class Commons
    {
        internal static RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
    }
}
