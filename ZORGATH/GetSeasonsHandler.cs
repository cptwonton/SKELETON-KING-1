namespace ZORGATH;

public class GetSeasonsHandler : IClientRequestHandler
{
    private static Task<IActionResult> _result = Task.FromResult<IActionResult>(new OkObjectResult("a:3:{s:11:\"all_seasons\";s:53:\"0,0|0,1|1,0|2,0|3,0|4,0|5,0|6,0|7,0|8,0|9,0|10,0|11,0\";s:16:\"vested_threshold\";i:5;i:0;b:1;}"));

    public Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        return _result;
    }
}
