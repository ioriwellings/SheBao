using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class MathEx
    {
        /// <summary>
        /// 远离 0 向上舍入
        /// </summary>
        public static decimal RoundUp(this decimal value, int digits)
        {
            if (digits == 0)
            {
                return (value >= 0 ? decimal.Ceiling(value) : decimal.Floor(value));
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, digits));
            return (value >= 0 ? decimal.Ceiling(value * multiple) : decimal.Floor(value * multiple)) / multiple;
        }

        /// <summary>
        /// 靠近 0 向下舍入
        /// </summary>
        public static decimal RoundDown(this decimal value, int digits)
        {
            if (digits == 0)
            {
                return (value >= 0 ? decimal.Floor(value) : decimal.Ceiling(value));
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, digits));
            return (value >= 0 ? decimal.Floor(value * multiple) : decimal.Ceiling(value * multiple)) / multiple;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        public static decimal RoundEx(this decimal value, int digits)
        {
            if (digits >= 0)
            {
                return decimal.Round(value, digits, MidpointRounding.AwayFromZero);
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, -digits));
            return decimal.Round(value / multiple, MidpointRounding.AwayFromZero) * multiple;
        }

        /// <summary>
        /// 远离 0 向上舍入
        /// </summary>
        public static double RoundUp(this double value, int digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundUp(digits));
        }

        /// <summary>
        /// 靠近 0 向下舍入
        /// </summary>
        public static double RoundDown(this double value, int digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundDown(digits));
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        public static double RoundEx(this double value, int digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundEx(digits));
        }
    }
}
