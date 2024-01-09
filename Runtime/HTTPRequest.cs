using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using JWTResolver = UHTTP.JWTTokenResolver;
using static UHTTP.HTTPRequestCoroutineRunner;
using System.Collections;
using UnityEngine;

namespace UHTTP
{
    public class HTTPRequest
    {
        public HTTPRequestData Data { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public Action<UnityWebRequest> callback;

        public Func<UnityWebRequest, UnityWebRequest> AddOptionsRequest;

        public HTTPRequest() { }
        public HTTPRequest(HTTPRequestData data) =>
            Data = data;

        public HTTPRequest SetCallback(Action<UnityWebRequest> callback) 
        {
            this.callback = callback;
            return this;
        }
           
        public void SetCard(HTTPRequestData data) =>
            Data = data;

        private UnityWebRequest CreateRequest()
        {
            UnityWebRequest CreateWebRequest()
            {
                if(Data.PostFields.Count > 0)
                    return UnityWebRequest.Post(Data.URL, Data.PostFields);
                
                if(Data.PostFormFields.Count > 0)
                    return UnityWebRequest.Post(Data.URL, Data.PostFormFields);

                return new UnityWebRequest()
                    {
                        method = Data.Method,
                        url = Data.URL
                    };
            }

            UnityWebRequest request = CreateWebRequest();
            
            void AddBody()
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(Data.BodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Add Body
            if (!string.IsNullOrEmpty(Data.BodyJson))
                AddBody();

            void SetHeaders()
            {
                List<KeyValuePair<string, string>> totalHeaders = new List<KeyValuePair<string, string>>();

                if (Data.Headers != null)
                    totalHeaders.AddRange(Data.Headers);

                // Add Defaults
                totalHeaders.AddRange(new KeyValuePair<string, string>[]  {
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Accept", "application/json")
                });

                // Add JWT
                if (Data.HaveAuth && !string.IsNullOrEmpty(JWTResolver.AccessToken))
                    totalHeaders.Add(JWTResolver.AccessTokenHeader);

                // Set
                if (totalHeaders != null && totalHeaders.Count > 0)
                    foreach (var header in totalHeaders)
                        request.SetRequestHeader(header.Key, header.Value);
            }

            SetHeaders();

            if (AddOptionsRequest != null)
                request = AddOptionsRequest(request);

            return request;
        }

        public void Send() 
        {
            WebRequest = CreateRequest();
            Run(SendRoutine(WebRequest));
        }

        public Coroutine SendCoroutine()
        {
            WebRequest = CreateRequest();
            return Run(SendRoutine(WebRequest));
        }

        private IEnumerator SendRoutine(UnityWebRequest request)
        {
            void ReviewToken(UnityWebRequest request)
            {
                if (request.responseCode != (int)HTTPResponseCodes.UNAUTHORIZED_401 &&
                    request.responseCode != (int)HTTPResponseCodes.FORBIDEN_403)
                {
                    callback(request);
                    return;
                }

                JWTResolver.RemoveAccessToken();

                if (Data.HaveAuth)
                {
                    JWTResolver.ResolveAccessToken(() => Send());
                    return;
                }

                callback(request);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (Data.HaveAuth)
                ReviewToken(request);
            else
                callback?.Invoke(request);
            request.Dispose();
        }
    }
}