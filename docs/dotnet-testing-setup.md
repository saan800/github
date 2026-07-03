# Setting up a .NET 10 solution for unit testing

This describes how `examples/MinimalWebApi` and `examples/NugetPackages` are wired for
testing, coverage, and CI, so you can replicate the pattern in a other solutions.

## 1. Commonly used testing packages

Both examples centralise the same core test packages via `Directory.Packages.props`
(see [§3](#3-directorybuildprops-directorypackagesprops-and-globaljson)):

| Package | Purpose |
|---|---|
| `AwesomeAssertions` | Fluent assertions library |
| `GitHubActionsTestLogger` | Emits test results as GitHub Actions log annotations/summary |
| `Microsoft.NET.Test.Sdk` | Base .NET test SDK |
| `Microsoft.Testing.Platform` | The modern "MTP" test engine (replaces classic VSTest) |
| `Microsoft.Testing.Extensions.CodeCoverage` | Native MTP code-coverage collector (powers `dotnet test --coverage`) |
| `xunit.v3.mtp-v2` | xUnit v3, run natively via MTP |
| `xunit.runner.visualstudio` | VSTest-compatible xUnit adapter (IDE test explorer support) |

**WebApi Test projects**

Additionally use `Microsoft.AspNetCore.Mvc.Testing` in its
integration test project, for spinning up the API in-memory (`WebApplicationFactory`).

## 2. Test project (`.csproj`) config

A unit test project needs almost no explicit configuration — everything else is
inherited from `Directory.Build.props` / `Directory.Packages.props`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MyProject\MyProject.csproj" />
  </ItemGroup>

</Project>
```

`<IsTestProject>true</IsTestProject>` is technically redundant if the project name
contains `.Tests.` or ends in `.Tests` (see §3 — `Directory.Build.props` detects this
automatically), but can set it explicitly for clarity.

An integration-test project that needs extra packages or config just adds them on top,
e.g. `examples/MinimalWebApi/tests/MinimalWebApi.Tests.Integration.Api`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MinimalWebApi.Api\MinimalWebApi.Api.csproj" />
  </ItemGroup>

</Project>
```

> **Don't** redeclare a `PackageReference` that's already added centrally in
> `Directory.Packages.props`'s test-project `ItemGroup` (§3) — e.g. adding
> `GitHubActionsTestLogger` again here — NuGet restore fails with `NU1504: Duplicate
> 'PackageReference' items found`, because `TreatWarningsAsErrors` is on.

## 3. Solution config files

### `./global.json`

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

This opts `dotnet test` into the modern Microsoft.Testing.Platform (MTP) engine for the
whole solution, instead of the legacy VSTest-based runner. It's required for the native
`dotnet test --coverage --coverage-output-format cobertura --results-directory ...` flags
used by `_dotnet-build-and-test.yml` on .NET 10 (see §5 for a related gotcha).


### `./Directory.Packages.props`

Central Package Management (CPM): every package version is declared once here, and
individual `.csproj` files reference packages **without** a version:

```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="AwesomeAssertions" Version="9.4.0" />
    <PackageVersion Include="GitHubActionsTestLogger" Version="3.0.4" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="18.7.0" />
    <PackageVersion Include="Microsoft.Testing.Extensions.CodeCoverage" Version="18.7.0" />
    <PackageVersion Include="Microsoft.Testing.Platform" Version="2.1.0" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageVersion Include="xunit.v3.mtp-v2" Version="3.2.2" />
  </ItemGroup>

  <ItemGroup Condition=" '$(IsTestProject)' == 'true'">
    <PackageReference Include="AwesomeAssertions" />
    <PackageReference Include="GitHubActionsTestLogger" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="Microsoft.Testing.Extensions.CodeCoverage" />
    <PackageReference Include="Microsoft.Testing.Platform" />
    <PackageReference Include="xunit.runner.visualstudio">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="xunit.v3.mtp-v2" />
  </ItemGroup>
</Project>
```

Because the conditioned `ItemGroup` applies to **every** project where
`IsTestProject == 'true'`, you never add these `PackageReference`s to individual test
`.csproj` files — that's what causes the `NU1504` duplicate error mentioned in §2.

After adding/changing a package version here, run `dotnet restore --use-lock-file` from
the solution folder and commit the updated `packages.lock.json` files — CI's "Verify ALL
lock files are up-to-date" step will fail the build otherwise.


### `Directory.Build.props`

Sets solution-wide defaults and auto-detects test projects by name:

```xml
<Project>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <PropertyGroup>
    <IsTestProject Condition="$(MSBuildProjectName.Contains('.Tests.'))">true</IsTestProject>
    <IsTestProject Condition="$(MSBuildProjectName.EndsWith('.Tests'))">true</IsTestProject>
    <IsTestProject Condition="'$(IsTestProject)' == ''">false</IsTestProject>
    <IsPackable Condition="'$(IsTestProject)' == 'true'">false</IsPackable>
    <IsPackable Condition="'$(IsPackable)' == ''">false</IsPackable>
  </PropertyGroup>

  <PropertyGroup Condition=" '$(IsTestProject)' == 'true'">
    <NoWarn>CS1591;CS8602;CS8625;CS8618</NoWarn>
  </PropertyGroup>

  <ItemGroup Condition=" '$(IsTestProject)' == 'true'">
    <Using Include="AwesomeAssertions" />
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
```

Key points:
- `RestorePackagesWithLockFile` + `ManagePackageVersionsCentrally` — every project restores
  from a committed `packages.lock.json`, with versions pinned centrally (see below).
- A project is automatically treated as a test project if its name contains `.Tests.`
  (e.g. `NugetPackages.Tests.Alpha`) or ends in `.Tests`. This also makes it non-packable
  and relaxes a handful of nullable/doc-comment warnings that are noisy in test code.
- Test projects get global `using AwesomeAssertions;` and `using Xunit;` for free — no
  need to repeat these usings in every test file.

## 4. Setting up Codecov

1. Sign in to [codecov.io](https://codecov.io) with your GitHub account and add the
   repository (or org) you want coverage for.
2. Copy the **Repository Upload Token** from the repo's Codecov settings page.
3. Add it as a GitHub Actions secret named `CODECOV_TOKEN` (see §5 — repo or org level).
4. No `codecov.yml` is required for the default setup used here (none of this repo's
   examples have one) — add one only if you want to configure coverage thresholds, PR
   comment behaviour, or ignore patterns. See the
   [Codecov YAML reference](https://docs.codecov.com/docs/codecov-yaml).
5. Pass `codecov-name` (and optionally `codecov-flag`) into `_dotnet-build-and-test.yml`
   (via `dotnet-pr.yml` / `dotnet-package-pr.yml`, see §5) — coverage upload is skipped
   entirely when `codecov-name` is empty.

### Known gotchas (already handled in `_dotnet-build-and-test.yml`, but good to know)

- **MTP coverage filenames aren't auto-discovered by Codecov.** `dotnet test --coverage`
  writes files named `<test-run-guid>.cobertura.xml`. Codecov's built-in file search only
  recognises a file named exactly `cobertura.xml`, or one whose name *contains* the
  substring `coverage`. The workflow renames each output file to
  `coverage.<guid>.cobertura.xml` after the test run to work around this — don't remove
  that step, and don't rely on `--coverage-output <fixed-name>` instead, since a fixed
  filename is unsafe when a solution has more than one test project (parallel test-host
  processes will race to write the same file).
- **Harden Runner needs `storage.googleapis.com` allow-listed.** The Codecov CLI fetches
  a signed upload URL from `ingest.codecov.io`, then uploads the actual report body
  directly to Google Cloud Storage (`storage.googleapis.com`). If that domain isn't in
  the job's `allowed-endpoints` list, the upload silently fails after several retries
  with `Request failed after too many retries`.

## 5. GitHub repository configuration

### Secrets

| Secret | Required when | Notes |
|---|---|---|
| `CODECOV_TOKEN` | `codecov-name` is set on `dotnet-pr.yml` / `dotnet-package-pr.yml` | From Codecov repo settings (§4) |

Add these under **Settings → Secrets and variables → Actions**, at the repo or org level.

### Parameters for GitHub workflow actions

Include `secrets: inherit`, to ensure that the build steps have access to the `CODECOV_TOKEN`. eg

```yaml
dotnet-web-api-pr:
  uses: saan800/github/.github/workflows/dotnet-pr.yml
  with:
    os: "ubuntu-latest"
    dotnet-version: "10.0.x"
    codecov-name: "my-project-name"
  secrets: inherit
```
