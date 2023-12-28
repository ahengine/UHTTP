using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace UHTTP.Sample.TodoModule
{
    public class TodoController : MonoBehaviour
    {
        private static TodoController instance;
        public static TodoController Instance => instance ?? CreateInstance();
        private TodoProvider provider;

        private static TodoController CreateInstance() 
        {
            instance = new GameObject(typeof(TodoController).Name).AddComponent<TodoController>();
            instance.provider = Resources.Load<TodoProvider>(typeof(TodoProvider).Name);
            return instance;
        }

        public void ShowAll()
        {
            provider.GetAll(Callback);
            void Callback(UnityWebRequest response)
            {
                switch (response.result)
                {
                    case UnityWebRequest.Result.Success:
                        ApplyShowAll(response.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        print("Get All Todos: Failed to fetch \n" + response.error);
                        break;
                }
            }
        }

        private void ApplyShowAll(string json)
        {
            var todos = JsonConvert.DeserializeObject<List<Todo>>(json);
            for (int i = 0; i < todos.Count; i++)
                Debug.Log(todos[i].ToString());
        }

        public void ShowById(int id)
        {
            provider.GetById(id, Callback);
            void Callback(UnityWebRequest response)
            {
                switch (response.result)
                {
                    case UnityWebRequest.Result.Success:
                        ApplyShowById(response.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        print("Get Todo by id " + id + " : Failed to fetch \n" + response.error);
                        break;
                }
            }
        }

        private void ApplyShowById(string json)
        {
            var todo = JsonConvert.DeserializeObject<Todo>(json);
            Debug.Log(todo.ToString());
        }
    }
}