using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP
{
    [CreateAssetMenu(fileName = "HTTPRequestCard", menuName = "Cards/HTTPRequestCard", order = 0)]
    public partial class HTTPRequestCard : ScriptableObject
    {
        [Tooltip("Add from Base: URL, Headers, PostFields, PostFormFields")]
        [SerializeField] private HTTPRequestCard baseCard;
        [SerializeField]
        private List<KeyValueItem> headers = new List<KeyValueItem>()
                {
                        new KeyValueItem() {key="Content-Type",value="application/json"},
                        new KeyValueItem() {key="Accept",value="application/json"}
                };
        [field: SerializeField] public string URL { private set; get; }
        [field: SerializeField] public HTTPRequestMethod Method { private set; get; }
        [field: SerializeField] public bool HaveAuth { private set; get; }

        // POST
        [field: SerializeField] public string BodyJson { private set; get; }
        [SerializeField] private List<KeyValueItem> postFields;
        [SerializeField] private List<KeyValueItem> postFormFields;

        public HTTPRequestData CreateRequestData()
        {
            var data = new HTTPRequestData();
            data.URL = URL;
            data.SetMethod(Method);
            data.HaveAuth = HaveAuth;

            // POST
            data.BodyJson = BodyJson;
            for (int i = 0; i < headers.Count; i++)
                data.AddHeader(headers[i].key, headers[i].value);
            for (int i = 0; i < postFields.Count; i++)
                data.PostFields.Add(postFields[i].key, postFields[i].value);
            for (int i = 0; i < postFields.Count; i++)
                data.PostFormFields.Add(postFields[i].key, postFields[i].value);

            if (baseCard)
            {
                data.URL = baseCard.URL + URL;
                for (int i = 0; i < baseCard.headers.Count; i++)
                    data.AddHeader(baseCard.headers[i].key, baseCard.headers[i].value);
                for (int i = 0; i < baseCard.postFields.Count; i++)
                    data.PostFields.Add(baseCard.postFields[i].key, baseCard.postFields[i].value);
                for (int i = 0; i < baseCard.postFields.Count; i++)
                    data.PostFormFields.Add(baseCard.postFields[i].key, baseCard.postFields[i].value);
            }

            return data;
        }

        public HTTPRequest CreateRequest() =>
                CreateRequestData().CreateRequest();

        public void Send(Action<UnityWebRequest> callback) =>
                CreateRequest().SetCallback(callback).Send();
    }

    [Serializable]
    public class KeyValueItem
    {
        public string key;
        public string value;
    }
}