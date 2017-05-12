using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
namespace Common
{
    /// <summary>
    ///CardCommon 的摘要说明
    /// </summary>
    public class CardCommon
    {
        public CardCommon()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 检查身份证号格式是否正确
        /// </summary>
        public static Boolean CheckCardID(string CardID)
        {
            bool ret = false;
            ret = Regex.IsMatch(CardID, @"^(\d{15}$)|(\d{17}(?:\d|x|X)$)");
            return ret;
        }

        /// <summary>
        /// 身份证号转换为15位身份证号
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public static string CardIDTo15(string CardID)
        {
            string CardID15 = "";
            CardID = CardID.Trim();
            if (Regex.IsMatch(CardID, @"(^\d{15}$)|(\d{17}(?:\d|x|X)$)"))
            {
                if (CardID.Length == 18)
                {
                    CardID15 = CardID.Substring(0, 6) + CardID.Substring(8, 9);
                }
                else
                {
                    CardID15 = CardID;
                }
            }
            return CardID15;
        }

        /// <summary>
        /// 身份证号转换为18位身份证号
        /// </summary>
        /// <param name="CardID">位身份证号</param>
        /// <returns>18位身份证号（如果返回空值说明身份证号不合法）</returns>
        public static string CardIDTo18(string CardID)
        {
            string CardID18 = "";
            CardID = CardID.Trim();
            if (Regex.IsMatch(CardID, @"(^\d{15}$)|(\d{17}(?:\d|x|X)$)"))
            {
                if (CardID.Length == 18)
                {
                    CardID = CardIDTo15(CardID.ToUpper());

                }
                string _CardID = CardID.Insert(6, "19");
                string[] arrVarifyCode = ("1,0,X,9,8,7,6,5,4,3,2").Split(',');  //ai18
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');   //wi
                char[] Ai = _CardID.ToCharArray();  //ai

                int AiWiSum = 0;
                for (int i = 0; i < 17; i++)
                {
                    AiWiSum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int Y = AiWiSum % 11;

                CardID18 = _CardID + arrVarifyCode[Y];
            }
            return CardID18;
        }

        /// <summary>
        /// 检测18位身份证号是否合法
        /// </summary>
        /// <param name="CardID18"></param>
        /// <returns></returns>
        public static bool CheckCardID18(string CardID18)
        {
            bool _Checked = false;
            CardID18 = CardID18.Trim().ToUpper();
            if (CardID18.Length > 18)
            {
                return _Checked;
            }
            if (!Regex.IsMatch(CardID18, @"(\d{17}(?:\d|x|X)$)"))
            {
                return _Checked;
            }

            string[] arrVarifyCode = ("1,0,X,9,8,7,6,5,4,3,2").Split(',');  //ai18
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');   //wi
            char[] Ai = CardID18.Remove(17).ToCharArray();  //ai

            int AiWiSum = 0;
            for (int i = 0; i < 17; i++)
            {
                AiWiSum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int Y = AiWiSum % 11;
            if (arrVarifyCode[Y] == CardID18.Substring(17, 1).ToUpper())
            {
                _Checked = true;
            }
            return _Checked;
        }

        #region 根据身份证号获取性别
        /// <summary>
        /// 返回1男返回2女
        /// </summary>
        /// <param name="cardid">身份证号</param>
        /// <returns></returns>
        public static int Getsex(string cardid)
        {

            int sexint = 0;
            string se = "0";
            if (cardid.Length == 15)
            {
                se = cardid.Substring(13, 1);
            }
            else if (cardid.Length == 18)
            {
                se = cardid.Substring(16, 1);
            }
            if (se == "1" || se == "3" || se == "5" || se == "7" || se == "9")
            {
                sexint = 1;
            }
            else
            {
                sexint = 2;
            }
            return sexint;
        }
        #endregion
        #region 根据身份证号获得生日
        /// <summary>
        /// 根据身份证号获得生日
        /// </summary>
        /// <param name="cardid">身份证号</param>
        /// <returns></returns>
        public static DateTime GetShengRi(string cardid)
        {
            DateTime dt = DateTime.Now;
            if (cardid.Length == 18)
            {
                dt = Convert.ToDateTime(cardid.Substring(6, 4) + "-" + cardid.Substring(10, 2) + "-" + cardid.Substring(12, 2));
            }
            if (cardid.Length == 15)
            {
                dt = Convert.ToDateTime("19" + cardid.Substring(7, 2) + "-" + cardid.Substring(9, 2) + "-" + cardid.Substring(11, 2));
            }
            return dt;
        }

        #endregion
        #region 根据身份证号获得年龄
        /// <summary>
        /// 根据身份证号获得年龄
        /// </summary>
        /// <param name="cardid">身份证号</param>
        /// <returns></returns>
        public static string GetNianLing(string cardid)
        {
            string dt = "";
            //if (cardid.Length == 18)
            //{
            //    dt = (DateTime.Now.Year - Convert.ToInt32(cardid.Substring(6, 4)) + 1).ToString();
            //}
            //if (cardid.Length == 15)
            //{
            //    dt = (DateTime.Now.Year - Convert.ToInt32("19" + cardid.Substring(7, 2)) + 1).ToString();
            //}
            //return dt;

            if (cardid.Length == 18)
            {
                string Sub_str = cardid.Substring(6, 8).Insert(4, "-").Insert(7, "-"); //提取出生年份
                TimeSpan ts = DateTime.Now.Subtract(Convert.ToDateTime(Sub_str));
                dt = (ts.Days / 365).ToString();
            }
            if (cardid.Length == 15)
            {
                string Sub_str = "19" + cardid.Substring(6, 6).Insert(2, "-").Insert(5, "-"); //提取出生年份          
                TimeSpan ts = DateTime.Now.Subtract(Convert.ToDateTime(Sub_str));
                dt = (ts.Days / 365).ToString();

            }
            return dt;
        }

        #endregion

        #region
        /// <summary>
        /// 18位身份证号最后一位X转成大写
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public static string Card_ZDX(string CardID)
        {
            string dt = "";
            if (CardID.Length == 18)
            {
                if (CardID.Substring(17, 1) == "X" || CardID.Substring(17, 1) == "x")
                {
                    dt = CardID.Substring(0, 17) + "X";
                }
                else
                {
                    dt = CardID;
                }
            }
            if (CardID.Length == 15)
            {
                dt = CardID;
            }
            return dt;
        }
        #endregion

        #region 判断手机号是否合法
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1\\d{10}$");
            return regex.IsMatch(input);
        }
        #endregion

        #region 判断邮箱是否合法
        public static bool IsEmail(string input)
        {
            Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            return regex.IsMatch(input);
        }
        #endregion
        #region 判断日期是否合法
        public static bool IsDate(string StrSource) { return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$"); }
        #endregion
    }
}