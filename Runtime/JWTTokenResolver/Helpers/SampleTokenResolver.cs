using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using ResponseCodes = HTTPRequestService.HTTPResponseCodes;

namespace HTTPRequestService.Helpers
{
    public class SampleTokenResolver
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Resolver()
        {
            return;

            HTTPRequest.SetTokenResolver(ResolveAccessToken);
        }
        private static void ResolveAccessToken(Action requestAction)
        {
            if (string.IsNullOrEmpty(JWTTokenResolver.RefreshToken))
            {
                requestAction?.Invoke();
                return;
            }

            void Resolve(UnityWebRequest request)
            {
                if (request.responseCode == (int)ResponseCodes.UNAUTHORIZED_401)
                    JWTTokenResolver.RemoveTokens();
                else
                {
                    // Your Get Access Token Action
                }

                requestAction?.Invoke();
            }

            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("Token", JWTTokenResolver.RefreshToken);
            var req = new HTTPRequest()
            {
                url="Your URL",
                method=HTTPRequestMethod.POST,
                postFields=body,
                callback = Resolve
            };
            req.Send();
        }
    }
}