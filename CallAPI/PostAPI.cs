
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace Langben.CallAPI
{
    public static class PostAPI
    {
        #region 同步方法

        /// <summary>
        /// 获取Post结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnText"></param>
        /// <returns></returns>
        public static T GetResult<T>(string returnText)
        {
          
            if (returnText.Contains("errcode"))
            {
                //可能发生错误

            }
            T result = JsonConvert.DeserializeObject<T>(returnText);

            return result;
        }
        public static string PostGetJson<T>(string url, T entity)
        {
          
            return RequestUtility.HttpPost(url, JsonConvert.SerializeObject(entity));

        }
        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = RequestUtility.HttpPost(url, cookieContainer, formData, encoding, timeOut: timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        #endregion


    }
}
