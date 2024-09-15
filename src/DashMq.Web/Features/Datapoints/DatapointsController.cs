using Microsoft.AspNetCore.Mvc;

namespace DashMq.Web.Features.Datapoints;

public class DatapointsController : Controller
{
    private readonly IReadDatapointHandler readDatapointHandler;

    public DatapointsController(IReadDatapointHandler readDatapointHandler)
    {
        this.readDatapointHandler = readDatapointHandler;
    }
    
    [HttpGet]
    public IActionResult List()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var model = readDatapointHandler.Get(id);
        return View(model);
    }
}