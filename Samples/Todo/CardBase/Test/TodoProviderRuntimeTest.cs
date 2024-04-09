using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.Todo
{
    public class TodoProviderRuntimeTest : MonoBehaviour
    {
        [SerializeField] private int Id = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                All();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                GetById();
        }

        private void All()
        {
            TodoProvider.GetResource().GetALL(Callback);

            void Callback(UnityWebRequest.Result result, string value)
            {
                switch (result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log(value);
                        break;
                    default:
                        Debug.Log("GetById Failed: "+result);
                        break;
                }
            }
        }
        private void GetById()
        {
            TodoProvider.GetResource().GetById(Id, Callback);

            void Callback(UnityWebRequest.Result result, string value)
            {
                switch (result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log(value);
                        break;
                    default:
                        Debug.Log("GetById Failed: "+result);
                        break;
                }
            }
        }
    }
}