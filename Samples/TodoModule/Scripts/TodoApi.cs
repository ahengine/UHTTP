using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace UHTTP.Sample.TodoModule
{
    public static class TodoApi
    {
        public static void GetById(HTTPRequestData data, int id, Action<RequestResult<Todo>> onGet)
        {
            data.URL += id.ToString();
            HTTPRequest request = data.CreateRequest((req) => onGet(GetData<Todo>(req)));
            request.Send();
        }

        public static void GetAll(HTTPRequestData data, Action<RequestResult<Todo[]>> onGet)
        {
            HTTPRequest request = data.CreateRequest((req) => onGet(GetData<Todo[]>(req)));
            request.Send();
        }

        public static async Task<RequestResult<Todo>> GetByIdAsync(HTTPRequestData data, int id)
        {
            data.URL += id.ToString();
            UnityWebRequest request = await data.CreateRequest(null).SendAsync();
            return GetData<Todo>(request);
        }

        public static async Task<RequestResult<Todo[]>> GetAllAsync(HTTPRequestData data)
        {
            UnityWebRequest request = await data.CreateRequest(null).SendAsync();
            return GetData<Todo[]>(request);
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
