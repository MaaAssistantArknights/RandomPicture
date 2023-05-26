// <copyright file="BackgroundJobs.cs" company="RandomPicture">
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

using ILogger = Serilog.ILogger;

namespace RandomPicture.Helpers;

public class BackgroundJob
{
    private readonly Func<CancellationToken, ulong, Task> _func;
    private readonly TimeSpan _interval;
    private readonly bool _intervalBeforeFirstRun;

    private readonly CancellationTokenSource _ctx = new();
    private readonly ILogger _logger = Serilog.Log.ForContext<BackgroundJob>();

    private ulong _executions = 1;

    private Task? _task;

    public BackgroundJob(
        Func<CancellationToken, ulong, Task> func,
        TimeSpan interval,
        bool startImmediately = true,
        bool intervalBeforeFirstRun = false)
    {
        _func = func;
        _interval = interval;
        _intervalBeforeFirstRun = intervalBeforeFirstRun;

        if (startImmediately)
        {
            Start();
        }
    }

    public void Start()
    {
        if (_task is not null)
        {
            throw new TaskSchedulerException("Task is already running");
        }

        _task = Run(_ctx.Token);
    }

    public void Cancel()
    {
        _ctx.Cancel();
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                if (_intervalBeforeFirstRun && _executions == 1)
                {
                    await Task.Delay(_interval, cancellationToken);
                }

                try
                {
                    await _func.Invoke(cancellationToken, _executions);
                }
                catch (TaskCanceledException)
                {
                    // Ignore
                }
                catch (Exception e)
                {
                    _logger.Error("Exception in background job: {Exception}", e);
                }

                await Task.Delay(_interval, cancellationToken);

                _executions++;
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
        }
    }
}
