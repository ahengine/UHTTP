using System;
using UnityEngine;

namespace UHTTP.Helpers
{
    [CreateAssetMenu(fileName ="Token Resolver",menuName ="Cards/Token Resolver",order = 0)]
    public class TokenResolver : ScriptableObject
    {
        [SerializeField, TextArea] private string accessToken;
        [SerializeField, TextArea] private string refreshToken;
        [SerializeField] private HTTPRequestCard refreshReqest;

        [ContextMenu("Resolve Token")]
        private void ResolveToken()
        {
            JWTTokenResolver.SetAccessToken(accessToken);
            Debug.Log("Access Token Updated \n" + accessToken);
        }

        [ContextMenu("Resolve Refresh Token")]
        private void ResolveRefreshToken()
        { 
            JWTTokenResolver.SetRefreshTokenData(refreshToken, refreshReqest.CreateRequestData(), SetAccessToken); 
            Debug.Log("Refresh Token Updated \n" + refreshToken);
        }

        private void SetAccessToken(string responseText) => JWTTokenResolver.SetAccessToken(responseText);
    }
}