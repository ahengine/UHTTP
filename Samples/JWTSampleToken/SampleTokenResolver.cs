using UnityEngine;

namespace UHTTP.Sample
{
    public class SampleTokenResolver
    {
        [RuntimeInitializeOnLoadMethod]
        private static void ExtractRefreshTokenResponseToSetAccessToken()
        {
            return;

            void SetAccessToken(string json) =>
                    JWTTokenResolver.SetAccessToken(json);

            JWTTokenResolver.SetRefreshTokenData(null,SetAccessToken);
        }
    }
}