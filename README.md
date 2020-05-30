# ![Cached](https://github.com/ryhled/cached/blob/master/logo.png?raw=true)

![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/ryhled/79d73c90-2ec7-4406-b466-b14dd3a54f24/3/master)
![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ryhled/cached/3/master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ryhled/cached/3/master)
![GitHub issues](https://img.shields.io/github/issues/ryhled/cached)
![GitHub last commit](https://img.shields.io/github/last-commit/ryhled/cached)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/ryhled/cached/blob/master/LICENSE.md)

The light-weight, stampede safe, unobtrusive C# / .NET Core cache library.

#### Features

- Does automatic fetch / recaching on cache miss.
- Prevents cache stampedes. Calls to populate cache are done only once.
- Selective locking, only the key being fetched is locked during cache repopulation.
- Flexibility. For example: inject as service, or inject call directly as an 'ICached<>' instance.

## Prerequisites

Cached provides multiple nuget packages. Below is what you need to use InMemory caching with asp.net core.

```

nuget install cached.net
nuget install cached.inmemory

```

## Examples

### In-Memory - Minimal

```
var cacher = MemoryCacher.New();
var value = await cacher.GetOrFetchAsync("cached_value", async () => { ... });
```

### In-Memory - Dependency Injection

#### Startup.cs

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddCached(options => options.AddInMemoryCaching());
}
```

#### RazorPage

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

## Future plans

- Add support for more cache providers.
- Add support for cache warmup.

## Owner(s)

- [ryhled](https://github.com/ryhled)

## Contributer(s)

- [ryhled](https://github.com/ryhled)

## Social

Please feel free to add constructive feedback, issues, bug reports and feature requests. Feedback is always appreciated!

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
