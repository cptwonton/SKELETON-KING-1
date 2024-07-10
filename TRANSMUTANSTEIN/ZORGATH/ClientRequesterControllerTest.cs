namespace ZORGATH;

using EBULA;

[TestClass]
public class ClientRequesterControllerTest
{
    private class TestClientRequestHandler : IClientRequestHandler
    {
        private readonly IActionResult _result;
        public TestClientRequestHandler(IActionResult result)
        {
            _result = result;
        }

        public Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
        {
            return Task.FromResult(_result);
        }
    }

    [TestMethod]
    public async Task HandlerIsInvoked()
    {
        IActionResult expected = new OkResult();

        // Register our fake ClientRequesterHandler.
        Dictionary<string, IClientRequestHandler> handlers = new()
        {
            ["test"] = new TestClientRequestHandler(expected)
        };
        ClientRequesterController controller = new(handlers);
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        Dictionary<string, string> formData = new()
        {
            ["f"] = "test"
        };

        // Trigger our fake ClientRequesterHandler.
        var actual = await controller.ClientRequester(formData);
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public async Task UnknownRequest()
    {
        ClientRequesterController controller = new(new Dictionary<string, IClientRequestHandler>());
        Dictionary<string, string> formData = new()
        {
            ["f"] = "test2"
        };

        // Trigger an unknown request.
        var actual = await controller.ClientRequester(formData);
        Assert.IsInstanceOfType(actual, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UnspecifiedRequest()
    {
        ClientRequesterController controller = new(new Dictionary<string, IClientRequestHandler>());
        Dictionary<string, string> formData = new();

        // Trigger an unspecified request.
        var actual = await controller.ClientRequester(formData);
        Assert.IsInstanceOfType(actual, typeof(BadRequestObjectResult));
    }
}
