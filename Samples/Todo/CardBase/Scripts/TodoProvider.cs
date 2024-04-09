using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.Todo
{
    [CreateAssetMenu(fileName = "TodoProvider", menuName = "Providers/TodoProvider", order = 0)]
    public class TodoProvider : Provider<TodoProvider> 
    {
        [SerializeField] private HTTPRequestCard getAll;
        [SerializeField] private HTTPRequestCard getById;

        public void GetALL(Action<UnityWebRequest.Result, string> callback) =>
            getAll.Send(webReq => callback?.Invoke(webReq.result, webReq.downloadHandler.text));

        public void GetById(int id,Action<UnityWebRequest.Result,string> callback)
        {
            var req = getById.CreateRequestData();
            req.AppendUrl("" + id);
            req.CreateRequest(Callback).Send();

            void Callback(UnityWebRequest request) =>
                callback?.Invoke(request.result, request.downloadHandler.text);
        }
    }
}