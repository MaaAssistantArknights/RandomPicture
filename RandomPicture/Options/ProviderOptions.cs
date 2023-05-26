// <copyright file="ProviderOptions.cs" company="RandomPicture">
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

namespace RandomPicture.Options;

public class ProviderOptions
{
    public string Name { get; set; } = "default";
    public string Type { get; set; } = "filesystem";
    public string Path { get; set; } = "/app/content";

    public string? S3AccessId { get; set; }
    public string? S3AccessToken { get; set; }
    public string? S3BucketName { get; set; }
    public string? S3Region { get; set; }
    public string? S3Endpoint { get; set; }
    public bool? S3Secure { get; set; }
}
