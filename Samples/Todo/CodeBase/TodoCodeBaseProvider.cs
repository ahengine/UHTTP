using System;
using UnityEngine.Networking;

namespace UHTTP.Sample.Todo
{
    public static class TodoCodeBaseProvider
    {
        private const string URL_Base = "https://jsonplaceholder.typicode.com/todos/";

        public static void GetALL(Action<UnityWebRequest.Result, string> callback) 
        {
            new HTTPRequestData()
            {
                URL = URL_Base,
                Method = UnityWebRequest.kHttpVerbGET
            }.CreateRequest(Callback).Send();

            void Callback(UnityWebRequest request) =>
                callback?.Invoke(request.result, request.downloadHandler.text);
        }

        public static void GetById(int id,Action<UnityWebRequest.Result,string> callback)
        {
            new HTTPRequestData()
            {
                URL = URL_Base + id,
                Method = UnityWebRequest.kHttpVerbGET,
            }.CreateRequest(Callback).Send();
            
            void Callback(UnityWebRequest request) =>
                callback?.Invoke(request.result, request.downloadHandler.text);
        }
    }
}