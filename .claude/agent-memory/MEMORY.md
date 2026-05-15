# Agent Memory

## Feedback

### NuGet .snupkg push to NuGet.org
When reviewing `dotnet nuget push` steps targeting nuget.org, do NOT flag the absence of a separate `*.snupkg` push step. NuGet.org automatically detects and ingests the `.snupkg` symbol package when the corresponding `.nupkg` is pushed.

### Dependabot NuGet PRs and the no-release label
NuGet Dependabot PRs in `dependabot.yml` intentionally omit the `no-release` label. NuGet dependency bumps are deliberately release-worthy so consumers get updated packages. Do NOT flag this as missing.
