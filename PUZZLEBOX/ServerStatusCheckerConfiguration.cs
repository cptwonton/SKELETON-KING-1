namespace PUZZLEBOX;

/// <summary>
/// Configuration for the server status checker component.
/// </summary>
public class ServerStatusCheckerConfiguration
{
    /// <summary>
    /// The address that the server status checker will connect to.
    /// </summary>
    public readonly string Address;
    
    /// <summary>
    /// The port used for server status checker connections.
    /// </summary>
    public readonly int Port;

    /// <summary>
    /// Creates a new server status checker configuration with the specified parameters.
    /// </summary>
    /// <param name="address">The address that the server status checker will connect to.</param>
    /// <param name="port">The port used for server status checker connections.</param>
    public ServerStatusCheckerConfiguration(string address, int port)
    {
        Address = address;
        Port = port;
    }
}
