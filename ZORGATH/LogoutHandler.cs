using Microsoft.EntityFrameworkCore;

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
                await bountyContext.Database.ExecuteSqlAsync($"UPDATE Accounts SET Cookie = NULL WHERE Cookie = {cookie}");
            }
        }

        return new OkObjectResult("");
    }
}
