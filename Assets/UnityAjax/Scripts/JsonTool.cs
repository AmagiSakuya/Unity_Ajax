using RSG;
using System;
using LitJson;

namespace UnityAjax
{
    public static class JsonTool
    {
        /// <summary>
        /// 尝试Json化
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="json">json数据</param>
        /// <returns></returns>
        public static Promise<T> ParseJson<T>(string json)
        {
            return new Promise<T>((reslove, reject) =>
            {
                T result;
                try
                {
                    result = JsonMapper.ToObject<T>(json);
                    reslove(result);
                }
                catch (Exception e)
                {
                    reject(e);
                }
            });
        }

        public static Promise<JsonData> ParseJson(string json)
        {
            return new Promise<JsonData>((reslove, reject) =>
            {
                JsonData result;
                try
                {
                    result = JsonMapper.ToObject(json);
                    reslove(result);
                }
                catch (Exception e)
                {
                    reject(e);
                }
            });
        }
    }
}

