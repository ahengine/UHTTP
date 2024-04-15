using UnityEngine;

namespace UHTTP.Sample.OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField,TextArea] private string message;
        [SerializeField] private ChatBotConfig config;
        private ChatBotOpenAI chatBot;

        private void Awake() =>
            chatBot = new ChatBotOpenAI(config,result => Debug.Log("Chat Bot Intialize: "+result));


        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            chatBot.SendMessage(message,messages => {});

        [ContextMenu("Get Messages")]
        public void GetMessages() =>
            chatBot.GetMessages(message => {});
    }
}