namespace PUZZLEBOX;

/// <summary>
/// Configuration for the chat server component.
/// </summary>
public class ChatServerConfiguration
{
    /// <summary>
    /// The address that clients will use to connect to the chat server.
    /// </summary>
    public readonly string Address;
    
    /// <summary>
    /// The port used for client connections.
    /// </summary>
    public readonly short ClientPort;
    
    /// <summary>
    /// The port used for game server connections.
    /// </summary>
    public readonly short ServerPort;
    
    /// <summary>
    /// The port used for manager connections.
    /// </summary>
    public readonly short ManagerPort;

    /// <summary>
    /// Creates a new chat server configuration with the specified parameters.
    /// </summary>
    /// <param name="address">The address that clients will use to connect to the chat server.</param>
    /// <param name="clientPort">The port used for client connections.</param>
    /// <param name="serverPort">The port used for game server connections.</param>
    /// <param name="managerPort">The port used for manager connections.</param>
    public ChatServerConfiguration(string address, short clientPort, short serverPort, short managerPort)
    {
        Address = address;
        ClientPort = clientPort;
        ServerPort = serverPort;
        ManagerPort = managerPort;
    }
}
