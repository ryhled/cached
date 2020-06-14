# <img src="https://github.com/ryhled/cached/raw/master/logo.png?raw=true" alt="Cached" width="350" height="64">

[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/ryhled/79d73c90-2ec7-4406-b466-b14dd3a54f24/3/master?style=flat-square)](https://dev.azure.com/ryhled/Cached/_build?definitionId=3)
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ryhled/cached/3/master?style=flat-square)](https://dev.azure.com/ryhled/Cached/_build?definitionId=3)
[![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ryhled/cached/3/master?style=flat-square)](https://dev.azure.com/ryhled/Cached/_build?definitionId=3)
[![GitHub issues](https://img.shields.io/github/issues/ryhled/cached?style=flat-square)](https://github.com/ryhled/cached/issues)
[![GitHub last commit (branch)](https://img.shields.io/github/last-commit/ryhled/cached/master?style=flat-square)](https://github.com/ryhled/cached/commits/master)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/cached?color=informational&style=flat-square)](https://www.nuget.org/packages/Cached)


The light-weight, fast, concurrency-safe and unobtrusive cache library for Net Core / Net 5. 

#### What is Cached?

The goal of Cached is to be a thin, fast, wrapper around existing cache providers (like MemoryCache).
Cached wraps these providers and makes them concurrency-safe **with a minimal amount of locking**. It also provides flexibility and ease-of-use.

#### Why use Cached?

- Cached wraps your methods and caches it for you automatically, without risk of cache stampedes.
- It does not prevent, or hide, official cache provider configuration, standard methods still applies.
- Cached only applies locking as it is required to prevent stampedes (no global locks). 
- Cached offers flexibility and ease-of-use.
- Cached is thin. No need to create dependencies on huge caching frameworks.

## Getting started

Install Cached In-Memory nuget package.

```
nuget install Cached
nuget install Cached.MemoryCache
```

Then in your project, add the following in your net core  application:

*Startup.cs*

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddCached(options => options.AddMemoryCaching());
}
```

*Razor page*

```
private readonly ICache<IMemory> _cache;

public IndexModel(ICache<IMemory> cache)
{
    _cache = cache;
}

public async Task OnGet()
{
    var value = await _cache.GetOrFetchAsync("key", async key => { ... });
}
```

**More examples and information can be found in the [wiki](https://github.com/ryhled/cached/wiki) and in the [demo](https://github.com/ryhled/cached/tree/master/demo/) section**.


## Planned features

* [ ] Support for distributed caching (IDistributedCache).
* [ ]  Support for optimal probabalistic stampede prevention.
* [ ] Support for cache warmup.
* [ ] New optional key strategies for cache key prefixes.

## Social

Please feel free to add constructive feedback, issues, bug reports and feature requests. Feedback is always appreciated!

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
