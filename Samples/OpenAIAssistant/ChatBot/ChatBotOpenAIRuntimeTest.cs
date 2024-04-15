using UnityEngine;

namespace UHTTP.Sample.OpenAIAssistant
{
    public class ChatBotOpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField,TextArea] private string message;
        [SerializeField] private ChatBotConfig config;
        private ChatBotOpenAI chatBot;

        private void Awake() =>
            chatBot = new ChatBotOpenAI(config);


        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            chatBot.SendMessage(message);

        [ContextMenu("Get Messages")]
        public void GetMessages() =>
            chatBot.GetMessages();
    }
}