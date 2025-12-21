using System;
using System.Numerics;

namespace EnemyEditor
{
    public class BigNumber
    {
        private BigInteger value;
        private const int Base = 1000; 


        public BigNumber()
        {
            value = BigInteger.Zero;
        }

        public BigNumber(int initialValue)
        {
            value = new BigInteger(initialValue);
        }

        public BigNumber(double initialValue)
        {
            value = new BigInteger((long)initialValue);
        }
        public BigNumber(long initialValue)
        {
            value = new BigInteger(initialValue);
        }
        public BigNumber(BigInteger bigInteger)
        {
            value = bigInteger;
        }

        public BigNumber Add(BigNumber other)
        {
            return new BigNumber(value + other.value);
        }

        public BigNumber Add(int other)
        {
            return new BigNumber(value + other);
        }

        public BigNumber Subtract(BigNumber other)
        {
            return new BigNumber(value - other.value);
        }

        public BigNumber Multiply(double multiplier)
        {
            decimal decimalMultiplier = (decimal)multiplier;
            BigInteger result = (BigInteger)((decimal)value * decimalMultiplier);
            return new BigNumber(result);
        }

        public BigNumber Multiply(BigNumber other)
        {
            return new BigNumber(value * other.value);
        }

        public BigNumber Divide(double divisor)
        {
            if (divisor == 0) throw new DivideByZeroException();
            decimal decimalDivisor = (decimal)divisor;
            BigInteger result = (BigInteger)((decimal)value / decimalDivisor);
            return new BigNumber(result);
        }

        public bool GreaterThan(BigNumber other)
        {
            return value > other.value;
        }

        public bool GreaterThanOrEqual(BigNumber other)
        {
            return value >= other.value;
        }

        public bool LessThan(BigNumber other)
        {
            return value < other.value;
        }

        public bool LessThanOrEqual(BigNumber other)
        {
            return value <= other.value;
        }

        public bool Equals(BigNumber other)
        {
            return value == other.value;
        }

        public int ToInt()
        {
            return (int)value;
        }

        public double ToDouble()
        {
            return (double)value;
        }

        public override string ToString()
        {
            return FormatNumber(value);
        }

        private string FormatNumber(BigInteger num)
        {
            if (num < 1000) return num.ToString();

            string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No" };
            int suffixIndex = 0;
            BigInteger currentValue = num;

            while (currentValue >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                currentValue /= 1000;
                suffixIndex++;
            }

            double formattedValue = (double)currentValue + (double)(num % 1000) / 1000.0;
            return $"{formattedValue:F2}{suffixes[suffixIndex]}";
        }

        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Subtract(b);
        public static BigNumber operator *(BigNumber a, double b) => a.Multiply(b);
        public static bool operator >(BigNumber a, BigNumber b) => a.GreaterThan(b);
        public static bool operator <(BigNumber a, BigNumber b) => a.LessThan(b);
        public static bool operator >=(BigNumber a, BigNumber b) => a.GreaterThanOrEqual(b);
        public static bool operator <=(BigNumber a, BigNumber b) => a.LessThanOrEqual(b);
        public static bool operator ==(BigNumber a, BigNumber b) => a.Equals(b);
        public static bool operator !=(BigNumber a, BigNumber b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (obj is BigNumber other)
                return value == other.value;
            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}