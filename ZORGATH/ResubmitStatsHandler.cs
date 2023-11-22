namespace ZORGATH;

internal class PasswordSaltAndHashedPassword
{
    public readonly string PasswordSalt;
    public readonly string HashedPassword;

    public PasswordSaltAndHashedPassword(string passwordSalt, string hashedPassword)
    {
        PasswordSalt = passwordSalt;
        HashedPassword = hashedPassword;
    }
}

public class ResubmitStatsHandler : IStatsRequestHandler
{
    private readonly string _matchIdValidatorSalt = "s8c7xaduxAbRanaspUf3kadRachecrac9efeyupr8suwrewecrUphayeweqUmana";
    private readonly SubmitStatsHandler _submitStatsHandler;
    public ResubmitStatsHandler(SubmitStatsHandler submitStatsHandler)
    {
        _submitStatsHandler = submitStatsHandler;
    }

    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, BountyContext bountyContext, MatchStatsForSubmission stats)
    {
        string login = stats.login;
        if (login.Length < 1)
        {
            return new BadRequestResult();
        }
        login = login.Substring(0, login.Length - 1); // drop last character, which is expected to be ':'.

        PasswordSaltAndHashedPassword? passwordSaltAndHashedPassword = await bountyContext.Accounts.Where(account => account.Name == login)
            .Select(account => new PasswordSaltAndHashedPassword(account.User.PasswordSalt, account.User.HashedPassword))
            .FirstOrDefaultAsync();
        if (passwordSaltAndHashedPassword == null)
        {
            return new UnauthorizedResult();
        }

        string expectedResubmissionKey1 = Convert.ToHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(stats.match_stats.match_id + _matchIdValidatorSalt))).ToLower();
        string expectedResubmissionKey2 = string.Format("{0}_honfigurator", stats.match_stats.match_id);
        if (stats.resubmission_key != expectedResubmissionKey1 && stats.resubmission_key != expectedResubmissionKey2)
        {
            // invalid resubmissionKey.
            return new UnauthorizedResult();
        }

        if (RegistrationDataHelper.HashAccountPasswordMD5(stats.pass, passwordSaltAndHashedPassword.PasswordSalt) != passwordSaltAndHashedPassword.HashedPassword)
        {
            return new UnauthorizedResult();
        }

        return await _submitStatsHandler.HandleRequest(controllerContext, bountyContext, stats);
    }
}
