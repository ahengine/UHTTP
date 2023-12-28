using UnityEngine;

namespace UHTTP.Helpers
{
    public class SampleTokenResolver
    {
        [RuntimeInitializeOnLoadMethod]
        private static void ExtractRefreshTokenResponseToSetAccessToken()
        {
            return;

            void SetAccessToken(string json) =>
                    JWTTokenResolver.SetAccessToken(json);

            HTTPRequest.SetRefreshTokenData(null,SetAccessToken);
        }
    }
}