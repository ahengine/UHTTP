using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.OpenAIAssistant
{
    public class ChatBotOpenAI : MonoBehaviour
    {
        [SerializeField] private float getMessagesAfterSendMessageDelay = 2;
        [SerializeField,TextArea] private string message;

        [Header("Settings")]
        [SerializeField] private string token;
        [SerializeField] private string threadId;
        [SerializeField] private string assistantId;
        [SerializeField] private string instructionAssistant;

        private void Awake() 
        {
            OpenAIAssistantProvider.Initialize(token);
            CreateThread();
        }

        [ContextMenu("Create Thread")]
        public void CreateThread()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.CreateThread(Response);

            SendRequest();
            Debug.Log("Creating Thread ...");

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Create Thread Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }
                
                Debug.Log("Create Thread Successfully, \n"+request.downloadHandler.text);
                CreateThread thread = JsonConvert.DeserializeObject<CreateThread>(request.downloadHandler.text);
                Debug.Log("Thread Successfully convert to entity" + thread.id);
                threadId = thread.id;
                AddAssistantToThread();
            }
        }

        [ContextMenu("Add Assistant")]
        public void AddAssistantToThread()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.AddAssistantToThread(threadId,new AddAssistantToThreadDTO(assistantId,instructionAssistant),Response);

            SendRequest();
            Debug.Log("Adding Assistant To Thread Thread ...");

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Add Assistant To Thread Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }
                
                Debug.Log("Add Assistant To Thread Successfully, \n"+request.downloadHandler.text);
            }
        }

        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            SendMsg(message);

        public void SendMsg(string message)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.AddMessageToThread(threadId, new AddMessageToThreadDTO("user", message), Response);

            SendRequest();
            Debug.Log("Sending Message \n"+message);

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("SendMessage Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }

                Debug.Log("Send Message Successfully, \n"+request.downloadHandler.text);
                CoroutineRunner.Run(RunAssistant, getMessagesAfterSendMessageDelay);
            }
        }

        [ContextMenu("Run Assistant")]
        public void RunAssistant()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.RunAssistantToThread(threadId,assistantId, Response);

            SendRequest();
            Debug.Log("Running Assistant");

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Run Assistant Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }

                Debug.Log("Run Assistant Successfully, \n"+request.downloadHandler.text);
                CoroutineRunner.Run(GetMessages, getMessagesAfterSendMessageDelay);
            }
        }

        [ContextMenu("Get Messages")]
        public void GetMessages()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.GetMessagesThread(threadId, Response);

            SendRequest();
            Debug.Log("Getting Messages");

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("GetMessages Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }

                Debug.Log("GetMessages Message Successfully, \n"+request.downloadHandler.text);
            }
        }
    }
}