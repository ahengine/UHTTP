using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UHTTP.JWTTokenResolver;
using static UHTTP.HTTPRequestCoroutineRunner;


namespace UHTTP
{
    public class HTTPRequest
    {
        public HTTPRequestData data;
        public Action<UnityWebRequest> callback;
        public UnityWebRequest webRequest;

        public Func<UnityWebRequest, UnityWebRequest> AddOptionsRequest;

        public HTTPRequest(HTTPRequestData data,Action<UnityWebRequest> callback) 
        {
            this.data = data;
            this.callback = callback;
        }

        private UnityWebRequest CreateRequest()
        {
            UnityWebRequest CreateWebRequest()
            {
                if(data.PostFields.Count > 0)
                    return UnityWebRequest.Post(data.URL, data.PostFields);
                
                if(data.PostFormFields.Count > 0)
                    return UnityWebRequest.Post(data.URL, data.PostFormFields);

                return new UnityWebRequest()
                    {
                        method = data.Method,
                        url = data.URL
                    };
            }

            UnityWebRequest request = CreateWebRequest();
            
            void AddBody()
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data.BodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Add Body
            if (!string.IsNullOrEmpty(data.BodyJson))
                AddBody();

            void SetHeaders()
            {
                List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

                if (data.Headers != null)
                    headers.AddRange(data.Headers);

                // Add JWT
                if (data.HaveAuth && !string.IsNullOrEmpty(AccessToken))
                    headers.Add(AccessTokenHeader);

                // Set
                if (headers != null && headers.Count > 0)
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, header.Value);
            }

            SetHeaders();

            if (AddOptionsRequest != null)
                request = AddOptionsRequest(request);

            return request;
        }

        public void Send() 
        {
            webRequest = CreateRequest();
            Run(SendCoroutine(webRequest));
        }

        public Coroutine SendCoroutine()
        {
            webRequest = CreateRequest();
            return Run(SendCoroutine(webRequest));
        }

        private IEnumerator SendCoroutine(UnityWebRequest request)
        {
            bool DoRenewToken()
            {
                bool CanRenewToken() 
                {
                    bool FailedAuthorize = 
                        request.responseCode == (int)HTTPResponseCodes.UNAUTHORIZED_401 || request.responseCode == (int)HTTPResponseCodes.FORBIDEN_403;

                    return data.HaveAuth && FailedAuthorize && TokenExpiredSterategy != null;
                }

                if (CanRenewToken())
                {
                    RemoveAccessToken();
                    TokenExpiredSterategy.Invoke(Send);
                    return true;
                }

                return false;
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (!DoRenewToken())
                callback?.Invoke(request);

            request.Dispose();
        }
    }
}