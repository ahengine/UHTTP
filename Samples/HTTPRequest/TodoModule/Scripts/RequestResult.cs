using UnityEngine.Networking;

namespace UHTTP.Sample.TodoModule
{
    public struct RequestResult<T>
    {
        public T Data;
        public UnityWebRequest Request;

        public RequestResult(T data, UnityWebRequest request)
        {
            Data = data;
            Request = request;
        }
    }
}
