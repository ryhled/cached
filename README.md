# ![Cached](https://github.com/ryhled/cached/blob/master/logo.png?raw=true)

![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/ryhled/79d73c90-2ec7-4406-b466-b14dd3a54f24/3/master)
![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ryhled/cached/3/master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ryhled/cached/3/master)
![GitHub issues](https://img.shields.io/github/issues/ryhled/cached)
![GitHub last commit](https://img.shields.io/github/last-commit/ryhled/cached)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/ryhled/cached/blob/master/LICENSE.md?label=license)
![GitHub tag (latest SemVer pre-release)](https://img.shields.io/github/v/tag/ryhled/cached?include_prereleases&label=latest%20release)

The light-weight, stampede safe, unobtrusive C# / .NET Core cache library. Aim is to be as thin as possible and to use underlying providers (instead of building own complex frameworks and reinventing the wheel).

#### Features

- Does automatic fetch / recaching on cache miss.
- Cache stampede prevention (in-process). Calls to populate cache are done only once.
- Selective locking, only the key being fetched is locked during cache repopulation.
- Flexibility. For example: inject as service, or inject call directly as an 'ICached<>' instance.

## Prerequisites

Cached provides multiple nuget packages. Below is what you need to use InMemory caching with asp.net core.

```

nuget install Cached.Net
nuget install Cached.InMemory

```
## Examples

### Minimal in-memory implementation

##### 1. Install nuget package
```
nuget install Cached.InMemory
```
##### 2. Add below code to your console application (for example)
```
var cacher = MemoryCacher.Default();
var value = await cacher.GetOrFetchAsync("cached_value", async () => { ... });
```

### In-memory provider used in Net Core application

##### 1. Add Cached support in Startup.cs.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddCached(options => options.AddInMemoryCaching());
}
```

##### 2. Inject and use cacher in your razor page.

```
private readonly IMemoryCacher _cacher;

public IndexModel(IMemoryCacher cacher)
{
    _cached = cached;
}

public async Task OnGet()
{
    var value = await _cacher.GetOrFetchAsync("key", async () => { ... });
}
```

More examples and information can be found in the [Wiki](https://github.com/ryhled/cached/wiki).

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
