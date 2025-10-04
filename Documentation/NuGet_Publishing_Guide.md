# NuGet Publishing Guide

End-to-end checklist for publishing DemiCode Scheduler packages to NuGet.

## Target Packages
- `DemiCode.Shared`
- `DemiCode.Scheduling`
- `DemiCode.Logging.log4net`

`DemiCode.Logging` is no longer published as of 2.0.0; consumers should migrate to `DemiCode.Shared`. Keep the project in the solution so type forwarders continue to compile for in-solution references.

## Version Management
- **Source of truth** – `Directory.Build.props` `<Version>` property.
- **Assembly version** – Update `AssemblyInfo.Version.cs` to the same value (major.minor.patch.0) when bumping.
- All shipping packages use the same version number. Create a Git tag that matches the published version (for example, `v2.0.0`) once the push succeeds.

## Prerequisites
- .NET SDK 8.0 (`global.json` pins `8.0.120`).
- Strong-name key `DemiCodeStrongName.snk` present (already referenced in `Directory.Build.props`).
- NuGet.org API key with push rights, exported locally as `NUGET_API_KEY` (set via shell or stored secret in CI).
- Working symbol upload: projects have `<IncludeSymbols>true</IncludeSymbols>` and `<SymbolPackageFormat>snupkg` configured.

## Release Preparation
1. Ensure `Directory.Build.props` and `AssemblyInfo.Version.cs` reflect the target version. Update `CHANGELOG.md` and `PackageReleaseNotes` metadata before packing.
2. Restore and build once to surface warnings as errors:
   ```bash
   dotnet build DemiCode.Scheduling.sln -c Release
   ```
3. Run the full test suite:
   ```bash
   dotnet test DemiCode.Scheduling.sln -c Release
   ```

## Packing
1. Choose an output folder (convention: `artifacts/nuget`).
2. Pack each publishable project using the shared version metadata:
   ```bash
   mkdir -p artifacts/nuget
   dotnet pack DemiCode.Shared/DemiCode.Shared.csproj -c Release --no-restore --output artifacts/nuget
   dotnet pack DemiCode.Scheduling/DemiCode.Scheduling.csproj -c Release --no-restore --output artifacts/nuget
   dotnet pack DemiCode.Logging.log4net/DemiCode.Logging.log4net.csproj -c Release --no-restore --output artifacts/nuget
   ```
   Each command emits a `.nupkg` and matching `.snupkg` in `artifacts/nuget`.

## Quick Validation
- Confirm package contents before pushing:
  ```bash
  unzip -l artifacts/nuget/DemiCode.Scheduling.<version>.nupkg
  ```
  Check for DLL, XML documentation, `LICENSE.txt`, and `NuGet_README.md` (in package root).
- (Optional but fast) Verify strong-name signature on one DLL if keys changed:
  ```bash
  sn -T DemiCode.Scheduling/bin/Release/netstandard2.0/DemiCode.Scheduling.dll
  ```

## Publishing
1. Export the API key for the session if not already set:
   ```bash
   export NUGET_API_KEY=<paste key>
   ```
2. Push packages (includes symbols via `--symbol-source`):
   ```bash
   dotnet nuget push "artifacts/nuget/DemiCode.*.nupkg" \
     --api-key "$NUGET_API_KEY" \
     --source https://api.nuget.org/v3/index.json \
     --symbol-source https://symbols.nuget.org/upload \
     --skip-duplicate
   ```
   Re-run the command if any package reports a transient failure.
3. After all pushes succeed, create the release tag and publish release notes on GitHub or your chosen channel.

## Automation Hooks
- CI can reuse the same steps: restore, build, test, pack to `artifacts/nuget`, then push on tagged builds using a stored `NUGET_API_KEY` secret.
- Keep this guide updated alongside changes to build tooling or package metadata so the manual and automated flows stay aligned.
