using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    public static class RequestHelperConfig
    {
        public string BaseURL {private set; get;}
        public bool UseBearerPrefixAuthHeader {private set; get;} = true;
    }

    public static class RequestHelper 
    {
        public static RequestHelperConfig Config = new RequestHelperConfig();

        private static Action onTokenExpired { get; set; }
        private static void OnTokenExpired(Action action) =>
            onTokenExpired = action;

        private static Action<bool> LoadingAction;
        public static void SetLoading(Action<bool> action) =>
            LoadingAction = action;

        private static string token = null;
        public static void SetToken(string token) =>
            UHTTP.token = token;

        public static UnityWebRequest CreateRequest(string appendUrl, string method, string body = null, List<KeyValuePair<string, string>> headers = default)
        {
            UnityWebRequest req = new UnityWebRequest(Config.BaseURL + appendUrl, method);

            req.downloadHandler = new DownloadHandlerBuffer();
            if(body != null)
                req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

            foreach (var header in headers)
                req.SetRequestHeader(header.Key, header.Value);

            return req;
        }

        public static void AddToken(this UnityWebRequest request) 
            => request.SetRequestHeader("Authorization",Config.UseBearerPrefixAuthHeader ? $"Bearer {token}" : token);

        public static void Send(this UnityWebRequest request, Action onComplete = null,bool addTokenIfExist = true, bool haveLoading = false)
        {
            if(haveLoading)
                LoadingAction?.Invoke(true);

            if(addTokenIfExist)
                request.AddToken();

            request.SendWebRequest().completed += Response;

            void Response(AsyncOperation ao)
            {
                if(haveLoading) LoadingAction?.Invoke(false);

                if(request.responseCode == 401 && addTokenIfExist && token != null && onTokenExpired != null)
                {
                    token = null;
                    onTokenExpired();
                    return;
                }

                onComplete?.Invoke();
            }
        }
    }
}