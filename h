* [33mcommit cbadd2a9f4384e9b59bd0543f1f131d28dbcafff[m[33m ([m[1;36mHEAD -> [m[1;32mmaster[m[33m)[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 21:57:36 2020 +0200
[31m|[m 
[31m|[m     Added fallback to memorycache config & Removed disconnected load project.
[31m|[m     
[31m|[m     * Added support for using settings-free memorycaching,
[31m|[m       meaning relyiung on memorycache global config.
[31m|[m     * Removed old load project that was missed earlier.
[31m|[m 
* [33mcommit 9dd10e08f676bfe16fa87cb7ad4cc0b349f21dbd[m[33m ([m[1;31morigin/master[m[33m)[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 19:01:28 2020 +0200
[31m|[m 
[31m|[m     Added trigger for use in pipeline testing
[31m|[m 
* [33mcommit 493c7609cca6d3e127d69249e93818bac676bbd8[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 18:38:31 2020 +0200
[31m|[m 
[31m|[m     Made MemoryCache instance mandatory in static New extension.
[31m|[m     
[31m|[m     Likely user should be aware of this, disposable, dependency.
[31m|[m     Currently not interested in addid idisposable on cacher either.
[31m|[m 
* [33mcommit d9049845fd9edd23b293a272e604499745417a65[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 17:13:38 2020 +0200
[31m|[m 
[31m|[m     Added note that 0 releases has so far been made
[31m|[m 
* [33mcommit 39e5aa6859a98008278b62f12fa5aa5e5c588bfd[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 17:12:06 2020 +0200
[31m|[m 
[31m|[m     Pipeline work to build necessary artifacts for CD.
[31m|[m 
* [33mcommit a72355ed113a8a18f9506a7a804dd2fadc427515[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 13:16:49 2020 +0200
[31m|[m 
[31m|[m     Removed versioning from props file due to azure pipeline.
[31m|[m 
* [33mcommit 1bf09fe5d497a0b27b817889c1309ec093264579[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 13:16:04 2020 +0200
[31m|[m 
[31m|[m     Removed versioning from csproj files due to pipeline build
[31m|[m 
* [33mcommit c06e5df73612fb4bf2b27a8aa971d664d65aa169[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 02:15:21 2020 +0200
[31m|[m 
[31m|[m     Configured projects to create nuget packages (and symbol files) on build.
[31m|[m 
* [33mcommit 72e317b4bac543a84e9787706eef89229e081cb1[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 01:58:01 2020 +0200
[31m|[m 
[31m|[m     Fixed syntax error in pipeline file
[31m|[m 
* [33mcommit 99547f35aa16c1b471dc75ccc46625fb08688e0c[m
[31m|[m Author: Daniel Ryhle <daniel@ryhle.se>
[31m|[m Date:   Sun May 31 01:50:47 2020 +0200
[31m|[m 
[31m|[m     Added azure devops pipeline file.
[31m|[m     
[31m|[m     Contains CI and github release logic (no nuget publish yet).
