using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace UHTTP.Sample.TodoModule
{
    public static class TodoApi
    {
        public static void GetById(HTTPRequestData data, int id, Action<RequestResult<Todo>> onGet)
        {
            data.URL += id.ToString();
            HTTPRequest request = data.CreateRequest();
            request.SetCallback((webRequest) => onGet(GetData<Todo>(webRequest)));
            request.Send();
        }

        public static void GetAll(HTTPRequestData data, Action<RequestResult<Todo[]>> onGet)
        {
            HTTPRequest request = data.CreateRequest();
            request.SetCallback((webRequest) => onGet(GetData<Todo[]>(webRequest)));
            request.Send();
        }

        private static RequestResult<T> GetData<T>(UnityWebRequest request) where T : class
        {
            T data = request.result == UnityWebRequest.Result.Success
                ? JsonConvert.DeserializeObject<T>(request.downloadHandler.text)
                : null;

            return new RequestResult<T>(data, request);
        }
    }
}
