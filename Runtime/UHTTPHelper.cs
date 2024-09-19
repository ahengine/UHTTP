// UHTTPHelper is a helper class for UnityWebRequest [Writer: ahengine]

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    public enum HTTPResponseCodes
    {
        OK_200 = 200,
        AUTHORIZED_201 = 201,
        NOT_FOUND_404 = 404,
        SERVER_ERROR_500 = 500,
        UNAUTHORIZED_401 = 401,
        FORBIDEN_403 = 401,
    }   

    public static class HTTPHeaders
    {
        public const string HEADER_AUTHORIZATION_KEY = "Authorization";
        public const string HEADER_AUTHORIZATION_VALUE = "Bearer ";
        public const string HEADER_ACCEPT_KEY = "Accept";
        public const string HEADER_CONTENT_TYPE_KEY = "Content-Type";
        public const string HEADER_CONTENT_TYPE_VALUE = "application/json";
    }

    public static class JWTTokenResolver
    {
        private const string ACCESS_TOKEN_KEY = "JWT_ACCESS_TOKEN";
        private const string REFRESH_TOKEN_KEY = "JWT_REFRESH_TOKEN";

        public static Action<Action,Action> TokenExpiredSterategy { private set; get; }

        public static void SetTokenExpiredSterategy(Action<Action,Action> strategy) =>
            TokenExpiredSterategy = strategy;

        public static string AccessToken =>
            HTTPHeaders.HEADER_AUTHORIZATION_VALUE + PlayerPrefs.GetString(ACCESS_TOKEN_KEY, "");
        public static string RefreshToken =>
            HTTPHeaders.HEADER_AUTHORIZATION_VALUE + PlayerPrefs.GetString(REFRESH_TOKEN_KEY, "");

        public static void SetAccessToken(string token) =>
            PlayerPrefs.SetString(ACCESS_TOKEN_KEY, token);
        public static void SetRefreshToken(string token) =>
            PlayerPrefs.SetString(ACCESS_TOKEN_KEY, token);

        public static void RemoveTokens()
        {
            PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
            PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        }
        public static void RemoveAccessToken() =>
            PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);

        public static bool DoRenewToken(UnityWebRequest request,Action RequestAgain, Action<UnityWebRequest> callback)
        {
            bool CanRenewToken() 
            {
                bool FailedAuthorize = 
                    request.responseCode == (int)HTTPResponseCodes.UNAUTHORIZED_401 || request.responseCode == (int)HTTPResponseCodes.FORBIDEN_403;

                return FailedAuthorize && TokenExpiredSterategy != null;
            }

            if (!CanRenewToken())
                return false;

            RemoveAccessToken();
            TokenExpiredSterategy.Invoke(RequestAgain,()=> callback?.Invoke(request));
            return true;
        }
    }
}