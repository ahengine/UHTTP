namespace UHTTP.Sample.OpenAIAssistant.ChatBot
{
    [System.Serializable]
    public record ChatBotConfig
    {
        public string token;
        public string assistantId;
        public float delayGetMessage = 2;
    }
}