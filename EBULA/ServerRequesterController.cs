namespace EBULA;

using ZORGATH;
using PUZZLEBOX;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("server_requester.php")]
[Consumes("application/x-www-form-urlencoded")]

public class ServerRequesterController : ControllerBase
{
    private readonly IReadOnlyDictionary<string, IServerRequestHandler> _serverRequestHandlers;

    public ServerRequesterController(IReadOnlyDictionary<string, IServerRequestHandler> serverRequestHandlers)
    {
        _serverRequestHandlers = serverRequestHandlers;
    }

    [HttpPost(Name = "Server Requester")]
    public async Task<IActionResult> ServerRequester([FromForm] Dictionary<string, string> formData)
    {
        // Client requester has two forms for function identifiers. Some are
        // part of the query in the format `client_requester.php?f=`. Others
        // are specified as the `f` parameter in the `formData`.
        if (!formData.TryGetValue("f", out string? functionName))
        {
            var request = Request;
            if (request != null && request.Query.TryGetValue("f", out var value))
            {
                // API returns a collection in case the parameter is specified more than once.
                functionName = value[0];
            }
        }

        if (functionName == null)
        {
            // Unspecified request name.
            return BadRequest("Unknown request.");
        }

        if (_serverRequestHandlers.TryGetValue(functionName, out var requestHandler))
        {
            using PerformanceCounter performanceCounter = new PerformanceCounter(HttpContext.RequestServices);
            performanceCounter.Category = "ServerRequesterController";
            performanceCounter.Subcategory = functionName;

            return await requestHandler.HandleRequest(ControllerContext, formData);
        }

        // Unknown request name.
        Console.WriteLine("Unknown server request '{0}'.", functionName);
        return BadRequest(functionName);
    }
}
