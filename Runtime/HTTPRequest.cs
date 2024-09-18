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
        public Action<UnityWebRequest> callbackStream;
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
                
                var form = data.Form();
                if (form != null)
                    return UnityWebRequest.Post(data.URL, form);

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

        public void Send() =>
            Run(SendCoroutine(CreateRequest()));
        public Coroutine SendCoroutine()
        {
            webRequest = CreateRequest();
            return Run(SendCoroutine(webRequest));
        }
        private IEnumerator SendCoroutine(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest();

            while (!request.isDone)
            {
                callbackStream?.Invoke(request);
                yield return null;
            }

            if (!DoRenewToken(request))
                callback?.Invoke(request);

            request.Dispose();
        }   

        public void SendAsync() =>
            SendAsyncBase(CreateRequest());
        private void SendAsyncBase(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest().completed += (op) => 
            {            
                if (!DoRenewToken(request))
                    callback?.Invoke(request);

                request.Dispose();
            };
        }

        private bool DoRenewToken(UnityWebRequest request)
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
                    TokenExpiredSterategy.Invoke(Send,()=> callback?.Invoke(request));
                    return true;
                }

                return false;
            }
    }
}