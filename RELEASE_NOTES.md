## v1.0.0

#### Packages

* Cached
* Cached.MemoryCache

#### New features

* Memory caching, wrapping Microsoft.Extensions.Caching.Memory.MemoryCache.
* Cache service injection. Inject Cached using ```ICache<IMemory>```.
* Cached functions. Wrap a service call and inject it as ```ICached<Response, Request>```.
* Stampede safe (using SemaphorSlim).
