using DashMq.DataAccess;
using DashMq.DataAccess.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.Client;

namespace DashMq.Web.Features.Datapoints;

[Authorize]
public class DatapointsController(IDatapointRepository datapointRepository, IDatapointValueRepository valuesRepository, IMqttClient mqttClient) : Controller
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
    public async Task<IActionResult> Details(int id, int? limit, int? offset, CancellationToken cancellationToken)
    {
        var datapoint = await datapointRepository.GetAsync(id, cancellationToken);
        if (datapoint == null)
            return RedirectToAction("ResourceNotFound", "Home");

        var values = await valuesRepository.ListAsync(id, new LimitOffset(limit ?? 20, offset ?? 0), cancellationToken);
        var model = new DatapointModel
        {
            Id = datapoint.Id,
            Name = datapoint.Name,
            Topic = datapoint.Topic,
            Direction = datapoint.Direction,
            Values = values.Select(x => new DatapointValueModel { Value = x.Value, Timestamp = x.Timestamp }).ToList(),
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Fetch(int id, int? limit, int? offset, CancellationToken cancellationToken)
    {
        var values = await valuesRepository.ListAsync(id, new LimitOffset(limit ?? 20, offset ?? 0), cancellationToken);
        return PartialView("Values", values);
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
            Topic = model.Topic,
            Direction = model.Direction,
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
            Id = datapoint.Id,
            Name = datapoint.Name,
            Topic = datapoint.Topic,
            Direction = datapoint.Direction,
            Values = []
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Datapoint model, CancellationToken cancellationToken)
    {
        var datapoint = await datapointRepository.GetAsync(model.Id, cancellationToken);
        if (datapoint == null)
            return RedirectToAction("ResourceNotFound", "Home");

        datapoint.Name = model.Name;
        datapoint.Topic = model.Topic;
        datapoint.Direction = model.Direction;

        await datapointRepository.SaveAsync(cancellationToken);

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

    [HttpPost]
    public async Task<IActionResult> Click(CancellationToken cancellationToken)
    {
        var payload = "{\"message\":\"this is the message\"}";
        await mqttClient.PublishStringAsync("switch", payload, cancellationToken: cancellationToken);
        return NoContent();
    }
}