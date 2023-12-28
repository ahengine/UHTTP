using UnityEngine;
using UHTTP;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HTTPRequestCard", menuName = "Cards/HTTPRequestCard", order = 0)]
public class HTTPRequestCard : ScriptableObject {

        public KeyValuePair<string, string>[] Headers
        {
                get
                {
                        KeyValuePair<string, string>[] keys = new KeyValuePair<string, string>[headers.Length];

                        for (int i = 0; i < headers.Length; i++)
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
        [SerializeField] private KeyValueItem[] headers;
        [field:SerializeField] public HTTPRequestMethod Method { private set; get; }
        [field:SerializeField] public string URL { private set; get; }
        [field:SerializeField] public bool HaveAuth { private set; get; }
        [field:SerializeField] public string BodyJson { private set; get; }
        [SerializeField] private List<KeyValueItem> postFields;

        [SerializeField] private List<KeyValueItem> postForm;

}

[System.Serializable]
public class KeyValueItem
{
    public string key;
    public string value;
}