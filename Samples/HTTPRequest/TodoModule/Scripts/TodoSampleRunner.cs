using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UHTTP.Sample.TodoModule
{
    public class TodoSampleRunner : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int Id;
        [SerializeField] private HTTPRequestCard m_TodoById;
        [SerializeField] private HTTPRequestCard m_TodoAll;

        [Header("UI")]
        [SerializeField] private Button m_GetById;
        [SerializeField] private Button m_GetAll;
        [SerializeField] private Button m_GetByIdAsync;
        [SerializeField] private Button m_GetAllAsync;

        private void OnEnable()
        {
            m_GetById.onClick.AddListener(ShowTodo);
            m_GetAll.onClick.AddListener(ShowAllTodos);
            m_GetByIdAsync.onClick.AddListener(ShowTodoAsync);
            m_GetAllAsync.onClick.AddListener(ShowAllTodosAsync);
        }

        private void OnDisable()
        {
            m_GetById.onClick.RemoveListener(ShowTodo);
            m_GetAll.onClick.RemoveListener(ShowAllTodos);
            m_GetByIdAsync.onClick.RemoveListener(ShowTodoAsync);
            m_GetAllAsync.onClick.RemoveListener(ShowAllTodosAsync);
        }

        private void ShowTodo()
            => TodoApi.GetById(m_TodoById.CreateRequestData(), Id, (response) => LogTodo(response, Id));

        private void ShowAllTodos()
            => TodoApi.GetAll(m_TodoAll.CreateRequestData(), LogAllTodo);

        private async void ShowTodoAsync()
        {
            RequestResult<Todo> response = await TodoApi.GetByIdAsync(m_TodoById.CreateRequestData(), Id);
            LogTodo(response, Id);
        }

        private async void ShowAllTodosAsync()
        {
            RequestResult<Todo[]> response = await TodoApi.GetAllAsync(m_TodoById.CreateRequestData());
            LogAllTodo(response);
        }

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
