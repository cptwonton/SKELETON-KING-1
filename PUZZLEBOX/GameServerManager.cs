namespace PUZZLEBOX;

public class GameServerManager
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int GameServerManagerId { get; set; }
    public string Cookie { get; set; }

    public GameServerManager(int gameServerManagerId, string cookie)
    {
        GameServerManagerId = gameServerManagerId;
        Cookie = cookie;
    }
}
