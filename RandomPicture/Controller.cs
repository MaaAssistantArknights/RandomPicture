// <copyright file="Controller.cs" company="RandomPicture">
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

namespace RandomPicture;

public class Controller : ControllerBase
{
    public IActionResult Test()
    {
        return Ok();
    }
}
