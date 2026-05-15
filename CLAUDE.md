# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository purpose

A collection of reusable GitHub Actions workflow templates for .NET and node projects, plus reference example implementations (`examples/MinimalWebApi` and `examples/NugetPackages`).

## Commands

All commands run from within an example directory (e.g. `examples/MinimalWebApi`):

```bash
# Restore (lock file required)
dotnet restore --use-lock-file

# Build
dotnet build --no-restore --configuration Release

# Run all tests
dotnet test --no-build --configuration Release

# Run a single test project
dotnet test tests/MinimalWebApi.Tests.Api --no-build --configuration Release

# Run a single test by name
dotnet test tests/MinimalWebApi.Tests.Api --no-build --configuration Release --filter "FullyQualifiedName~ItemTests"

# Run with coverage (outputs to reports/coverage/)
dotnet test --no-build --configuration Release /p:CollectCoverage=true
```

## Architecture

### Workflow templates

Files in `.github/workflows/` prefixed with `_` are reusable workflows called via `workflow_call`. They are composed by the non-prefixed entry-point workflows:

| Template | Purpose |
|---|---|
| `_pr-labeler.yml` | Validates PR title (Conventional Commits), applies labels |
| `_pr-lint.yml` | Runs super-linter (C#, YAML, GitHub Actions) |
| `_dotnet-build-and-test.yml` | Build + test with optional matrix across frameworks |
| `_dotnet-build-and-pack.yml` | Pack NuGet `.nupkg`/`.snupkg` with version injection |
| `_version.yml` | Determines next semver from conventional commits |
| `_check-release-eligibility.yml` | Decides whether to publish a release |
| `_dotnet-publish-nuget.yml` | Publishes to GPR (prerelease) or nuget.org (release) |
| `_github-tag-and-release.yml` | Creates git tag and GitHub release with changelog |
| `_dotnet-build-test-pack-publish-nuget.yml` | Full orchestration: build → test → pack → publish |

### Example projects

Both examples share the same conventions:

- `src/` — application code
- `tests/*.Tests.*` — unit tests; `tests/*.Tests.Integration.*` — integration tests
- `Directory.Build.props` — shared MSBuild properties for the whole solution
- `Directory.Packages.props` — central package version management; add all `PackageReference` versions here, never in individual `.csproj` files

### Key build conventions

- **`TreatWarningsAsErrors=true`** — all warnings must be resolved
- **Lock files required**: `packages.lock.json` is committed and validated in CI; run `dotnet restore --use-lock-file` to update it after adding/changing packages
- **Test project detection**: projects with `.Tests.` anywhere in the name (or ending in `.Tests`) are auto-detected as test projects via `Directory.Build.props` and have coverage, test packages, and global usings applied automatically
- **Solution format**: `.slnx` (new format), not `.sln`
- **Package output**: `artifacts/packages/` at the solution root

### PR and commit conventions

PR titles and commit messages must follow [Conventional Commits](https://www.conventionalcommits.org/). Allowed types: `feat`, `fix`, `chore`, `ci`, `build`, `docs`, `perf`, `refactor`, `revert`, `style`, `test`, `dependencies`, `BREAKING CHANGE`. Scopes, when used, must match `(#\d{2,}|github-actions|nuget)`.

### Code style

- Indent: 2 spaces globally, 4 spaces for `.cs` files
- Line endings: LF
- Spelling: en-GB (dictionary overrides in `dictionary.dic`)
- Max line length: 250 (off for Markdown and YAML)

## Memory

Store all persistent memory in `.claude/agent-memory/MEMORY.md` in this repository. Do not use the default system memory path (`~/.claude/projects/*/memory/`). Use a flat structure with `##` headings per topic — no separate files per memory entry.
