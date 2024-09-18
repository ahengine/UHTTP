using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    public record HTTPRequestData
    {
        public bool HaveAuth;

        // URL
        public string URL;
        public void AppendUrl(string additionalUrl) => 
            URL = URL + additionalUrl;

        // METHOD
        public string Method;
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
                    throw new ArgumentOutOfRangeException(nameof(method), $"The method is not supported.");
            }
        }

        public List<KeyValuePair<string, string>> Headers = 
            new List<KeyValuePair<string, string>>();

        // --------------POST--------------

        public string BodyJson;
        public Dictionary<string, string> PostFields = 
            new Dictionary<string, string>();
        public Dictionary<string, string> PostFormFields = 
            new Dictionary<string, string>();

        public FormBinaryData FormBinaryData;

        public WWWForm Form()
        {
            if(PostFields.Count == 0 && FormBinaryData == null)
                return null;

            var form = new WWWForm();

            if(FormBinaryData != null)
                form = FormBinaryData.FormWithBinaryData();

            if(PostFields.Count > 0)
                foreach (var field in PostFields)
                    form.AddField(field.Key, field.Value);

            return form;
        }

        public HTTPRequest CreateRequest(Action<UnityWebRequest> callback) =>
            new HTTPRequest(this,callback);
    
        public void ToJson()
        {
            string json = JsonUtility.ToJson(this);
            Debug.Log(json);
        }
    }
}