using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Utilities
{
    public static class DecimalMath
    {
        public static readonly decimal Pi = 3.14159265358979323846264338327950288419716939937510M;
        public static readonly decimal PiTimes2 = 6.28318530717958647692528676655900576839433879875021M;
        public static readonly decimal PiHalf = 1.570796326794896619231321691639751442098584699687552910487M;

        public static readonly decimal E = 2.7182818284590452353602874713526624977572470936999595749M;
        public static readonly decimal Einv = 0.3678794411714423215955237701614608674458111310317678M;
        public static readonly decimal LOG2 = 0.693147180559945309417232121458176568075500134360255254120M;

        public static readonly decimal Zero = 0.0M;
        public static readonly decimal One = 1.0M;

        public static int MaxIteration = 100;
        public static decimal Exp(decimal x)
        {
            int say = 0;
            while (x > One)
            {
                x--;
                say++;
            }
            while (x < Zero)
            {
                x++;
                say--;
            }
            int iteration = 1;
            decimal result = One;
            decimal fatorial = One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                fatorial *= x / iteration++;
                result += fatorial;
                //if(iteration%100==0) Console.WriteLine("result"+ result);
            } while (cachedResult != result);
            if (say != 0) result = result * PowerN(E, say);
            return result;
        }

        public static decimal Power(decimal val, decimal pow)
        {
            return Exp(pow * Log(val));
        }

        public static decimal PowerN(decimal value, int power)
        {
            var q = power;
            var prod = One;
            var current = value;
            while (q > 0)
            {
                if (q % 2 == 1)
                {
                    // detects the 1s in the binary expression of power
                    prod = current * prod; // picks up the relevant power
                    q--;
                }
                current = current * current; // value^i -> value^(2*i)
                q = q / 2;
            }

            return prod;
        }
        // natural logarithm series
        public static decimal Log(decimal x)
        {
            if (x <= 0)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            int count = 0;
            while (x >= E)
            {
                x *= Einv;
                count++;
            }
            while (x <= Einv)
            {
                x *= E;
                count--;
            }
            x--;
            if (x == 0) return count;
            decimal result = 0.0M;
            int iteration = 0;
            decimal y = 1.0M;
            decimal cacheResult = result - 1.0M;
            while (cacheResult != result && iteration < MaxIteration)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
                //Console.WriteLine(result);
            }
            return count - result;
        }

        public static decimal Cos(decimal x)
        {
            while (x > PiTimes2)
            {
                x -= PiTimes2;
            }
            while (x < -PiTimes2)
            {
                x += PiTimes2;
            }
            // now x in (-2pi,2pi)
            if (x >= Pi && x <= PiTimes2)
            {
                return -Cos(x - Pi);
            }
            if (x >= -PiTimes2 && x <= -Pi)
            {
                return -Cos(x + Pi);
            }
            x = x * x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            decimal xx = -x * 0.5M;
            decimal y = 1.0M + xx;
            decimal cachedY = y - 1.0M;//init cache  with different value
            //10 iterations
            for (int i = 1; cachedY != y && i < MaxIteration; i++)
            {
                cachedY = y;
                //decimal factor = (2 * i + 1) * (i + 1);//2i^2+2i+i+1
                decimal factor = i * (i + i + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -0.5M / factor;
                xx *= x * factor;
                y += xx;
            }
            return y;
        }

        public static decimal Tan(decimal x)
        {
            return Sin(x) / Cos(x);
        }
        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            var real = Math.Sin((double)x);
            return Sqrt(1.0M - cos * cos) * Math.Sign(real);
        }
        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0.0M;
                current = (previous + x / previous) * 0.5M;
            } while (Math.Abs(previous - current) > epsilon);
            return current;
        }

        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y - yy) / 2.0M;
        }
        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y + yy) / 2.0M;
        }

        public static decimal Sign(decimal x)
        {
            return x < Zero ? -1 : (x > Zero ? 1 : 0);
        }

        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y - yy) / (y + yy);
        }
        /// <summary>
        /// y = x + 1/2*(x^3/3) + 1/2*3/4(x^5/5) + 1/2*3/4*5/6(x^7/7)
        /// 
        /// asin(t) = 0.5*asin(2*t*t-1)
        /// 2*t*t-1=x --> t=sqrt((x+1)/2)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>

        public static decimal Asin(decimal x)
        {
            if (x > One || x < -One)
            {
                throw new ArgumentException("x must be in [-1,1]");
            }
            if (x == Zero) return 0;
            if (x == One) return PiHalf;
            decimal y = 0;
            decimal result = x;
            decimal cachedResult;
            int i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (1 - 1.0M / (2.0M * i));
                y += result / (2 * i + 1);
                i++;
                //Console.WriteLine(y);
            } while (cachedResult != result);
            return y;
        }

        public static decimal Acos(decimal x)
        {
            return PiHalf - Asin(x);
        }
    }
}
