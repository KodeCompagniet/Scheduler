# Release Management and NuGet Publishing Workflow

This document explains how to cut a new release of DemiCode Scheduler and how the GitHub Actions workflow builds, tests, packs, and publishes NuGet packages.

## Packages
- `DemiCode.Shared`
- `DemiCode.Scheduling`
- `DemiCode.Logging.log4net`

`DemiCode.Logging` was removed in v2.0.0; the logging namespace now ships from `DemiCode.Shared`.

## Versioning
- Source of truth: `Directory.Build.props` `<Version>` (for example, `2.0.0`).
- Assembly version: `AssemblyInfo.Version.cs` must match (`2.0.0.0`).
- All packages ship with the same version.

Checklist before release:
- Update `CHANGELOG.md` for the new version.
- Ensure `Directory.Build.props` `<PackageReleaseNotes>` reflects the same changes (or remove/update for future releases as needed).
- Build and test locally or via CI.

## CI Workflow Overview
File: `.github/workflows/nuget-publish.yml`

Triggers:
- `push` of tags matching `v*` → builds, tests, packs, publishes to NuGet, and creates a GitHub Release.
- `workflow_dispatch` (manual) → builds, tests, packs, uploads artifacts. Set `publish=true` to publish from a manual run.

Runners and SDK:
- `windows-latest` runner (required to pass Windows-specific tests).
- .NET SDK `8.0.120` (pinned by `global.json`).

Secrets and permissions:
- NuGet API key: `KODECOMPAGNIET_SCHEDULER_NUGET_APIKEY` (repository Actions secret).
- Workflow sets `permissions: contents: write` for GitHub Release creation.

Jobs:
1) `build-test-pack`
   - Restore, build (Release), and run tests.
   - Uses the `v*` tag as the package version on releases; manual runs use `Directory.Build.props`.
   - Packs the three NuGet packages to `artifacts/nuget`.
   - Validates packages by consuming them in a temporary console project.
   - Uploads packed artifacts (`retention-days: 30`).

2) `publish`
   - Downloads packaged artifacts.
   - Pushes `.nupkg` and `.snupkg` to NuGet.org (`--skip-duplicate`, symbols to `https://symbols.nuget.org/upload`).
   - Creates a GitHub Release for the tag and attaches the packages.

## Standard Release Flow
1. Prepare a branch:
   - Bump `AssemblyInfo.Version.cs`.
   - Update `CHANGELOG.md` and adjust `<PackageReleaseNotes>` if used.
   - Open a PR and ensure CI is green.

2. Merge to the default branch.

3. Manual pack test (optional):
   - GitHub → Actions → "NuGet Publish" → Run workflow → select the branch → keep `publish = false`.
   - Download the `nuget-packages` artifact and inspect.

4. Publish:
   - Tag the commit with the exact version:
     - `git tag -a v2.0.0 -m "Release 2.0.0"`
     - `git push origin v2.0.0`
   - The workflow uses the tag version for packages, publishes to NuGet, and creates a GitHub Release.

## Local Packaging/Publishing
For local builds or dry runs, scripts are available:
- `./pack-nuget.sh` → restore, build, test, pack to `artifacts/nuget`.
- `./publish-nuget.sh` → publishes the contents of `artifacts/nuget` using `NUGET_API_KEY`.

Note: Some tests require Windows kernels (P/Invoke). Prefer running the CI workflow (Windows) when verifying releases.

## Troubleshooting
- Invalid release tag: The workflow requires release tags to start with `v`.
- Duplicate package: NuGet returns 409; the workflow uses `--skip-duplicate` to ignore already-published versions.
- Symbol upload: Ensure `.snupkg` is produced; the workflow pushes symbols via `--symbol-source`.
- Failing tests on Linux: Use the Windows runner; do not run release validation on Linux-only runners.

## Quick Reference
- Version file: `Directory.Build.props` `<Version>`
- Assembly version: `AssemblyInfo.Version.cs`
- Workflow: `.github/workflows/nuget-publish.yml`
- NuGet secret: `KODECOMPAGNIET_SCHEDULER_NUGET_APIKEY`
- Artifacts: `artifacts/nuget` (`.nupkg` and `.snupkg`)
