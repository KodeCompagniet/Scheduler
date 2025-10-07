# NuGet Publishing Guide (Quick Start)

This is a concise guide for publishing DemiCode Scheduler packages to NuGet. For full details on versioning, CI, tagging, validation, and troubleshooting, see `Documentation/Release_Management.md`.

## Target Packages
- `DemiCode.Shared`
- `DemiCode.Scheduling`
- `DemiCode.Logging.log4net`

Note: `DemiCode.Logging` is not published as of v2.x.

## Prerequisites
- .NET SDK 8.0 (pinned by `global.json` to `8.0.120`).
- Strong-name key `DemiCodeStrongName.snk` already referenced in `Directory.Build.props`.
- NuGet.org API key available as an environment variable (`NUGET_API_KEY`) or as the repository secret `KODECOMPAGNIET_SCHEDULER_NUGET_APIKEY` (for CI).

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
- Tag the release commit with the exact version (for example, `v2.0.0`) and push the tag. The workflow validates the tag matches `<Version>` and then publishes to NuGet and creates a GitHub Release.
- For a manual pack test without publishing, run the "NuGet Publish" workflow via GitHub Actions with `workflow_dispatch` and leave `publish=false`.

Workflow file: `.github/workflows/nuget-publish.yml`.

## Minimal Validation
- Inspect a package before publishing:
  ```bash
  unzip -l artifacts/nuget/DemiCode.Scheduling.<version>.nupkg
  ```
  Confirm DLL, XML docs, `LICENSE.txt`, and `NuGet_README.md` (package root) are present.

For deeper validation, consumer-compilation checks, and troubleshooting, use `Documentation/Release_Management.md`.
