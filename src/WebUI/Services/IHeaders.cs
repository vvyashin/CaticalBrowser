namespace WebUI.Services
{
    public interface IHeaders
    {
        string UserAgent { get; }

        string Accept { get; }
    }
}
