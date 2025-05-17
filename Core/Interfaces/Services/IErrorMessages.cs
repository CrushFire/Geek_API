namespace Core.Interfaces.Services
{
    public interface IErrorMessages
    {
        string GetMessage(string key, string lang, params object[] args);
    }
}