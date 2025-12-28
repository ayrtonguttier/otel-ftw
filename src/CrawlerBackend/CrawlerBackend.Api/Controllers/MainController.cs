using Microsoft.AspNetCore.Mvc;

namespace CrawlerBackend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private Random _random = new Random();

    [HttpGet]
    public IActionResult GetMain()
    {
        int n = _random.Next(1, 101);
        if (n % 100 == 0)
        {
            return Problem("unlucky try again");
        }

        return Ok();
    }
}