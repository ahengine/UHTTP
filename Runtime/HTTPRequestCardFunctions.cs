using System;
using UnityEngine;
using UnityEngine.Networking;
using UHTTP;

public partial class HTTPRequestCard
{

        public HTTPRequest CreateRequest() =>
                new HTTPRequest(this);
        public HTTPRequest Send(Action<UnityWebRequest> callback) =>
                new HTTPRequest(this).SetCallback(callback).Send();

        public void SetURL(string URL) =>
                this.URL = URL;  

        public void SetAdditionalURL(string additionalURL) =>
                AdditionalURL = additionalURL;
        
        public void SetBodyJson(string json) =>
                BodyJson = json;

        public void AddHeader(KeyValueItem newHeader) =>
                headers.Add(newHeader);

        public void ClearHeaders() =>
                headers.Clear();
                
        public void AddPostField(KeyValueItem newPostField) =>
                postFields.Add(newPostField);

        public void ClearPostFields() =>
                postFields.Clear();
                
        public void AddPostFormField(KeyValueItem newPostFormField) =>
                postFormFields.Add(newPostFormField);

        public void ClearPostFormFields() =>
                postFormFields.Clear();
}
