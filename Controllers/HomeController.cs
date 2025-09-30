using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      throw new Exception("We're done when I say we're done");
    }
  }
}
