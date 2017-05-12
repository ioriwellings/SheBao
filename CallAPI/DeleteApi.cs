using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.CallAPI
{
    public static class DeleteApi
    {
        public static string DeleteGetJson<T>(string url)
        {
            return RequestUtility.HttpDelete(url);
        }
    }
}
