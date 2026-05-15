# Agent Memory

## Feedback

### NuGet .snupkg push to NuGet.org
When reviewing `dotnet nuget push` steps targeting nuget.org, do NOT flag the absence of a separate `*.snupkg` push step. NuGet.org automatically detects and ingests the `.snupkg` symbol package when the corresponding `.nupkg` is pushed.

### Dependabot NuGet PRs and the no-release label
NuGet Dependabot PRs in `dependabot.yml` intentionally omit the `no-release` label. NuGet dependency bumps are deliberately release-worthy so consumers get updated packages. Do NOT flag this as missing.

### ItemTests.ATest is an intentional placeholder
The test `ATest` in `examples/MinimalWebApi/tests/MinimalWebApi.Tests.Api/Schema/ItemTests.cs` is a deliberate minimal placeholder — the example project needs at least one unit test to exercise the CI pipeline. Do NOT flag it as trivial, low-quality, or missing real assertions.

### example-nuget-packages.yml is-release-branch: false is intentional
The hardcoded `is-release-branch: false` in `.github/workflows/example-nuget-packages.yml` is intentional. This example project must never auto-release to NuGet.org. Do NOT flag the commented-out expression or the hardcoded false as an issue.
