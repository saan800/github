# github

Reusable GitHub Actions workflow templates for .NET projects, with reference example implementations.

## Reusable Workflow Templates

All templates live in `.github/workflows/` and are prefixed with `_`. Call them from your own workflows using `workflow_call`.

### Build & Test

#### `_dotnet-build-and-test.yml`

Restores, builds, and runs tests with optional framework matrix and Codecov upload.

| Input | Required | Default | Description |
|---|---|---|---|
| `working-directory` | | `.` | Root of the .NET solution |
| `dotnet-version` | | `10.0.x` | SDK version (single build) |
| `dotnet-version-matrix` | | | JSON array of `{dotnet-version, framework}` pairs for matrix builds |
| `os` | | `ubuntu-latest` | Runner OS |
| `codecov-slug` | | | Repo slug for Codecov upload (skipped if empty) |
| `codecov-flag` | | `unittests` | Codecov flag |

| Secret | Required | Description |
|---|---|---|
| `CODECOV_TOKEN` | | Token for Codecov |

---

#### `_dotnet-build-and-pack.yml`

Packs NuGet `.nupkg` and `.snupkg` with version injection and uploads as a build artifact.

| Input | Required | Default | Description |
|---|---|---|---|
| `working-directory` | | `.` | Root of the .NET solution |
| `package-version` | Yes | | Semver string to inject (e.g. `1.2.3-beta.1`) |
| `package-artifact-name` | | `packages` | Name for the uploaded artifact |
| `add-assembly-version` | | `false` | Also set `AssemblyVersion` (use on main branch only) |
| `dotnet-version` | | `10.0.x` | SDK version |
| `os` | | `ubuntu-latest` | Runner OS |

---

### Versioning & Release

#### `_version.yml`

Determines the next semver from conventional commits using [`reecetech/version-increment`](https://github.com/reecetech/version-increment). Outputs `version` and `current-version`, and uploads a `version.txt` artifact.

| Input | Required | Default | Description |
|---|---|---|---|
| `is-release-branch` | Yes | | `true` when running on the release branch (e.g. main) |
| `version-artifact-name` | | `version` | Name for the uploaded artifact |

---

#### `_check-release-eligibility.yml`

Determines whether a release should be published by inspecting PR labels. A release is skipped only if every merged PR since the last tag carries the `no-release` label.

| Input | Required | Default | Description |
|---|---|---|---|
| `is-release-branch` | Yes | | `true` when running on the release branch |
| `force-release` | | `false` | Override all label checks and always release |

Outputs: `should-release` (`true` / `false`).

---

#### `_dotnet-publish-nuget.yml`

Downloads a packed artifact and publishes to GitHub Package Registry (prerelease builds) or NuGet.org (releases). Also cleans up old prerelease packages from GPR.

| Input | Required | Default | Description |
|---|---|---|---|
| `package-artifact-name` | | `packages` | Must match the artifact name from `_dotnet-build-and-pack.yml` |
| `upload-to-github` | | `false` | Publish to GitHub Package Registry |
| `upload-to-nuget` | | `false` | Publish to NuGet.org |
| `num-github-prerelease-packages-to-keep` | | `50` | How many old GPR prerelease versions to retain |
| `dotnet-version` | | `10.0.x` | SDK version |
| `os` | | `ubuntu-latest` | Runner OS |

| Secret | Required | Description |
|---|---|---|
| `NUGET_API_KEY` | | API key for NuGet.org. Required if `upload-to-nuget` is `true` |

---

#### `_github-tag-and-release.yml`

Creates an annotated git tag and a GitHub release with auto-generated release notes.

| Input | Required | Default | Description |
|---|---|---|---|
| `version` | Yes | | Version string without `v` prefix (e.g. `1.2.3`) |

---

### Full Orchestration

#### `_dotnet-build-test-pack-publish-nuget.yml`

Composes all of the above into a single end-to-end pipeline:

```
get-version ─┬─ check-release-eligibility
             └─ build-and-test ── pack ── publish ── release (if eligible)
```

| Input | Required | Default | Description |
|---|---|---|---|
| `working-directory` | | `.` | Root of the .NET solution |
| `is-release-branch` | Yes | | `true` when running on the release branch |
| `force-release` | | `false` | Force a release regardless of labels |
| `dotnet-version` | | `10.0.x` | SDK version for pack/publish jobs |
| `dotnet-version-matrix` | | | JSON matrix for the build-and-test job |
| `os` | | `ubuntu-latest` | Runner OS |
| `codecov-slug` | | | Codecov repo slug |
| `codecov-flag` | | `unittests` | Codecov flag |
| `num-github-prerelease-packages-to-keep` | | `50` | GPR cleanup retention count |

| Secret | Required | Description |
|---|---|---|
| `NUGET_API_KEY` | | API key for NuGet.org releases |
| `CODECOV_TOKEN` | | Token for Codecov |

**Quick-start example** — call from your own workflow:

```yaml
jobs:
  ci-cd:
    permissions:
      contents: write
      packages: write
      pull-requests: read
    uses: saan800/github/.github/workflows/_dotnet-build-test-pack-publish-nuget.yml@main
    with:
      working-directory: "./src/MyPackage"
      is-release-branch: ${{ github.ref == 'refs/heads/main' }}
    secrets:
      NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
```

---

### PR Automation

#### `_pr-labeler.yml`

Validates PR titles against [Conventional Commits](https://www.conventionalcommits.org/) and applies labels automatically. Creates any missing labels on first run.

#### `_pr-lint.yml`

Runs [super-linter](https://github.com/super-linter/super-linter) (C#, YAML, GitHub Actions) against changed files. Optionally runs [cspell](https://cspell.org/) spell-checking.

| Input | Required | Default | Description |
|---|---|---|---|
| `run-lint` | | `false` | Enable super-linter |
| `cspell-config` | | | Path to cspell config file. Spell-checking is skipped if omitted |

#### `_dependabot-auto-approve-and-merge.yml`

Auto-approves and squash-merges Dependabot PRs for non-major updates. Blocks on merge conflicts.

| Input | Required | Description |
|---|---|---|
| `pr-url` | Yes | URL of the Dependabot PR |

---

## Example Projects

| Project | Description |
|---|---|
| [`examples/MinimalWebApi`](examples/MinimalWebApi) | ASP.NET Core 10 controller-based API with unit and integration tests |
| [`examples/NugetPackages`](examples/NugetPackages) | Two packable class libraries showing the full NuGet publish pipeline |
