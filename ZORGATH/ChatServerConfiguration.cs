namespace ZORGATH;

public class ChatServerConfiguration
{
    public readonly string Address;
    public readonly short ClientPort;
    public readonly short ServerPort;
    public readonly short ManagerPort;
    public ChatServerConfiguration(string address, short clientPort, short serverPort, short managerPort)
    {
        Address = address;
        ClientPort = clientPort;
        ServerPort = serverPort;
        ManagerPort = managerPort;
    }
}
