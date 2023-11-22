namespace ZORGATH;

/// <summary>
///    Interface for a `stats_requester.php` function request handlers.
/// </summary>
public interface IStatsRequestHandler
{
    public Task<IActionResult> HandleRequest(ControllerContext controllerContext, BountyContext bountyContext, MatchStatsForSubmission stats);
}
