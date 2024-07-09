namespace ZORGATH;

public class LogoutHandler3 : IClientRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        if (formData.TryGetValue("cookie", out var cookie))
        {
            if (cookie != "" && cookie != null)
            {
                using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                Account? account = await bountyContext.Accounts.Where(a => a.Cookie == cookie).FirstOrDefaultAsync();
                if (account != null)
                {
                    account.Cookie = null;
                    await bountyContext.SaveChangesAsync();
                }
            }
        }

        return new OkObjectResult("");
    }
}
