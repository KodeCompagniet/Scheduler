# NuGet Publishing Guide (Quick Start)

This is a concise guide for publishing DemiCode Scheduler packages to NuGet. For full details on versioning, CI, tagging, validation, and troubleshooting, see `Documentation/Release_Management.md`.

## Target Packages
- `DemiCode.Shared`
- `DemiCode.Scheduling`
- `DemiCode.Logging.log4net`

Note: `DemiCode.Logging` was removed in v2.0.0; consumers should reference `DemiCode.Shared` for the `DemiCode.Logging` namespace.

## Prerequisites
- .NET SDK 8.0 (pinned by `global.json` to `8.0.120`).
- Strong-name key `DemiCodeStrongName.snk` already referenced in `Directory.Build.props`.
- For CI publishing, configure a NuGet.org trusted publisher for this GitHub repository and `.github/workflows/nuget-publish.yml`, and add the NuGet.org username as the `NUGET_USER` GitHub Actions secret. The workflow uses GitHub OIDC to obtain a short-lived publishing key; no long-lived NuGet API key secret is needed.
- For manual local publishing only, a NuGet.org API key is required as the `NUGET_API_KEY` environment variable.

## Manual Publish (Local)
1. Ensure the version is correct in `Directory.Build.props` `<Version>` and matches `AssemblyInfo.Version.cs`.
2. Create packages:
   ```bash
   ./pack-nuget.sh
   ```
3. Publish packages to NuGet:
   ```bash
   export NUGET_API_KEY=<your-key>
   ./publish-nuget.sh
   ```

## CI Publish (Recommended)
- Configure NuGet.org trusted publishing for the repository and workflow before the first publish.
- Tag the release commit with the exact version (for example, `v2.0.0`) and push the tag. The workflow validates the tag matches `<Version>`, exchanges its GitHub OIDC identity for a short-lived NuGet publishing key, publishes to NuGet, and creates a GitHub Release.
- For a manual pack test without publishing, run the "NuGet Publish" workflow via GitHub Actions with `workflow_dispatch` and leave `publish=false`.

Workflow file: `.github/workflows/nuget-publish.yml`.

## Minimal Validation
- Inspect a package before publishing:
  ```bash
  unzip -l artifacts/nuget/DemiCode.Scheduling.<version>.nupkg
  ```
  Confirm DLL, XML docs, `LICENSE.txt`, and `NuGet_README.md` (package root) are present.

For deeper validation, consumer-compilation checks, and troubleshooting, use `Documentation/Release_Management.md`.
