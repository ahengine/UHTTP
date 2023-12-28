using System;
using System.Collections.Generic;
using UnityEngine;
using UHTTP;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "HTTPRequestCard", menuName = "Cards/HTTPRequestCard", order = 0)]
public partial class HTTPRequestCard : ScriptableObject 
{
        public KeyValuePair<string, string>[] Headers
        {
                get
                {
                        KeyValuePair<string, string>[] keys = new KeyValuePair<string, string>[headers.Count];

                        for (int i = 0; i < headers.Count; i++)
                                keys[i] = new KeyValuePair<string, string>(headers[i].key, headers[i].value);

                        return keys;
                }
        }
        public Dictionary<string, string> PostFields {
                get {
                        Dictionary<string, string> fields = new Dictionary<string, string>();

                        for(int i=0; i<postFields.Count; i++)
                                fields.Add(postFields[i].key, postFields[i].value);

                        return fields;
                }
        }
        public WWWForm PostForm {
                get {
                        WWWForm form = new WWWForm();

                        for(int i=0; i<postFields.Count; i++)
                                form.AddField(postFields[i].key, postFields[i].value);

                        return form;
                }
        }
        public string URLFull => URL + AdditionalURL;

        [SerializeField] private List<KeyValueItem> headers;
        [field:SerializeField] public string URL { private set; get; }
        public string AdditionalURL { private set; get; }
        [field:SerializeField] public HTTPRequestMethod Method { private set; get; }
        public string MethodStr
        {
                get
                {
                        switch (Method)
                        {
                                case HTTPRequestMethod.GET:
                                        return UnityWebRequest.kHttpVerbGET;
                                case HTTPRequestMethod.POST:
                                        return UnityWebRequest.kHttpVerbPOST;
                                case HTTPRequestMethod.PUT:
                                        return UnityWebRequest.kHttpVerbPUT;
                        }

                        return "";
                }
        }

        [field:SerializeField] public bool HaveAuth { private set; get; }

        // POST
        [field:SerializeField] public string BodyJson { private set; get; }
        [SerializeField] private List<KeyValueItem> postFields;
        [SerializeField] private List<KeyValueItem> postFormFields;
}

[Serializable]
public class KeyValueItem
{
    public string key;
    public string value;
}