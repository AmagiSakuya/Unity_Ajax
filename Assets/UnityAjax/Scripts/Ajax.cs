using System.Collections;
using UnityEngine;
using RSG;
using System;
using UnityEngine.Networking;
using System.Linq;
using LitJson;
using System.Reflection;
using System.Collections.Generic;

namespace UnityAjax
{
    public enum RequestType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class Ajax
    {
        public static Promise<T> Request<T>(MonoBehaviour mono, string url, RequestType requestType, object sendData = null, Dictionary<string, string> headers = null)
        {
            if (requestType == RequestType.GET)
            {
                return Get<T>(mono, url, sendData, headers);
            }
            else if (requestType == RequestType.POST)
            {
                return Post<T>(mono, url, sendData, headers);
            }
            else if (requestType == RequestType.PUT)
            {
                return Put<T>(mono, url, sendData, headers);
            }
            else
            {
                return Delete<T>(mono, url, sendData, headers);
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="mono">mono脚本</param>
        /// <param name="url">请求地址</param>
        /// <param name="sendData">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public static Promise<T> Get<T>(MonoBehaviour mono, string url, object sendData = null, Dictionary<string, string> headers = null)
        {
            if (sendData != null)
            {
                EachObjectField(sendData, (key, value, index) =>
                {
                    url += index == 0 ? "?" : "&";
                    url += $"{key}={value}";
                });
            }
            return new Promise<T>((resolve, reject) => { mono.StartCoroutine(GetRequest(url, resolve, reject, headers)); });
        }

        static IEnumerator GetRequest<T>(string url, Action<T> resolve, Action<SystemException> reject, Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            SetHeaders(webRequest, headers);
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error)) reject(new SystemException(webRequest.error));
            else OnHttpSuccess(resolve, reject, webRequest);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="mono">mono脚本</param>
        /// <param name="url">请求地址</param>
        /// <param name="sendData">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public static Promise<T> Post<T>(MonoBehaviour mono, string url, object sendData = null, Dictionary<string, string> headers = null)
        {
            WWWForm m_wwwForm = new WWWForm();
            if (sendData != null)
            {
                m_wwwForm = CreateWWWform(sendData);
            }
            return Post<T>(mono, url, m_wwwForm, headers);
        }

        static Promise<T> Post<T>(MonoBehaviour mono, string url, WWWForm sendData = null, Dictionary<string, string> headers = null)
        {
            return new Promise<T>((resolve, reject) =>
            {
                mono.StartCoroutine(PostRequest(url, sendData, resolve, reject, headers));
            });
        }

        static IEnumerator PostRequest<T>(string url, WWWForm formdata, Action<T> resolve, Action<SystemException> reject, Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(url, formdata);
            SetHeaders(webRequest, headers);
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error)) reject(new SystemException(webRequest.error));
            else OnHttpSuccess(resolve, reject, webRequest);
        }

        /// <summary>
        /// Put 请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="mono">mono脚本</param>
        /// <param name="url">请求地址</param>
        /// <param name="sendData">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public static Promise<T> Put<T>(MonoBehaviour mono, string url, object sendData = null, Dictionary<string, string> headers = null)
        {
            string m_sendJson = "{}";
            if (sendData != null)
            {
                m_sendJson = JsonUtility.ToJson(sendData);
            }
            return new Promise<T>((resolve, reject) =>
            {
                mono.StartCoroutine(PutRequest(url, m_sendJson, resolve, reject));
            });
        }

        static IEnumerator PutRequest<T>(string url, string sendData, Action<T> resolve, Action<SystemException> reject, Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Put(url, sendData);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            SetHeaders(webRequest, headers);
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error)) reject(new SystemException(webRequest.error));
            else OnHttpSuccess(resolve, reject, webRequest);
        }

        /// <summary>
        /// Delete 请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="mono">mono脚本</param>
        /// <param name="url">请求地址</param>
        /// <param name="sendData">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public static Promise<T> Delete<T>(MonoBehaviour mono, string url, object sendData = null, Dictionary<string, string> headers = null)
        {
            if (sendData != null)
            {
                EachObjectField(sendData, (key, value, index) =>
                {
                    url += index == 0 ? "?" : "&";
                    url += $"{key}={value}";
                });
            }
            return new Promise<T>((resolve, reject) =>
            {
                mono.StartCoroutine(DeleteRequest(url, resolve, reject));
            });
        }

        static IEnumerator DeleteRequest<T>(string url, Action<T> resolve, Action<SystemException> reject, Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Delete(url);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            SetHeaders(webRequest, headers);
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error)) reject(new SystemException(webRequest.error));
            else OnHttpSuccess(resolve, reject, webRequest);
        }

        /// <summary>
        /// 对象转WWWform
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static WWWForm CreateWWWform<T>(T data)
        {
            WWWForm requesForm = new WWWForm();
            EachObjectField(data, (key, value, index) =>
            {
                requesForm.AddField(key, value);
            });
            return requesForm;
        }

        static void OnHttpSuccess<T>(Action<T> reslove, Action<SystemException> reject, UnityWebRequest webRequest)
        {
            if (typeof(T) == typeof(string))
            {
                T dataText = (T)(object)webRequest.downloadHandler.text;
                reslove(dataText);
            }
            else if (typeof(T) == typeof(LitJson.JsonData))
            {
                JsonTool.ParseJson(webRequest.downloadHandler.text).Then((LitJson.JsonData result) =>
                {
                    reslove((T)(object)result);
                }).Catch((Exception e) =>
                {
                    reject(new SystemException(e.Message));
                });
            }
            else
            {
                JsonTool.ParseJson<T>(webRequest.downloadHandler.text).Then((T result) =>
                {
                    reslove(result);
                }).Catch((Exception e) =>
                {
                    reject(new SystemException(e.Message));
                });
            }
        }

        static void EachObjectField<T>(T data, Action<string, string, int> Callback)
        {
            FieldInfo[] fields = data.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                Callback(fields[i].Name, fields[i].GetValue(data).ToString().ToLower(), i);
            }
        }

        static void SetHeaders(UnityWebRequest webRequest, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }
        }
    }
}