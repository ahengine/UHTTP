using System.Collections.Generic;
using UnityEngine.Networking;

namespace UHTTP
{
    public struct HTTPRequestData
    {
        // URL
        public string URL { get; set; }

        // METHOD
        public string Method { get; set; }

        public void SetMethod(HTTPRequestMethod method)
        {
            switch (method)
            {
                case HTTPRequestMethod.GET:
                    Method = UnityWebRequest.kHttpVerbGET;
                    break;
                case HTTPRequestMethod.POST:
                    Method = UnityWebRequest.kHttpVerbPOST;
                    break;
                case HTTPRequestMethod.PUT:
                    Method = UnityWebRequest.kHttpVerbPUT;
                    break;
                case HTTPRequestMethod.HEAD:
                    Method = UnityWebRequest.kHttpVerbHEAD;
                    break;
                case HTTPRequestMethod.CREATE:
                    Method = UnityWebRequest.kHttpVerbCREATE;
                    break;
                case HTTPRequestMethod.DELETE:
                    Method = UnityWebRequest.kHttpVerbDELETE;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(method), $"The method is not supported.");
            }
        }

        // AUTH
        public bool HaveAuth { get; set; }

        // HEADER
        private List<KeyValuePair<string, string>> m_Headers;
        public IReadOnlyList<KeyValuePair<string, string>> Headers => m_Headers;

        public void AddHeader(string name, string value) =>
            m_Headers.Add(new KeyValuePair<string, string>(name, value));

        public void ClearHeaders() =>
            m_Headers.Clear();


        // --------------POST--------------

        // BODY JSON
        public string BodyJson { get; set; }

        // POST FIELD
        private Dictionary<string, string> m_PostFields;
        public IReadOnlyDictionary<string, string> PostFields => m_PostFields;

        public void AddPostField(string key, string value) =>
            m_PostFields.Add(key,value);

        public void ClearPostFields() =>
            m_PostFields.Clear();

        // POST FORM FIELD
        private Dictionary<string, string> m_PostFormFields;
        public IReadOnlyDictionary<string, string> PostFormFields => m_PostFormFields;

        public void AddPostFormField(string key, string value) =>
            m_PostFormFields.Add(key,value);

        public void ClearPostFormFields() =>
            m_PostFormFields.Clear();

        public HTTPRequest CreateRequest() =>
            new HTTPRequest(this);
    }
}