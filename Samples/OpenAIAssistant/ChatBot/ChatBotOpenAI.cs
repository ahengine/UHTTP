using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace UHTTP.Sample.OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAI
    {
        private ChatBotConfig config;
        private string threadId;

        // Config
        public ChatBotOpenAI(ChatBotConfig config,UnityAction<bool> initializeResult) 
        {
            this.config = config;
            OpenAIAssistantProvider.Initialize(this.config.token);
            CreateThread(initializeResult);
        }
        private void CreateThread(UnityAction<bool> initializeResult)
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
                AddAssistantToThread(initializeResult);
            }
        }
        private void AddAssistantToThread(UnityAction<bool> initializeResult)
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

                initializeResult?.Invoke(true);
            }
        }

        public void SendMessage(string message,UnityAction<string> result)
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

                CoroutineRunner.Run(()=> RunAssistant(RunAssistantComplete), config.delayGetMessage);

                void RunAssistantComplete() =>
                    CoroutineRunner.Run(()=> GetMessages(result), config.delayGetMessage);
            }
        }
        private void RunAssistant(Action completeAction)
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

                completeAction?.Invoke();


            }
        }
        public void GetMessages(UnityAction<string> result)
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

                result?.Invoke(request.downloadHandler.text);
            }
        }
    }
}