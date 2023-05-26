// <copyright file="RandomPictureController.cs" company="RandomPicture">
// RandomPicture - A part of the RandomPicture project
// Copyright (C) 2023 Alisa and Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>

using Microsoft.AspNetCore.Mvc;
using RandomPicture.Helpers;
using RandomPicture.Services;

namespace RandomPicture.Controllers;

[Route("/")]
public class RandomPictureController : ControllerBase
{
    private readonly IProviderService _providerService;

    public RandomPictureController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    public IActionResult GetRandomImage([FromQuery] string? provider)
    {
        if (provider is null)
        {
            return NotFound();
        }

        var content = _providerService.GetContent(provider);
        if (content is null)
        {
            return NotFound();
        }

        switch (content.Type)
        {
            case "s3":
                return Redirect(content.Content);
            case "filesystem":
                var mt = MediaTypeMap.Map(content.Content.Split(".").Last());
                var stream = System.IO.File.OpenRead(content.Content);
                return File(stream, mt);
        }

        return NotFound();
    }
}
