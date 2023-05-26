// <copyright file="IProvider.cs" company="RandomPicture">
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

using RandomPicture.Options;

namespace RandomPicture.Providers;

public interface IProvider : IDisposable
{
    public static IEnumerable<string> SupportedTypes => new List<string>
    {
        "jpg",
        "jpeg",
        "png",
        "webp",
    };

    public string Name { get; }

    public string Type { get; }

    public static IProvider GetInstance(ProviderOptions options)
    {
        return options.Type switch
        {
            "s3" => S3Provider.GetInstance(options),
            "filesystem" => FilesystemProvider.GetInstance(options),
            _ => throw new NotSupportedException($"Provider type {options.Type} is not supported")
        };
    }

    public void Rebuild(ProviderOptions options);

    public string? GetContent();
}
