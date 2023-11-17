namespace PUZZLEBOX;

public class GameServer
{
    public GameServer(int gameServerId, int gameServerManagerId, DateTime timestampCreated, DateTime? timestampLastSession, string address, short port, string location, string name, string cookie)
    {
        GameServerId = gameServerId;
        GameServerManagerId = gameServerManagerId;
        TimestampCreated = timestampCreated;
        TimestampLastSession = timestampLastSession;
        Address = address;
        Port = port;
        Location = location;
        Name = name;
        Cookie = cookie;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int GameServerId { get; set; }

    public int GameServerManagerId { get; set; }

    [Required]
    public DateTime TimestampCreated { get; private set; }

    public DateTime? TimestampLastSession { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public short Port { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    // Deprecated
    public string HostName { get; set; } = "";

    [Required]
    // Deprecated
    public string HostPasswordHash { get; set; } = "";

    [Required]
    // Deprecated
    public string Verifier { get; set; } = "";

    [Required]
    public bool Official { get; set; } = true;

    [Required]
    public string? Cookie { get; set; }
}
