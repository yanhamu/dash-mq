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
        return View(datapoints.Select(x => new DatapointModel()
        {
            Id = x.Id,
            Name = x.Name,
            Topic = x.Topic
        }).ToArray());
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
            Topic = datapoint.Topic,
            Values = values.Select(x => new DatapointValueModel { Value = x.Value, Timestamp = x.Timestamp }).ToList(),
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult New()
    {
        return View(new DatapointModel());
    }

    [HttpPost]
    public async Task<IActionResult> New(DatapointModel model, CancellationToken cancellationToken)
    {
        datapointRepository.Add(new Datapoint()
        {
            Name = model.Name,
            Topic = model.Topic
        });
        await datapointRepository.SaveAsync(cancellationToken);
        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var datapoint = await datapointRepository.GetAsync(id, cancellationToken);
        if (datapoint == null)
            return RedirectToAction("ResourceNotFound", "Home");

        return View(new DatapointModel
        {
            Topic = datapoint.Topic,
            Name = datapoint.Name,
            Id = datapoint.Id,
            Values = []
        });
    }

    [HttpPost]
    public IActionResult Edit(Datapoint model)
    {
        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var datapoint = await datapointRepository.GetAsync(id, cancellationToken);
        if (datapoint == null)
            return RedirectToAction("ResourceNotFound", "Home");

        datapointRepository.Remove(datapoint);
        await datapointRepository.SaveAsync(cancellationToken);
        return RedirectToAction("List");
    }
}