# Agent Memory

## Feedback

### NuGet .snupkg push to NuGet.org
When reviewing `dotnet nuget push` steps targeting nuget.org, do NOT flag the absence of a separate `*.snupkg` push step. NuGet.org automatically detects and ingests the `.snupkg` symbol package when the corresponding `.nupkg` is pushed.

### Dependabot NuGet PRs and the no-release label
NuGet Dependabot PRs in `dependabot.yml` intentionally omit the `no-release` label. NuGet dependency bumps are deliberately release-worthy so consumers get updated packages. Do NOT flag this as missing.

### ItemTests.ATest is an intentional placeholder
The test `ATest` in `examples/MinimalWebApi/tests/MinimalWebApi.Tests.Api/Schema/ItemTests.cs` is a deliberate minimal placeholder — the example project needs at least one unit test to exercise the CI pipeline. Do NOT flag it as trivial, low-quality, or missing real assertions.

### example-pr.yml is-release-branch: false is intentional
The hardcoded `is-release-branch: false` in `.github/workflows/example-pr.yml` is intentional. This example project must never auto-release to NuGet.org. Do NOT flag the commented-out expression or the hardcoded false as an issue.

### example-pr.yml workflow_dispatch is intentional
`workflow_dispatch` on `example-pr.yml` (and the commented-out `force-release` input) is by design — it exists for manual testing of the PR workflow. The example must never release to NuGet.org regardless. Do NOT flag `workflow_dispatch` on PR workflows or silently-ignored `force-release` as an issue in example workflows.

## Project Context

### Workflow secrets are caller-provided, not repo-owned
The secrets declared in `workflow_call.secrets:` blocks (e.g. `GITHUB_TOKEN`, `NUGET_API_KEY`, `CODECOV_TOKEN`) represent tokens that **calling repositories** provide when they consume these workflow templates — they are not secrets belonging to this repo. When analysing or fixing secrets-related issues, account for the fact that these workflows are reusable templates consumed by other GitHub projects, and the tokens are scoped to those caller repos.
