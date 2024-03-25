using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ECC
{
    /// <summary>
	/// Operations with big numbers (BigInteger).
	/// </summary>
	public static class Numerics
    {
       
        /// <summary>
        /// Computes modulus of given value. The result is never negative.
        /// </summary>
        /// <param name="value">Original value.</param>
        /// <param name="modulo">Modulo to work with.</param>
        /// <returns></returns>
        public static BigInteger Modulus(BigInteger value, BigInteger modulo)
        {
            BigInteger reminder = value % modulo;

            return reminder < 0 ? reminder += modulo : reminder;
        }

        /// <summary>
        /// Computes inverse number to given value and modulo.
        /// </summary>
        /// <param name="value">Original value.</param>
        /// <param name="modulo">Modulo to work with.</param>
        /// <returns></returns>
        public static BigInteger ModularInverse(BigInteger value, BigInteger modulo)
        {
            if (value == 0)
                throw new DivideByZeroException();

            if (value < 0)
                return modulo - ModularInverse(-value, modulo);

            BigInteger a = 0, oldA = 1;
            BigInteger b = modulo, oldB = value;

            while (b != 0)
            {
                BigInteger quotient = oldB / b;

                BigInteger prov = b;
                b = oldB - quotient * prov;
                oldB = prov;

                prov = a;
                a = oldA - quotient * prov;
                oldA = prov;
            }

            BigInteger gcd = oldB;
            BigInteger c = oldA;

            if (gcd != 1)
                return -1;
                //throw new GreatestCommonDivisorException($"GCD is not 1, but {gcd}.");

            if (Modulus(value * c, modulo) != 1)
                throw new ArithmeticException("Modular inverse final check failed.");

            return Modulus(c, modulo);
        }

        /// <summary>
        /// Generates random number of specified length (in bits).
        /// </summary>
        /// <param name="length">Length of number in bits.</param>
        /// <returns></returns>
        public static BigInteger GetNumber(uint length)
        {
            byte[] randomOutput = new byte[length];
            Commons.randomNumberGenerator.GetBytes(randomOutput);

            return new BigInteger(randomOutput);
        }

    }
}
