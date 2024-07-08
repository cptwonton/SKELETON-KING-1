namespace ZORGATH;

public class LogoutHandler : IClientRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        if (formData.TryGetValue("cookie", out var cookie))
        {
            if (cookie != "" && cookie != null)
            {
                using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                await bountyContext.Accounts.ExecuteUpdateAsync(s => s.SetProperty(prop => prop.Cookie, value => null));
            }
        }

        return new OkObjectResult("");
    }
}
