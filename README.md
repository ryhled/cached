# ![Cached](https://github.com/ryhled/cached/blob/master/logo.png?raw=true)

![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/ryhled/79d73c90-2ec7-4406-b466-b14dd3a54f24/3/master)
![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ryhled/cached/3/master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ryhled/cached/3/master)
![GitHub issues](https://img.shields.io/github/issues/ryhled/cached)
![GitHub last commit](https://img.shields.io/github/last-commit/ryhled/cached)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/ryhled/cached/blob/master/LICENSE.md?label=license)
![GitHub tag (latest SemVer pre-release)](https://img.shields.io/github/v/tag/ryhled/cached?include_prereleases&label=latest%20release)

The light-weight, stampede safe, unobtrusive C# / .NET Core cache library. 

The aim is to be an 'as thin as possible' wrapper of the official providers, such as MemoryCache and DistributedCache (not reinventing the wheel). But at the same time provide ease of use and cache stampede safety / prevention amongst other things.

#### Features

- Uses the official cache providers under the hood (configuration is done the same way you are used to).
- Does lazy fetch / repopulation of data on cache miss.
- Cache stampede safe (in-process). Calls to populate cache are done only once.
- Selective write locking, only the key being repopulated is locked.
- Flexibility. For example: inject as service, or inject call directly as an 'ICached<>' instance.

## Getting started

To show the absolute most basic implementation, suitable for a console app for example, this is what is required:

1. Add the nuget package for InMemory caching.
```
nuget install Cached.InMemory
```

2. Create a default instance and fetch data.

```
var cacher = MemoryCacher.Default();
var value = await cacher.GetOrFetchAsync("key", key => service.GetData(key));
```

The 'service.GetData' calls is where you place your asynchronous service that fetches the actual data (may it be from database, REST/SOAP service or which ever).

### Adding cached to a net core application

Startup.cs

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddCached(options => options.AddInMemoryCaching());
}
```

Razor page

```
private readonly IMemoryCacher _cacher;

public IndexModel(IMemoryCacher cacher)
{
    _cached = cached;
}

public async Task OnGet()
{
    var value = await _cacher.GetOrFetchAsync("key", async key => { ... });
}
```

More examples and information can be found in the [Wiki](https://github.com/ryhled/cached/wiki) and in the [demo](https://github.com/ryhled/cached/tree/master/demo/) section.


## Future plans

* Add support for distributed caching (IDistributedCache) and optimal probabalistic stampede prevention.
* Add support for cache warmup.

## Owner(s)

- [ryhled](https://github.com/ryhled)

## Contributer(s)

- [ryhled](https://github.com/ryhled)

## Social

Please feel free to add constructive feedback, issues, bug reports and feature requests. Feedback is always appreciated!

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
