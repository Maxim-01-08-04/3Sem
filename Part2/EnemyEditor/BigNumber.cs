using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class BigNumber
    {
        private List<int> number;
        private const int Base = 1000;
        private bool isNegative = false;

        public BigNumber(string numberStr)
        {
            if (string.IsNullOrEmpty(numberStr))
                numberStr = "0";

            if (numberStr.StartsWith("-"))
            {
                isNegative = true;
                numberStr = numberStr.Substring(1);
            }

            numberStr = numberStr.Replace(" ", "");
            ParseNumber(numberStr);
        }

        private BigNumber(List<int> digits, bool negative = false)
        {
            number = digits;
            isNegative = negative;
            TrimLeadingZeros();
        }

        private void ParseNumber(string numberStr)
        {
            number = new List<int>();

            for (int i = numberStr.Length; i > 0; i -= 3)
            {
                int start = Math.Max(0, i - 3);
                int length = Math.Min(3, i - start);
                string block = numberStr.Substring(start, length);

                if (int.TryParse(block, out int digit))
                {
                    number.Add(digit);
                }
                else
                {
                    number.Add(0);
                }
            }

            TrimLeadingZeros();
        }

        private void TrimLeadingZeros()
        {
            while (number.Count > 1 && number[number.Count - 1] == 0)
            {
                number.RemoveAt(number.Count - 1);
            }
        }

        public BigNumber Clone()
        {
            return new BigNumber(new List<int>(number), isNegative);
        }

        public override string ToString()
        {
            if (number.Count == 0) return "0";

            StringBuilder sb = new StringBuilder();

            if (isNegative)
                sb.Append("-");

            sb.Append(number[number.Count - 1].ToString());

            for (int i = number.Count - 2; i >= 0; i--)
            {
                sb.Append(number[i].ToString("D3"));
            }

            return sb.ToString();
        }

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a.isNegative && !b.isNegative)
                return b - new BigNumber(a.number, false);
            if (!a.isNegative && b.isNegative)
                return a - new BigNumber(b.number, false);
            if (a.isNegative && b.isNegative)
                return new BigNumber((new BigNumber(a.number, false) + new BigNumber(b.number, false)).number, true);

            List<int> result = new List<int>();
            int carry = 0;
            int maxLength = Math.Max(a.number.Count, b.number.Count);

            for (int i = 0; i < maxLength || carry > 0; i++)
            {
                int sum = carry;
                if (i < a.number.Count) sum += a.number[i];
                if (i < b.number.Count) sum += b.number[i];

                result.Add(sum % Base);
                carry = sum / Base;
            }

            return new BigNumber(result);
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            if (b.isNegative)
                return a + new BigNumber(b.number, false);
            if (a.isNegative)
                return new BigNumber((new BigNumber(a.number, false) + b).number, true);
            if (a < b)
                return new BigNumber((b - a).number, true);

            List<int> result = new List<int>();
            int borrow = 0;

            for (int i = 0; i < a.number.Count; i++)
            {
                int diff = a.number[i] - borrow;
                if (i < b.number.Count)
                    diff -= b.number[i];

                if (diff < 0)
                {
                    diff += Base;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.Add(diff);
            }

            return new BigNumber(result);
        }

        public static BigNumber operator *(BigNumber a, double multiplier)
        {
            if (multiplier == 0)
                return new BigNumber("0");

            BigNumber result = new BigNumber("0");
            BigNumber current = a.Clone();

            
            int iterations = (int)multiplier;
            double fraction = multiplier - iterations;

            for (int i = 0; i < iterations; i++)
            {
                result = result + current;
            }

            if (fraction > 0)
            {
                BigNumber fractionalPart = a / (1.0 / fraction);
                result = result + fractionalPart;
            }

            return result;
        }

        public static BigNumber operator /(BigNumber a, double divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();

            if (a.ToString() == "0")
                return new BigNumber("0");

            BigNumber result = new BigNumber("0");
            BigNumber temp = a.Clone();
            BigNumber divisorBig = new BigNumber(((int)divisor).ToString());

            while (temp >= divisorBig)
            {
                temp = temp - divisorBig;
                result = result + new BigNumber("1");
            }

            return result;
        }

        public static bool operator >(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) <= 0;
        }

        public int CompareTo(BigNumber other)
        {
            if (isNegative && !other.isNegative) return -1;
            if (!isNegative && other.isNegative) return 1;

            int comparison = 1;
            if (isNegative && other.isNegative)
                comparison = -1;

            if (number.Count != other.number.Count)
                return (number.Count > other.number.Count) ? comparison : -comparison;

            for (int i = number.Count - 1; i >= 0; i--)
            {
                if (number[i] != other.number[i])
                    return (number[i] > other.number[i]) ? comparison : -comparison;
            }

            return 0;
        }

        public static BigNumber Parse(string numberStr)
        {
            return new BigNumber(numberStr);
        }

        public bool IsZero()
        {
            return number.Count == 0 || (number.Count == 1 && number[0] == 0);
        }
    }
}
