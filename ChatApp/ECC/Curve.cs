using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Globalization;
using System.Numerics;

namespace ChatApp.ECC
{
    /// <summary>
    /// ECC.NET elliptic-curve class.
    /// </summary>
    public class Curve
    {
        public enum CurveName
        {
            secp256r1,
            secp256k1
        }

        public CurveName? Name { get; private set; }
        public BigInteger P { get; private set; }   //Eğrinin bir elemanı olan asal bir sayı. Modüler aritmetik işlemleri için kullanılır.
        public BigInteger A { get; private set; }
        public BigInteger B { get; private set; }
        public Point G { get; private set; }    // başlangıç noktası
        public BigInteger N { get; private set; }
        public short H { get; private set; }
        public uint Length { get; private set; }

        /// <summary>
        /// Creates known elliptic-curve instance: secp256k1.
        /// </summary>
        /// <param name="name">Name of target curve.</param>
        public Curve(CurveName name)
        {
            switch (name)
            {
                
                case CurveName.secp256r1:
                    {
                        Name = name;

                        P = BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber);

                        A = BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFC", NumberStyles.HexNumber);
                        B = BigInteger.Parse("005AC635D8AA3A93E7B3EBBD55769886BC651D06B0CC53B0F63BCE3C3E27D2604B", NumberStyles.HexNumber);

                        G = new Point(BigInteger.Parse("006B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", NumberStyles.HexNumber), BigInteger.Parse("004FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", NumberStyles.HexNumber), this);

                        N = BigInteger.Parse("00FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551", NumberStyles.HexNumber);
                        H = 1;

                        Length = 256;
                    }
                    break;
                case CurveName.secp256k1:
                    {
                        Name = name;

                        P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", NumberStyles.HexNumber);

                        A = 0;
                        B = 7;

                        G = new Point(BigInteger.Parse("79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.HexNumber), BigInteger.Parse("483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.HexNumber), this);

                        N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", NumberStyles.HexNumber);
                        H = 1;

                        Length = 256;
                    }
                    break;
                    //throw new UnknownCurveException($"Unknown curve ({name.ToString()}) has been requested.");
            }
        }

        /// <summary>
        /// Checks whether a point is on the elliptic-curve.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>True if point is on the elliptic-curve.</returns>
        public bool IsOnCurve(Point point)
        { return Point.IsInfinityPoint(point) ? true : ((BigInteger.Pow(point.Y, 2) - BigInteger.Pow(point.X, 3) - A * point.X - B) % P) == 0; }
        //=> 
        /// <summary>
        /// Checks whether a point is valid.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="exception">Exception to be thrown if the point is not valid.</param>
        public bool CheckPoint(Point point)
        {
            return IsOnCurve(point);  
        }
    }
}
