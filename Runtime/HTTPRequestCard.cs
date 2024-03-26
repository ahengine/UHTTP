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
        private HTTPRequestParamsCard headers;
        [field: SerializeField] public string URL { private set; get; }
        [field: SerializeField] public HTTPRequestMethod Method { private set; get; }
        [field: SerializeField] public bool HaveAuth { private set; get; }

        // POST
        [field: SerializeField] public string BodyJson { private set; get; }
        [SerializeField] private HTTPRequestParamsCard postFields;
        [SerializeField] private HTTPRequestParamsCard postFormFields;

        public HTTPRequestData CreateRequestData()
        {
            var data = new HTTPRequestData();
            data.URL = URL;
            data.SetMethod(Method);
            data.HaveAuth = HaveAuth;

            // POST
            data.BodyJson = BodyJson;
            for (int i = 0; i < headers.Parameters.Count; i++)
                data.AddHeader(headers.Parameters[i].key, headers.Parameters[i].value);
            for (int i = 0; i < postFields.Parameters.Count; i++)
                data.PostFields.Add(postFields.Parameters[i].key, postFields.Parameters[i].value);
            for (int i = 0; i < postFields.Parameters.Count; i++)
                data.PostFormFields.Add(postFields.Parameters[i].key, postFields.Parameters[i].value);

            if (baseCard)
            {
                data.URL = baseCard.URL + URL;
                for (int i = 0; i < baseCard.headers.Parameters.Count; i++)
                    data.AddHeader(baseCard.headers.Parameters[i].key, baseCard.headers.Parameters[i].value);
                for (int i = 0; i < baseCard.postFields.Parameters.Count; i++)
                    data.PostFields.Add(baseCard.postFields.Parameters[i].key, baseCard.postFields.Parameters[i].value);
                for (int i = 0; i < baseCard.postFields.Parameters.Count; i++)
                    data.PostFormFields.Add(baseCard.postFields.Parameters[i].key, baseCard.postFields.Parameters[i].value);
            }

            return data;
        }

        public HTTPRequest CreateRequest() =>
                CreateRequestData().CreateRequest();

        public void Send(Action<UnityWebRequest> callback) =>
                CreateRequest().SetCallback(callback).Send();
    }
}