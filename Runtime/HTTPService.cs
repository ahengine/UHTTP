using IEnumerator = System.Collections.IEnumerator;
using System;
using UnityEngine;
using UnityEngine.Networking;

public enum HTTPRequestMethod { GET, POST, PUT }

namespace HTTPRequestService
{
    public class HTTPService : MonoBehaviour
    {
        private static HTTPService instance;
        private static HTTPService Instance => instance ?? (instance = new GameObject(typeof(HTTPService).Name).AddComponent<HTTPService>());

        private void OnDestroy() =>
            instance = null;

        public static void SendData(UnityWebRequest request, Action<UnityWebRequest> callback) =>
            Instance.StartCoroutine(SendDataCoroutine(request, callback));

        private static IEnumerator SendDataCoroutine(UnityWebRequest request, Action<UnityWebRequest> callback)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            callback?.Invoke(request);
            request.Dispose();
        }

        class AcceptAllCertificates : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
    }
}