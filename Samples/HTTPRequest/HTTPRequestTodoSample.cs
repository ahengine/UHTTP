using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample
{
    public class TodoProviderSample : MonoBehaviour
    {
        [SerializeField] private HTTPRequestCard GetAllTodos;

        public void GetAll()
        {
            void Response(UnityWebRequest result)
            {
                
            }

            GetAllTodos.Send(Response);
        }

        [SerializeField] private HTTPRequestCard GetTodoById;
        
        public void GetById(int id)
        {
            void Response(UnityWebRequest result)
            {

            }
            
            GetTodoById.SetAdditionalURL(id.ToString());
            GetTodoById.Send(Response);
        }
    }
}