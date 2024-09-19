using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UHTTP;
using static UHTTP.HTTPRequestCoroutineRunner;
using static UHTTP.JWTTokenResolver;

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
                    headers.Add(new KeyValuePair<string, string>(HTTPHeaders.HEADER_AUTHORIZATION_KEY, AccessToken));

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

            if (!DoRenewToken(request,Send,callback))
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
                if (!DoRenewToken(request,Send,callback))
                    callback?.Invoke(request);

                request.Dispose();
            };
        }
    }
}