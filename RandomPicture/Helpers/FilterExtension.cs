// <copyright file="FileterExtension.cs" company="RandomPicture">
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

using Minio.DataModel;
using RandomPicture.Providers;

namespace RandomPicture.Helpers;

public static class FilterExtension
{
    public static IEnumerable<string> ApplyFilter(this IEnumerable<string> source)
    {
        return source.Where(x => IProvider.SupportedTypes.Contains(x.Split('.').Last()));
    }
}
