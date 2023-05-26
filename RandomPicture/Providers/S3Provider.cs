// <copyright file="S3Provider.cs" company="RandomPicture">
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

using Minio;
using RandomPicture.Helpers;
using RandomPicture.Options;
using ILogger = Serilog.ILogger;

namespace RandomPicture.Providers;

public class S3Provider : IProvider
{
    private readonly ILogger _logger = Serilog.Log.ForContext<S3Provider>();

    private ProviderOptions _providerOptions = null!;
    private MinioClient? _minio;
    private BackgroundJob? _job;

    private List<string> _files = new();

    public string Name => _providerOptions.Name;
    public string Type => _providerOptions.Type;

    private string _baseUrl = string.Empty;

    private S3Provider(ProviderOptions options)
    {
        Build(options);
    }

    private void Build(ProviderOptions options)
    {
        if (options.Type != "s3")
        {
            throw new ArgumentException("ProviderOptions is not of type s3");
        }

        _providerOptions = options;
        _baseUrl = ((_providerOptions.S3Secure ?? false)
            ? "https://"
            : "http://")
              + _providerOptions.S3Endpoint + "/"
              + _providerOptions.S3BucketName + "/";

        _minio = new MinioClient()
            .WithEndpoint(_providerOptions.S3Endpoint)
            .WithCredentials(_providerOptions.S3AccessId, _providerOptions.S3AccessToken)
            .WithRegion(_providerOptions.S3Region)
            .WithSSL(_providerOptions.S3Secure ?? false)
            .Build();

        var exist = _minio
            .BucketExistsAsync(new BucketExistsArgs().WithBucket(_providerOptions.S3BucketName))
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        if (exist is false)
        {
            throw new ArgumentException("Bucket does not exist");
        }

        _job = new BackgroundJob( (token, i) =>
        {
            _logger.Information("Refreshing file list for S3 provider {Name}, Execution {Execution}", Name, i);
            var args = new ListObjectsArgs()
                .WithBucket(_providerOptions.S3BucketName)
                .WithPrefix(_providerOptions.Path)
                .WithRecursive(false);

            var observable = _minio.ListObjectsAsync(args);
            var _ = observable.Subscribe(
                item =>
                {
                    if (item.IsDir)
                    {
                        return;
                    }

                    var allow = IProvider.SupportedTypes.Contains(item.Key.Split('.').Last());
                    if (allow is false)
                    {
                        return;
                    }
                    _files.Add(_baseUrl + item.Key);
                },
                ex => _logger.Error($"Minio Subscription OnError: {ex}"),
                () => {});
            return Task.CompletedTask;
        }, TimeSpan.FromMinutes(5), startImmediately: true);
    }

    public static IProvider GetInstance(ProviderOptions options)
    {
        return new S3Provider(options);
    }

    public void Rebuild(ProviderOptions options)
    {
        Build(options);
    }

    public string? GetContent()
    {
        var content = _files.Count == 0 ? null : _files[Random.Shared.Next(0, _files.Count)];

        if (content is null || content.StartsWith("http") is false)
        {
            return null;
        }

        return content;
    }

    public void Dispose()
    {
        _job?.Cancel();
        _minio?.Dispose();
        GC.SuppressFinalize(this);
    }
}
