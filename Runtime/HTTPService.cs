using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator;
using System;
using UnityEngine;
using UnityEngine.Networking;

public enum HTTPRequestMethod { GET, POST, PUT }

namespace Networking
{
    public class HTTPService : MonoBehaviour
    {
        private static HTTPService instance;
        private static HTTPService Instance => instance ?? (instance = new GameObject(typeof(HTTPService).Name).AddComponent<HTTPService>());

        private void OnDestroy() =>
            instance = null;

        public static void SendData(HTTPRequestMethod method, string url, Action<UnityWebRequest> callback = null, KeyValuePair<string, string>[] headers = default, string bodyRaw = "") =>
            Instance.StartCoroutine(SendDataCoroutine(CreateRequest(method, url, bodyRaw), callback, headers));
        public static void SendData(string url, Action<UnityWebRequest> callback = null, KeyValuePair<string, string>[] headers = default, WWWForm postForm = null) =>
            Instance.StartCoroutine(SendDataCoroutine(CreatePostFormDataRequest(url, postForm), callback, headers));
        public static void SendData(string url, Action<UnityWebRequest> callback = null, KeyValuePair<string, string>[] headers = default, Dictionary<string, string> postFields = default) =>
            Instance.StartCoroutine(SendDataCoroutine(CreatePostFieldsDataRequest(url, postFields), callback, headers));

        public static UnityWebRequest CreateRequest(HTTPRequestMethod method, string url, string body)
        {
            UnityWebRequest request = new UnityWebRequest();
            request.method = method.ToString();
            request.url = url;

            // Add Body
            if (!string.IsNullOrEmpty(body))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            return request;
        }
        public static UnityWebRequest CreatePostFormDataRequest(string url, WWWForm form) =>
            UnityWebRequest.Post(url, form);
        public static UnityWebRequest CreatePostFieldsDataRequest(string url, Dictionary<string, string> fields) =>
            UnityWebRequest.Post(url, fields);

        private static IEnumerator SendDataCoroutine(UnityWebRequest request, Action<UnityWebRequest> callback, KeyValuePair<string, string>[] headers)
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            // Add Headers
            if (headers != null && headers.Length > 0)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

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