
namespace ZORGATH;

public class SetOnlineHandler : IServerRequestHandler
{
    public Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        // We currently don't need to handle this request.
        return Task.FromResult((IActionResult) new OkResult());
    }
}
