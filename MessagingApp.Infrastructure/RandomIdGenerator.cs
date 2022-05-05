namespace MessagingApp.Infrastructure;

public class RandomIdGenerator : IIdGenerator
{
    public string NewId()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd("+=".ToCharArray());
    }
}