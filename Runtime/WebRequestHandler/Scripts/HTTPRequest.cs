using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using JWTResolver = HTTPRequestService.JWTTokenResolver;

namespace HTTPRequestService
{
    public class HTTPRequest
    {
        private static Action<Action> TokenResolver;
        public static void SetTokenResolver(Action<Action> tokenResolver) =>
            TokenResolver = tokenResolver;

        public List<KeyValuePair<string, string>> headers;

        public HTTPRequestMethod method;
        public string url;
        public bool haveAuth;

        public string bodyJson;
        public WWWForm postForm;
        public Dictionary<string, string> postFields;
        public Action<UnityWebRequest> callback;

        private UnityWebRequest CreateRequest()
        {
            UnityWebRequest request;

            if (postForm != null)
                request = UnityWebRequest.Post(url, postForm);
            else if (postFields != null)
                request = UnityWebRequest.Post(url, postFields);
            else
            {
                request = new UnityWebRequest()
                {
                    method = method.ToString(),
                    url = url
                };
            }

            // Add Body
            if (!string.IsNullOrEmpty(bodyJson))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Add Headers
            var totalHeaders = headers;
            // Add Default Headers
            totalHeaders.AddRange(new KeyValuePair<string, string>[]  {
                new KeyValuePair<string, string>("Content-Type", "application/json"),
                new KeyValuePair<string, string>("Accept", "application/json")
            });
            // Add JWT Header
            if (haveAuth && !string.IsNullOrEmpty(JWTResolver.AccessToken))
                totalHeaders.Add(JWTResolver.AccessTokenHeader);
            if (totalHeaders != null && totalHeaders.Count > 0)
                foreach (var header in totalHeaders)
                    request.SetRequestHeader(header.Key, header.Value);

            return request;
        }   
    
        public void Send() {

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

            UnityWebRequest webRequest = CreateRequest();
            HTTPService.SendData(webRequest, haveAuth ? ReviewToken : callback);
        }
    }
}