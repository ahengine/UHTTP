using System.Collections.Generic;
using UnityEngine.Networking;

namespace UHTTP
{
    public class HTTPRequestData
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
        public List<KeyValuePair<string, string>> Headers { get; } = new List<KeyValuePair<string, string>>();

        public void AddHeader(string name, string value) => Headers.Add(new KeyValuePair<string, string>(name, value));

        // --------------POST--------------

        // BODY JSON
        public string BodyJson { get; set; }

        // POST FIELD
        public Dictionary<string, string> PostFields { get; } = new Dictionary<string, string>();

        // POST FORM FIELD
        public Dictionary<string, string> PostFormFields { get; } = new Dictionary<string, string>();

        public HTTPRequest CreateRequest() =>
            new HTTPRequest(this);
    }
}