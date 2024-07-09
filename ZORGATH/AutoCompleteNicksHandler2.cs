namespace ZORGATH;

public class AutoCompleteNicksHandler2 : IClientRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        string pattern = formData["nickname"];
        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        List<string> matchingAccountNames = await bountyContext.Accounts
            .Where(account => account.Name.StartsWith(pattern))
            .Select(account => account.Name)
            .OrderBy(accountName => accountName)
            .Take(40)
            .ToListAsync();
        return new OkObjectResult(matchingAccountNames.Any() ?
            PHP.Serialize(new AutoCompleteNicksResponse(matchingAccountNames)) :
            PHP.Serialize(new AccountLookupErrorResponse()));
    }
}
