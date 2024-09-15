using DashMq.DataAccess;
using DashMq.DataAccess.Model;
using Microsoft.AspNetCore.Mvc;

namespace DashMq.Web.Features.Datapoints;

public class DatapointsController(IDatapointRepository datapointRepository, IDatapointValueRepository valuesRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var datapoints = await datapointRepository.ListAsync(cancellationToken);
        return View(datapoints.Select(x => new DatapointModel() { Id = x.Id, Name = x.Name }).ToArray());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var datapoint = await datapointRepository.GetAsync(id, cancellationToken);
        if (datapoint == null)
            return RedirectToAction("ResourceNotFound", "Home");

        var values = await valuesRepository.ListAsync(id, new LimitOffset(100, 0), cancellationToken);
        var model = new DatapointModel
        {
            Id = datapoint.Id,
            Name = datapoint.Name,
            Values = values.Select(x => new DatapointValueModel { Value = x.Value, Timestamp = x.Timestamp }).ToList(),
        };
        return View(model);
    }
}