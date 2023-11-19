using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using JWTResolver = HTTPRequestService.JWTTokenResolver;
using static HTTPRequestService.HTTPRequestCoroutineRunner;
using System.Collections;

namespace HTTPRequestService
{
    public class HTTPRequest
    {
        private static Action<Action> TokenResolver;
        public static void SetTokenResolver(Action<Action> tokenResolver) =>
            TokenResolver = tokenResolver;

        public KeyValuePair<string, string>[] headers;
        public HTTPRequestMethod method;
        public string url;
        public bool haveAuth;
        public string bodyJson;
        public WWWForm postForm;
        public Dictionary<string, string> postFields;
        public Action<UnityWebRequest> callback;

        private UnityWebRequest CreateRequest()
        {
            UnityWebRequest Create() 
            {
                if (postForm != null)
                    return UnityWebRequest.Post(url, postForm);
                else if (postFields != null)
                    return UnityWebRequest.Post(url, postFields);
                else
                    return new UnityWebRequest()
                    {
                        method = method.ToString(),
                        url = url
                    };
            }

            UnityWebRequest request = Create();

            void AddBody()
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Add Body
            if (!string.IsNullOrEmpty(bodyJson))
                AddBody();

            void SetHeaders()
            {
                List<KeyValuePair<string, string>> totalHeaders = new List<KeyValuePair<string, string>>();

                if(headers != null)
                    totalHeaders.AddRange(headers);

                // Add Defaults
                totalHeaders.AddRange(new KeyValuePair<string, string>[]  {
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Accept", "application/json")
                });
                // Add JWT
                if (haveAuth && !string.IsNullOrEmpty(JWTResolver.AccessToken))
                    totalHeaders.Add(JWTResolver.AccessTokenHeader);
                
                // Set
                if (totalHeaders != null && totalHeaders.Count > 0)
                    foreach (var header in totalHeaders)
                        request.SetRequestHeader(header.Key, header.Value);
            }

            SetHeaders();
            return request;
        }   

        public void Send() =>
            Run(SendCoroutine());

        public IEnumerator SendCoroutine()
        {
            void ReviewToken(UnityWebRequest request)
            {
                if (request.responseCode != (int)HTTPResponseCodes.UNAUTHORIZED_401)
                {
                    callback(request);
                    return;
                }

                JWTResolver.RemoveAccessToken();

                if (TokenResolver != null)
                    TokenResolver(Send);
                else
                    callback(request);
            }

            var request = CreateRequest();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (haveAuth)
                ReviewToken(request);
            else 
                callback?.Invoke(request);
            request.Dispose();
        }
    }
}