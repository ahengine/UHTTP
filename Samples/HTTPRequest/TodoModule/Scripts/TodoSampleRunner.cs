using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.TodoModule
{
    public class TodoSampleRunner : MonoBehaviour
    {
        [SerializeField] private int Id;
        [SerializeField] private HTTPRequestCard m_TodoById;
        [SerializeField] private HTTPRequestCard m_TodoAll;

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                ShowAllTodos();
            else if(Input.GetKeyDown(KeyCode.Alpha2))
                ShowTodo(Id);
        }

        private void ShowTodo(int id)
            => TodoApi.GetById(m_TodoById.CreateRequestData(), id, (response) => LogTodo(response, id));

        private void ShowAllTodos()
            => TodoApi.GetAll(m_TodoAll.CreateRequestData(), LogAllTodo);

        private void LogTodo(RequestResult<Todo> response, int id)
        {
            if (TryLogError(response.Request, $"Get Todo by id {id}"))
                return;

            Debug.Log(response.Data.ToString());
        }

        private void LogAllTodo(RequestResult<Todo[]> response)
        {
            if (TryLogError(response.Request, $"Get All Todos"))
                return;

            foreach(Todo todo in response.Data)
                Debug.Log(todo.ToString());
        }

        private bool TryLogError(UnityWebRequest request, string requestName)
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log($"{requestName} failed. Http code: {request.responseCode}.");
                    return true;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log($"{requestName} failed. Error: {request.error}");
                    return true;
            }

            return false;
        }
    }
}
