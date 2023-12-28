using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.TodoModule
{
    [CreateAssetMenu(fileName = "Prodvider", menuName = "Providers/TodoProvider", order = 0)]
    public class TodoProvider : ScriptableObject
    {
        [SerializeField] private HTTPRequestCard GetAllTodos;
        public void GetAll(Action<UnityWebRequest> responseCallback) =>
            GetAllTodos.Send(responseCallback);

        [SerializeField] private HTTPRequestCard GetTodoById; 
        public void GetById(int id, Action<UnityWebRequest> responseCallback)
        {
            GetTodoById.SetAdditionalURL(id.ToString());
            Debug.Log(GetTodoById.URLFull);
            GetTodoById.Send(responseCallback);
        }
    }
}