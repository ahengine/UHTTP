[System.Serializable]
public record ChatBotConfig
{
    public string token;
    public string assistantId;
    public string instructionAssistant;
    public float delayGetMessage = 2;
}