using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace UHTTP.Sample.OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAI
    {
        private const string CHAT_BOT_ROLE = "assistant";

        private ChatBotConfig config;
        private string threadId;
        private bool logInEditor;

        // Config
        public ChatBotOpenAI(ChatBotConfig config, UnityAction threadCreated) =>
            SetConfig(config, threadCreated);

        public void SetConfig(ChatBotConfig config,UnityAction threadCreated) 
        {
            this.config = config;
            OpenAIAssistantProvider.Initialize(this.config.token);
            if(string.IsNullOrEmpty(threadId = config.threadId))
                CreateThread(threadCreated);
            logInEditor = config.logInEditor;
        }

        private void CreateThread(UnityAction threadCreated)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.CreateThread(Response);

            SendRequest();
            #if UNITY_EDITOR
            if(logInEditor) Debug.Log("Creating Thread ...");
            #endif
            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    if(logInEditor) Debug.Log("Create Thread Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }
                
                #if UNITY_EDITOR
                if(logInEditor) Debug.Log("Create Thread Successfully, \n"+request.downloadHandler.text);
                #endif
                CreateThread thread = JsonConvert.DeserializeObject<CreateThread>(request.downloadHandler.text);
                #if UNITY_EDITOR
                if(logInEditor) Debug.Log("Thread Successfully convert to entity" + thread.id);
                #endif
                threadId = thread.id;
                threadCreated?.Invoke();
            }
        }

        public void SendMessage(string message,UnityAction<Message> result)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.AddMessageToThread(threadId, new AddMessageToThreadDTO("user", message), Response);

            SendRequest();
            #if UNITY_EDITOR
            if(logInEditor) Debug.Log("Sending Message \n"+message);
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    if(logInEditor) Debug.Log("SendMessage Request Failed, trying agian .... \n"+ request.error);
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                if(logInEditor) Debug.Log("Send Message Successfully, \n"+request.downloadHandler.text);
                #endif

                CoroutineRunner.Run(()=> RunAssistant(RunAssistantComplete), config.delayGetMessage);

                void RunAssistantComplete() =>
                    CoroutineRunner.Run(()=> GetLastMessage(result), config.delayGetMessage);
            }
        }
        private void RunAssistant(Action completeAction)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.RunAssistantToThread(threadId,config.assistantId, Response);

            SendRequest();
            #if UNITY_EDITOR
            if(logInEditor) Debug.Log("Running Assistant");
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    if(logInEditor) Debug.Log("Run Assistant Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                if(logInEditor) Debug.Log("Run Assistant Successfully, \n"+request.downloadHandler.text);
#endif

                //CoroutineRunner.Run(() => completeAction?.Invoke(), config.delayGetMessage);
                completeAction?.Invoke();
            }
        }
        public void GetLastMessage(UnityAction<Message> result) =>
            GetMessages(messages => result?.Invoke(messages.Length > 0 ? messages[0] : null));
        public void GetMessages(UnityAction<Message[]> result)
        {
            void SendRequest() =>
                OpenAIAssistantProvider.GetMessagesThread(threadId, Response);

            SendRequest();
            #if UNITY_EDITOR
            if(logInEditor) Debug.Log("Getting Messages");
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    if(logInEditor) Debug.Log("GetMessages Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }

                #if UNITY_EDITOR
                if(logInEditor) Debug.Log("GetMessages Message Successfully, \n"+request.downloadHandler.text);
                #endif

                Messages messages = JsonConvert.DeserializeObject<Messages>(request.downloadHandler.text);
                var message = messages.messages[0];
                #if UNITY_EDITOR
                if(logInEditor) message.Print();
                #endif

                var resetRequest = message.Contents.Length == 0 || message.Role != CHAT_BOT_ROLE;

                if (resetRequest)
                {
                    CoroutineRunner.Run(SendRequest, config.delayGetMessage);
                    Debug.Log("Waiting for assistant message ...");
                    return;
                }

                result?.Invoke(messages.messages);
            }
        }
    }
}