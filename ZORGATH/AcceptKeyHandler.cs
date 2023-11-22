namespace ZORGATH;

public class AcceptKeyHandler : IServerRequestHandler
{
    public Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        // TODO: actually validate the key.
        Dictionary<string, object> response = new()
        {
            { "server_id", 0 }, // ignored but must be present.
            { "official", 1 }   // 0 = Unofficial; 1 = Official With Stats; 2 = Official Without Stats;
        };
        return Task.FromResult((IActionResult) new OkObjectResult(PHP.Serialize(response)));
    }
}
