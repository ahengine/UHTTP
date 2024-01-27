using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    public static class JWTTokenResolver
    {
        public const string AUTHORIZATION_HEADER_KEY = "Authorization";
        private const string ACCESS_TOKEN_KEY = "JWT_ACCESS_TOKEN";
        private const string REFRESH_TOKEN_KEY = "JWT_REFRESH_TOKEN";

        private static HTTPRequestData RefreshTokenRequestData;
        private static Action<string> AccessTokenResolverFromRefreshResponse;

        public static void SetRefreshTokenData(string refreshToken, HTTPRequestData requestData,
            Action<string> accessTokenFromRefreshResponse) 
        {
            PlayerPrefs.SetString(REFRESH_TOKEN_KEY, refreshToken);
            if(!requestData.Headers.Contains(RefreshTokenHeader))
                requestData.AddHeader(RefreshTokenHeader.Key, RefreshTokenHeader.Value);
            RefreshTokenRequestData = requestData;
            AccessTokenResolverFromRefreshResponse = accessTokenFromRefreshResponse;
        }

        public static string AccessToken =>
            PlayerPrefs.GetString(ACCESS_TOKEN_KEY, "");
        public static string RefreshToken =>
            PlayerPrefs.GetString(REFRESH_TOKEN_KEY, "");

        public static KeyValuePair<string, string> AccessTokenHeader =>
            new KeyValuePair<string, string>(AUTHORIZATION_HEADER_KEY,"Bearer "+ AccessToken);
        
        public static KeyValuePair<string, string> RefreshTokenHeader =>
            new KeyValuePair<string, string>(AUTHORIZATION_HEADER_KEY, RefreshToken);

        public static void SetAccessToken(string token) =>
            PlayerPrefs.SetString(ACCESS_TOKEN_KEY, token);

        public static void RemoveTokens()
        {
            PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
            PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        }
        public static void RemoveAccessToken() =>
            PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);

        public static void ResolveAccessToken(Action requestCallback, Action resend)
        {
            // If no refresh token data was set, finish the request.
            if (string.IsNullOrEmpty(RefreshToken) || RefreshTokenRequestData == null ||
                AccessTokenResolverFromRefreshResponse == null)
            {
                requestCallback?.Invoke();
                return;
            }

            void Resolve(UnityWebRequest request)
            {
                if(request.result == UnityWebRequest.Result.Success)
                {
                    // Resolve access token
                    AccessTokenResolverFromRefreshResponse?.Invoke(request.downloadHandler.text);
                    // Retry with new access token
                    resend?.Invoke();
                    return;
                }

                if (request.responseCode == (int)HTTPResponseCodes.UNAUTHORIZED_401 ||
                    request.responseCode == (int)HTTPResponseCodes.FORBIDEN_403)
                    RemoveTokens();
                
                requestCallback?.Invoke();
            }

            var req = new HTTPRequest()
            {
                callback = Resolve
            };
            req.SetData(RefreshTokenRequestData);
            req.Send();
        }
    }   
}