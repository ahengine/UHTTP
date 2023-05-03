using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using RequestHandler = Networking.WebRequestHandler.WebRequestHandler;
using ResponseCodes = Networking.WebRequestHandler.HTTPResponseCodes;

namespace Networking.JWTTokenResolver.Helpers
{
    public class SampleTokenResolver
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Resolver()
        {
            return;

            RequestHandler.SetTokenResolver(ResolveAccessToken);
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

            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("Token", JWTTokenResolver.RefreshToken);
            HTTPService.SendData(HTTPRequestMethod.POST, "YourURL", Resolve, headers.ToArray(), JsonConvert.SerializeObject(body));
        }
    }
}