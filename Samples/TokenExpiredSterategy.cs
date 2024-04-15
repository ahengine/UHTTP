using System;
using UnityEngine;
using static UHTTP.JWTTokenResolver;

namespace UHTTP.Sample
{
    public static class TokenExpiredSterategy
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() =>
            SetTokenExpiredSterategy(Strategy);

        public static void Strategy(Action resendRequest,Action requestCallback)
        {

            // If you don't have refresh token, finish the request.
            if (string.IsNullOrEmpty(RefreshToken))
            {
                requestCallback?.Invoke();
                return;
            }

            SendRefreshToken();
        }

        private static void SendRefreshToken() 
        {
            
        }
    }
}
