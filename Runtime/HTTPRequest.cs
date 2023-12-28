using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using JWTResolver = UHTTP.JWTTokenResolver;
using static UHTTP.HTTPRequestCoroutineRunner;
using System.Collections;

namespace UHTTP
{
    public class HTTPRequest
    {
        private static HTTPRequestCard RefreshTokenCard;
        private static Action<string> AccessTokenResolverFromRefreshResponse;
        public static void SetRefreshTokenData(HTTPRequestCard refreshTokenCard, Action<string> SetNewAccessToken) 
        {
            RefreshTokenCard = refreshTokenCard;
            AccessTokenResolverFromRefreshResponse = SetNewAccessToken;
        }

        public HTTPRequestCard Card { private set; get; }
        public Action<UnityWebRequest> callback;

        public HTTPRequest() { }
        public HTTPRequest(HTTPRequestCard card) =>
            Card = card;

        public void SetCard(HTTPRequestCard card) =>
            Card = card;

        private UnityWebRequest CreateRequest()
        {
            UnityWebRequest Create() 
            {
                if (Card.PostForm != null)
                    return UnityWebRequest.Post(Card.URL, Card.PostForm);
                else if (Card.PostFields != null)
                    return UnityWebRequest.Post(Card.URL, Card.PostFields);
                else
                    return new UnityWebRequest()
                    {
                        method = Card.Method.ToString(),
                        url = Card.URL
                    };
            }

            UnityWebRequest request = Create();

            void AddBody()
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(Card.BodyJson);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Add Body
            if (!string.IsNullOrEmpty(Card.BodyJson))
                AddBody();

            void SetHeaders()
            {
                List<KeyValuePair<string, string>> totalHeaders = new List<KeyValuePair<string, string>>();

                if(Card.Headers != null)
                    totalHeaders.AddRange(Card.Headers);

                // Add Defaults
                totalHeaders.AddRange(new KeyValuePair<string, string>[]  {
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Accept", "application/json")
                });
                // Add JWT
                if (Card.HaveAuth && !string.IsNullOrEmpty(JWTResolver.AccessToken))
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
                if (request.responseCode != (int)HTTPResponseCodes.UNAUTHORIZED_401 && 
                    request.responseCode != (int)HTTPResponseCodes.FORBIDEN_403)
                {
                    callback(request);
                    return;
                }

                JWTResolver.RemoveAccessToken();

                if (Card.HaveAuth)
                    ResolveAccessToken(Send);
                else
                    callback(request);
            }

            var request = CreateRequest();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (Card.HaveAuth)
                ReviewToken(request);
            else 
                callback?.Invoke(request);
            request.Dispose();
        }

        private static void ResolveAccessToken(Action requestAction)
        {
            if (string.IsNullOrEmpty(JWTResolver.RefreshToken))
            {
                requestAction?.Invoke();
                return;
            }

            void Resolve(UnityWebRequest request)
            {
                if (request.responseCode == (int)HTTPResponseCodes.UNAUTHORIZED_401 ||
                    request.responseCode == (int)HTTPResponseCodes.FORBIDEN_403)
                    JWTResolver.RemoveTokens();
                else
                    AccessTokenResolverFromRefreshResponse?.Invoke(request.downloadHandler.text);

                requestAction?.Invoke();
            }

            var req = new HTTPRequest()
            {
                callback = Resolve
            };
            req.SetCard(RefreshTokenCard);
            req.Send();
        }
    }
}