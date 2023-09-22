using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine;
using JWTResolver = Networking.JWTTokenResolver.JWTTokenResolver;

namespace Networking.WebRequestHandler
{
    // json Serializer/Deserializer : com.unity.nuget.newtonsoft-json
    public static class WebRequestHandler
    {
        public static KeyValuePair<string, string>[] defaultHeaders =>
            new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Content-Type", "application/json"),
                new KeyValuePair<string, string>("Accept", "application/json"),
            };

        public delegate void RequestCallback(UnityWebRequest.Result result, long responseCode, DownloadHandler downloadHandler, Dictionary<string,string> responseHeaders);

        private static Action<Action> TokenResolver;
        public static void SetTokenResolver(Action<Action> tokenResolver) =>
            TokenResolver = tokenResolver;

        public static void SendData(HTTPRequestMethod method, string url, bool haveAuth = true, RequestCallback callbackAction = null, List<KeyValuePair<string, string>> headers = default, string bodyJson = default) =>
            SendData(method, url, haveAuth, callbackAction, headers, bodyJson,null,null);
        public static void SendData(string url, bool haveAuth = true, RequestCallback callbackAction = null, List<KeyValuePair<string, string>> headers = default, WWWForm postForm = default) =>
            SendData(HTTPRequestMethod.POST, url, haveAuth, callbackAction, headers, null, postForm,null);
        public static void SendData(string url, bool haveAuth = true, RequestCallback callbackAction = null, List<KeyValuePair<string, string>> headers = default, Dictionary<string, string> postFields = default) =>
            SendData(HTTPRequestMethod.POST, url, haveAuth, callbackAction, headers, null, null, postFields);

        private static void SendData(HTTPRequestMethod method, string url, bool haveAuth, RequestCallback callbackAction,
            List<KeyValuePair<string, string>> headers, string bodyJson, WWWForm postForm, Dictionary<string, string> postFields)
        {
            if (headers == null)
                headers = new List<KeyValuePair<string, string>>();

            void CallBack(UnityWebRequest request) =>
                callbackAction(request.result, request.responseCode, request.downloadHandler, request.GetResponseHeaders());
            void ReviewToken(UnityWebRequest request)
            {
                if (request.responseCode != (int)HTTPResponseCodes.UNAUTHORIZED_401)
                {
                    CallBack(request);
                    return;
                }

                JWTResolver.RemoveAccessToken();

                if (TokenResolver != null)
                    TokenResolver(() => SendData(method, url, haveAuth, callbackAction, headers, bodyJson));
                else
                    CallBack(request);
            }

            // Access Token
            if (haveAuth && !string.IsNullOrEmpty(JWTResolver.AccessToken))
                headers.Add(JWTResolver.AccessTokenHeader);

            headers.AddRange(defaultHeaders);

            Debug.Log(url + "\n" + method + " \nauth: " + haveAuth + " \n" + HeadersToString(headers.ToArray()) + bodyJson);

            if (postForm != null)
                HTTPService.SendData(url, haveAuth ? ReviewToken : CallBack, headers.ToArray(), postForm);
            else if (postFields != null)
                HTTPService.SendData(url, haveAuth ? ReviewToken : CallBack, headers.ToArray(), postFields);
            else
                HTTPService.SendData(method, url, haveAuth ? ReviewToken : CallBack, headers.ToArray(), bodyJson);
        }

        private static string HeadersToString(KeyValuePair<string, string>[] headers)
        {
            string str = "";
            for (int i = 0; i < headers.Length; i++)
                str += headers[i].Key + ": "+ headers[i].Value +"\n";

            return str;
        }
    }
       
}