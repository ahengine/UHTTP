using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace UHTTP.Sample.OpenAIAssistant
{
    public class ChatBotOpenAI
    {
        private ChatBotConfig config;
        private string threadId;

        // Config
        public ChatBotOpenAI(ChatBotConfig config) 
        {
            this.config = config;
            OpenAIAssistantProvider.Initialize(this.config.token);
            CreateThread();
        }
        private void CreateThread()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.CreateThread(Response);

            SendRequest();
            #if UNITY_EDITOR
            Debug.Log("Creating Thread ...");
            #endif
            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Create Thread Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }
                
                #if UNITY_EDITOR
                Debug.Log("Create Thread Successfully, \n"+request.downloadHandler.text);
                #endif
                CreateThread thread = JsonConvert.DeserializeObject<CreateThread>(request.downloadHandler.text);
                #if UNITY_EDITOR
                Debug.Log("Thread Successfully convert to entity" + thread.id);
                #endif
                threadId = thread.id;
                AddAssistantToThread();
            }
        }
        private void AddAssistantToThread()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.AddAssistantToThread(threadId,
                    new AddAssistantToThreadDTO(config.assistantId,config.instructionAssistant),Response);

            SendRequest();
            #if UNITY_EDITOR
            Debug.Log("Adding Assistant To Thread Thread ...");
            #endif
            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Add Assistant To Thread Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }
                
                #if UNITY_EDITOR
                Debug.Log("Create Thread Successfully, \n"+request.downloadHandler.text);
                #endif
            }
        }

        public void SendMessage(string message)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.AddMessageToThread(threadId, new AddMessageToThreadDTO("user", message), Response);

            SendRequest();
            #if UNITY_EDITOR
            Debug.Log("Sending Message \n"+message);
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("SendMessage Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                Debug.Log("Send Message Successfully, \n"+request.downloadHandler.text);
                #endif
                CoroutineRunner.Run(RunAssistant, config.delayGetMessage);
            }
        }
        private void RunAssistant()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.RunAssistantToThread(threadId,config.assistantId, Response);

            SendRequest();
            #if UNITY_EDITOR
            Debug.Log("Running Assistant");
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Run Assistant Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                Debug.Log("Run Assistant Successfully, \n"+request.downloadHandler.text);
                #endif
                CoroutineRunner.Run(GetMessages, config.delayGetMessage);
            }
        }
        public void GetMessages()
        {
            void SendRequest() =>
                OpenAIAssistantProvider.GetMessagesThread(threadId, Response);

            SendRequest();
            #if UNITY_EDITOR
            Debug.Log("Getting Messages");
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    Debug.Log("GetMessages Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                Debug.Log("GetMessages Message Successfully, \n"+request.downloadHandler.text);
                #endif
            }
        }
    }
}