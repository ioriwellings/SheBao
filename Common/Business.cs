using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Common
{
    public class Business
    {

        #region 保留小数，四舍五入
        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="SHU"></param>
        /// <returns></returns>
        public static decimal Get_TwoXiaoshu(decimal SHU)
        {
            return Math.Round(SHU, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region 计算时候补缴月数
        /// <summary>
        /// 计算补缴月数
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static int CHA_Months(DateTime dt1, DateTime dt2)
        {
            int cha = 0;
            cha = (dt2.Year - dt1.Year) * 12 + dt2.Month - dt1.Month;
            return cha;
        }
        #endregion

        #region 判断是不是decimal类型
        public static bool Is_Decimal(string str)
        {
            decimal dec = 0;
            if (decimal.TryParse(str, out dec))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 补缴时间判断
        public static DateTime bujiao_min(DateTime d1, DateTime s1)
        {
            if (d1 < s1)
                return d1;
            else
                return s1;
        }
        public static DateTime bujiao_max(DateTime d1, DateTime s1)
        {
            if (d1 < s1)
                return s1;
            else
                return d1;
        }
        #endregion

        #region 断一个字符串是否为合法数字(指定整数位数和小数位数)
        /**/
        /// <summary>  
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)  
        /// </summary>  
        /// <param name="s">字符串</param>  
        /// <param name="precision">整数位数</param>  
        /// <param name="scale">小数位数</param>  
        /// <returns></returns>  
        public static bool IsNumber(string s, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            string pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(s, pattern);
        }
        #endregion

        #region 生成二维码
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="massage">二维码中的信息</param>
        /// <returns></returns>
        public static string CreateBitmap(string massage)
        {
            string strResult = string.Empty;

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            //qrCodeEncoder.QRCodeScale = 4;

            qrCodeEncoder.QRCodeVersion = 0;

            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Bitmap image = qrCodeEncoder.Encode(massage);

            MemoryStream MStream = new MemoryStream();

            image.Save(MStream, ImageFormat.Png);

            try
            {
                byte[] byteImage = MStream.ToArray();
                strResult = Convert.ToBase64String(byteImage);

            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                MStream.Close();
            }
            return strResult;
        }

        #endregion
       
    }
}
