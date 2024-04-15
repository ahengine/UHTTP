using UnityEngine;
using UnityEngine.Networking;
using UHTTP;

namespace UHTTP_Sample.Todo
{
    public class TodoCodeBaseProviderRuntimeTest : MonoBehaviour
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
            TodoCodeBaseProvider.GetALL(Callback);

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
            TodoCodeBaseProvider.GetById(Id, Callback);

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
