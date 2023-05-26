// <copyright file="ProviderService.cs" company="RandomPicture">
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

using Microsoft.Extensions.Options;
using RandomPicture.Models;
using RandomPicture.Options;
using RandomPicture.Providers;

namespace RandomPicture.Services;

public class ProviderService : IProviderService
{
    private List<ProviderOptions> _providerOptions;
    private readonly Dictionary<string, IProvider> _providers = new();

    public ProviderService(IOptionsMonitor<List<ProviderOptions>> providersMonitor)
    {
        _providerOptions = providersMonitor.CurrentValue;

        BuildProviders();

        providersMonitor.OnChange( opt =>
        {
            _providerOptions = opt;
            BuildProviders();
        });
    }

    private void BuildProviders()
    {
        foreach (var po in _providerOptions)
        {
            if (_providers.TryGetValue(po.Name, out var provider))
            {
                provider.Rebuild(po);
            }

            _providers.Add(po.Name, IProvider.GetInstance(po));

            var nonExistent = _providers.Keys.Except(_providerOptions.Select(x => x.Name));
            foreach (var ne in nonExistent)
            {
                _providers[ne].Dispose();
                _providers.Remove(ne);
            }
        }
    }

    public ProviderContentResult? GetContent(string providerName)
    {
        var hasProvider = _providers.TryGetValue(providerName, out var provider);
        var content = hasProvider ? provider!.GetContent() : null;
        return content is null
            ? null
            : new ProviderContentResult { Content = content, Type = provider!.Type };
    }
}
