using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace UHTTP
{
    [CreateAssetMenu(fileName = "HTTPRequestCard", menuName = "Cards/HTTPRequestCard", order = 0)]
    public class HTTPRequestCard : ScriptableObject
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
            var data = new HTTPRequestData()
            {
                URL=URL,
                HaveAuth = HaveAuth,
            };

            data.SetMethod(Method);

            // POST
            data.BodyJson = BodyJson;

            if(headers)
                for (int i = 0; i < headers.Parameters.Count; i++)
                    data.Headers.Add(new KeyValuePair<string, string>(headers.Parameters[i].key, headers.Parameters[i].value));
            
            if(postFields)
                for (int i = 0; i < postFields.Parameters.Count; i++)
                    data.PostFields.Add(postFields.Parameters[i].key, postFields.Parameters[i].value);
            
            if(postFormFields)
                for (int i = 0; i < postFormFields.Parameters.Count; i++)
                    data.PostFormFields.Add(postFormFields.Parameters[i].key, postFormFields.Parameters[i].value);

            if (baseCard)
            {
                data.URL = baseCard.URL + URL;

                if(headers)
                    for (int i = 0; i < baseCard.headers.Parameters.Count; i++)
                        data.Headers.Add(new KeyValuePair<string, string>(baseCard.headers.Parameters[i].key, baseCard.headers.Parameters[i].value));
               
                if(postFields)
                    for (int i = 0; i < baseCard.postFields.Parameters.Count; i++)
                        data.PostFields.Add(baseCard.postFields.Parameters[i].key, baseCard.postFields.Parameters[i].value);
                
                if(postFormFields)
                    for (int i = 0; i < baseCard.postFormFields.Parameters.Count; i++)
                        data.PostFormFields.Add(baseCard.postFormFields.Parameters[i].key, baseCard.postFormFields.Parameters[i].value);
            }

            return data;
        }

        public HTTPRequest CreateRequest(Action<UnityWebRequest> callback) =>
                CreateRequestData().CreateRequest(callback);

        public void Send(Action<UnityWebRequest> callback) =>
                CreateRequest(callback).Send();
    }
}