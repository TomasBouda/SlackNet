﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore;

class AspNetCoreServiceProviderSlackRequestListener : IServiceProviderSlackRequestListener
{
    private readonly IRequestServiceProviderAccessor _serviceProviderAccessor;
    private readonly IServiceProvider _serviceProvider;

    public AspNetCoreServiceProviderSlackRequestListener(IRequestServiceProviderAccessor serviceProviderAccessor, IServiceProvider serviceProvider)
    {
        _serviceProviderAccessor = serviceProviderAccessor;
        _serviceProvider = serviceProvider;
    }

    public void OnRequestBegin(SlackRequestContext context)
    {
        if (context.ContainsKey("Envelope")) // Socket mode
        {
            var scope = _serviceProvider.CreateScope();
            context.SetServiceProvider(scope.ServiceProvider);
            context.OnComplete(() =>
                {
                    scope.Dispose();
                    return Task.CompletedTask;
                });
        }
        else
        {
            context.SetServiceProvider(_serviceProviderAccessor.ServiceProvider);
        }
    }
}