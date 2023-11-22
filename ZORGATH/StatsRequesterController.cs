namespace ZORGATH;

[ApiController]
[Route("stats_requester.php")]
[Consumes("application/x-www-form-urlencoded")]

public class StatsRequesterController : ControllerBase
{
    private readonly IReadOnlyDictionary<string, IStatsRequestHandler> _statsRequestHandlers;

    public StatsRequesterController(IReadOnlyDictionary<string, IStatsRequestHandler> statsRequestHandlers)
    {
        _statsRequestHandlers = statsRequestHandlers;
    }

    [HttpPost(Name = "Stats Requester")]
    public async Task<IActionResult> StatsRequester([FromForm] MatchStatsForSubmission stats)
    {
        string? functionName = stats.f;
        if (functionName == null)
        {
            // Unspecified request name.
            return BadRequest("Unknown request.");
        }

        if (_statsRequestHandlers.TryGetValue(functionName, out var requestHandler))
        {
            using BountyContext bountyContext = HttpContext.RequestServices.GetRequiredService<BountyContext>();
            return await requestHandler.HandleRequest(ControllerContext, bountyContext, stats);
        }

        // Unknown request name.
        Console.WriteLine("Unknown server request '{0}'.", functionName);
        return BadRequest(functionName);
    }
}
