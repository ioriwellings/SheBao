using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Langben.CallAPI
{
    public static class PutApi
    {

        public static string PutGetJson<T>(string url, T entity)
        {
            return RequestUtility.HttpPut(url, JsonConvert.SerializeObject(entity));
        }
    }
}
