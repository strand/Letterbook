﻿using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes.SubTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Controllers.ActivityPub;

/// <summary>
/// Provides the endpoints specified for Actors in the ActivityPub spec
/// https://www.w3.org/TR/activitypub/#actors
/// </summary>
[ApiController]
[Route("[controller]")]
public class ActorController
{
    private readonly SnakeCaseRouteTransformer _transformer = new();
    private readonly Uri _baseUri;

    public ActorController(IOptions<ConfigOptions> config)
    {
        _baseUri = new Uri($"{config.Value.Scheme}://{config.Value.HostName}");
    }


    [HttpGet]
    [Route("{id}")]
    public ActionResult<Actor> GetActor(int id)
    {
        var actor = new Actor
        {
            Inbox = CollectionUri(nameof(GetInbox), id.ToString()),
            Outbox = CollectionUri(nameof(GetOutbox), id.ToString()),
            Followers = CollectionUri(nameof(GetFollowers), id.ToString()),
            Following = CollectionUri(nameof(GetFollowing), id.ToString()),
            Endpoints = new Endpoints()
            {
                SharedInbox = CollectionUri(nameof(SharedInbox), id.ToString())
            }
        };
        return new OkObjectResult(actor);
    }

    [HttpGet]
    [ActionName("Followers")]
    [Route("{id}/collections/[action]")]
    public IActionResult GetFollowers(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ActionName("Following")]
    [Route("{id}/collections/[action]")]
    public IActionResult GetFollowing(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ActionName("Liked")]
    [Route("{id}/collections/[action]")]
    public IActionResult GetLiked(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ActionName("Inbox")]
    [Route("{id}/[action]")]
    public IActionResult GetInbox(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [ActionName("Inbox")]
    [Route("{id}/[action]")]
    public IActionResult PostInbox(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult SharedInbox()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ActionName("Outbox")]
    [Route("{id}/[action]")]
    public IActionResult GetOutbox(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [ActionName("Outbox")]
    [Route("{id}/[action]")]
    public IActionResult PostOutbox(int id)
    {
        throw new NotImplementedException();
    }

    private Uri CollectionUri(string actionName, string id)
    {
        var (action, routeTemplate) = ActionAttributes(actionName);
        var route = "/actor/" + routeTemplate
            .Replace("[action]", action)
            .Replace("{id}", id);
        var transformed = string.Join("/", route
            .Split("/")
            .Select(part => _transformer.TransformOutbound(part)));
        var result = new Uri(_baseUri, transformed);

        return result;
    }

    private static (string action, string route) ActionAttributes(string action)
    {
        var method = typeof(ActorController).GetMethod(action);

        var actionName = (method ?? throw new InvalidOperationException($"no method with name {action}"))
            .GetCustomAttribute<ActionNameAttribute>();
        var route = method.GetCustomAttribute<RouteAttribute>();
        if (route == null) throw new InvalidOperationException($"no route for action {action}");
        return (actionName?.Name ?? method.Name, route.Template);
    }
}