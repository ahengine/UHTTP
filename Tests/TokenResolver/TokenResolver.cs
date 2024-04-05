using System;
using UnityEngine;
using static UHTTP.JWTTokenResolver;

namespace UHTTP.Helpers
{
    [CreateAssetMenu(fileName ="Token Resolver",menuName ="Cards/Token Resolver",order = 0)]
    public class TokenResolver : ScriptableObject
    {
        [SerializeField, TextArea] private string accessToken;
        [SerializeField, TextArea] private string refreshToken;

        [ContextMenu("Resolve Token")]
        private void ResolveToken()
        {
            SetAccessToken(accessToken);
            Debug.Log("Access Token Updated \n" + accessToken);
        }

        [ContextMenu("Resolve Refresh Token")]
        private void ResolveRefreshToken()
        {
            SetRefreshToken(refreshToken);
            Debug.Log("Refresh Token Updated \n" + refreshToken);
        }
    }
}