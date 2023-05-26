// <copyright file="FilesystemProvider.cs" company="RandomPicture">
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

using RandomPicture.Helpers;
using RandomPicture.Options;
using ILogger = Serilog.ILogger;

namespace RandomPicture.Providers;

public class FilesystemProvider : IProvider
{
    private readonly ILogger _logger = Serilog.Log.ForContext<FilesystemProvider>();

    private ProviderOptions _providerOptions = null!;
    private string[] _files = Array.Empty<string>();

    public string Name => _providerOptions.Name;
    public string Type => _providerOptions.Type;

    private FilesystemProvider(ProviderOptions options)
    {
        Build(options);
    }

    private void Build(ProviderOptions options)
    {
        if (options.Type != "filesystem")
        {
            throw new ArgumentException("Provider type is not filesystem");
        }

        _providerOptions = options;

        BuildFileList();

        var w = new FileSystemWatcher(_providerOptions.Path);
        w.Created += FileSystemWatcherEventHandler;
        w.Deleted += FileSystemWatcherEventHandler;
        w.Renamed += FileSystemWatcherEventHandler;
        w.EnableRaisingEvents = true;
    }

    private void FileSystemWatcherEventHandler(object sender, FileSystemEventArgs e)
    {
        _logger.Information("Filesystem provider {Name} has been updated, rebuilding file list, change type {ChangeType}", Name, e.ChangeType);
        BuildFileList();
    }

    private void BuildFileList()
    {
        var fs = Directory
            .GetFiles(_providerOptions.Path, "*", SearchOption.TopDirectoryOnly)
            .ApplyFilter();

        _files = fs.Select(Path.GetFullPath).ToArray();
        _logger.Information("Filesystem provider {Name} has {Count} files", Name, _files.Length);
    }

    public static IProvider GetInstance(ProviderOptions options)
    {
        return new FilesystemProvider(options);
    }

    public void Rebuild(ProviderOptions options)
    {
        Build(options);
    }

    public string? GetContent()
    {
        return _files.Length == 0 ? null : _files[Random.Shared.NextInt64(0, _files.Length)];
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
