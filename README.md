# FracturedCode.GitVersioning

Version with the commit hash and access said version in C#.

It's so simple, you could "install" this by adding the one csharp file and 
two dependencies to your project by hand.

This project implements [a source generator (`IIncrementalGenerator`)](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md) to 
provide the functionality.

## Installation and usage

Clone the repository and add it as a [project reference](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference) to any csproj you 
want to have it.

Example:
```Bash
git submodule add "<this-repo's-url>"
dotnet add reference FracturedCode.GitVersioning/GitVersioning.csproj
```

You could also use `Directory.Build.props` to add it to all 
projects. You could clone it somewhere else on the machine instead of a 
submodule. You could build it as a dll and reference that instead. The point 
is, it's flexible.

To use it:

```csharp
string version = ThisAssembly.Version;
```

## What problems is this solving?

**Problem:** For public projects or nuget packages, you must use semver because 
it is 
ingrained in our systems, tooling and culture. However, in some contexts, 
especially personal projects that are never published, you may find yourself 
desiring a 
simpler path that doesn't have to even be consciously thought about.

**Solution:** In this case, a git commit hash is a superb way to version your 
software (imo). The 
commit hash is unique, and you get it during the normal course of 
development. It comes "for free" in the sense that you don't have to change 
what you're doing (assuming you use git).

**Problem:** With that in mind, there is another common problem where a program 
wants to read/display its own version at runtime.

**Solution:** By adding this package to a csproj, you get an internal type 
from source generation that contains the "version" (commit hash). You may 
easily reference this anywhere in your code with `ThisAssembly.Version`.

## End result

You can now put an absolutely miniscule amount of thought into versioning 
and accessing that version at runtime. 
The commit hash is already there as part of development, and you don't even have
to manually change a file with the hash or populate an environment variable 
in a pipeline.

## Why not use Nerdbank.GitVersioning?

Doesn't `Nerdbank.GitVersioning` provide a similar type that includes the 
commit hash? Yes, however:

- It's opinionated.
- It's a comparatively much larger dependency.
- You have to put thought into it.
- You have at least two new files to source control.

It's great for a lot of scenarios, but for this package's niche it's 
a bit much.

## Irony

Yes, I know if I want to publish this as a package it has to have a semver. If 
you think this is an actual criticism, you may have missed the point.

## Resources
- https://github.com/amis92/csharp-source-generators?tab=readme-ov-file#metaprogramming
- https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
- https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.cookbook.md
- https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md

## Comparisons
- [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
- [GitVersion](https://gitversion.net/)
- [MinVer](https://github.com/adamralph/minver)
- [OctoVersion](https://github.com/OctopusDeploy/OctoVersion)

## License

[MIT License](./LICENSE)