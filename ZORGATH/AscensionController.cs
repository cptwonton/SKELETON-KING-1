namespace ZORGATH;

[ApiController]
[Route("")]
public class AscensionController : ControllerBase
{
    [HttpGet("/", Name = "Ascension Root")]
    public IActionResult Root()
    {
        return Ok(@"{""error_code"":100,""data"":{""is_season_match"":true}}");
    }

    [HttpGet("index.php", Name = "Ascension Index")]
    public IActionResult Index()
    {
        return Root();
    }
}
