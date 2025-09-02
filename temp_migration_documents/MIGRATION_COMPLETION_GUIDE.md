# .NET 8 Migration Completion Guide

## Current Migration Status

### ✅ Successfully Migrated (5 projects)
- **DemiCode.Shared** - Core library (netstandard2.0 + net48)
- **DemiCode.Scheduling** - Scheduling library (netstandard2.0 + net48)  
- **DemiCode.Logging** - Logging library (netstandard2.0 + net48)
- **DemiCode.Logging.log4net** - log4net integration (netstandard2.0 + net48)
- **DemiCode.Shared.Test** - Test project (net8.0)

### ⚠️ Partially Migrated (2 projects)
- **DemiCode.Scheduling.Test.Dummy** - Builds but may have runtime issues (net8.0)
- **DemiCode.Logging.Feeds** - Project file migrated but has compilation errors (net8.0)

### ❌ Need Completion (4 projects)
- **DemiCode.Scheduling.Test** - 185 errors, needs NUnit migration
- **DemiCode.Logging.Test** - Needs NUnit migration
- **DemiCode.Logging.Feeds.Test** - Needs NUnit migration  
- **DemiCode.SchedulerService** - Windows service, needs testing on Windows

## Key Issues Fixed

### 1. Project File Migration
- All project files converted to SDK-style format
- Multi-targeting configured for libraries (netstandard2.0 + net48)
- Test projects target net8.0

### 2. Code Compatibility Issues
- **MonthlySchedule.cs**: Fixed base constructor call - was passing DateTime instead of string name
- **Namespace issues**: Changed `DemiCode.Shared.Configuration` to `DemiCode.Configuration`
- **InternalsVisibleTo**: Added DemiCode.Logging to DemiCode.Shared's AssemblyInfo
- **Security attributes**: Removed obsolete FileIOPermission and ReflectionPermission attributes
- **AppDomain.SetupInformation**: Replaced with ConfigurationManager.OpenExeConfiguration()

### 3. Assembly Attribute Conflicts
- Removed duplicate attributes between AssemblyInfo.Common.cs and Directory.Build.props
- Test projects had duplicate AssemblyInfo includes that were removed

## Remaining Work

### 1. Complete NUnit 2.x to 3.x Migration

The test projects use NUnit 2.4.8 APIs that don't exist in NUnit 3.14.0. Required changes:

#### a. Replace [ExpectedException] attribute
```csharp
// OLD (NUnit 2.x)
[Test]
[ExpectedException(typeof(ArgumentNullException))]
public void TestMethod()
{
    SomeClass.MethodThatThrows(null);
}

// NEW (NUnit 3.x)
[Test]
public void TestMethod()
{
    Assert.Throws<ArgumentNullException>(() => SomeClass.MethodThatThrows(null));
}
```

#### b. Replace Assert.IsInstanceOfType
```csharp
// OLD (NUnit 2.x)
Assert.IsInstanceOfType(typeof(ExpectedType), actualObject);

// NEW (NUnit 3.x)
Assert.IsInstanceOf<ExpectedType>(actualObject);
// OR
Assert.That(actualObject, Is.InstanceOf<ExpectedType>());
```

#### c. Fix AppDomain configuration file access
```csharp
// OLD (.NET Framework)
var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

// NEW (.NET Core/5+)
var configFile = System.Configuration.ConfigurationManager
    .OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
```

#### Files needing ExpectedException migration (26 files):
- DemiCode.Scheduling.Test: 14 files
- DemiCode.Shared.Test: 2 files (✅ completed)
- DemiCode.Logging.Test: 2 files

Run this command to find all occurrences:
```bash
grep -r "ExpectedException" --include="*.cs" DemiCode.*.Test/
```

#### Files needing IsInstanceOfType migration (16 files):
Run this command to find all occurrences:
```bash
grep -r "Assert.IsInstanceOfType" --include="*.cs" DemiCode.*.Test/
```

### 2. Fix DemiCode.Logging.Feeds Compilation

Check and fix:
- Missing assembly attributes
- Reference issues with System.ServiceModel.Syndication
- Any Windows-specific dependencies

### 3. Test DemiCode.SchedulerService on Windows

This is a Windows Service project targeting net8.0-windows:
- Verify it compiles on Windows
- Test service installation and runtime
- May need to update service hosting model for .NET 8

### 4. Update Mocking Framework

Consider migrating from Rhino.Mocks (abandoned) to Moq:
```xml
<!-- Add to test project files -->
<PackageReference Include="Moq" Version="4.20.70" />
```

### 5. Solution File Cleanup

The .sln file has references to non-existent projects. Either:
- Clean up the solution file manually
- Create a new solution file:
```bash
dotnet new sln -n DemiCode.Scheduling
dotnet sln add **/*.csproj
```

## Build Commands for Verification

```bash
# Build all core libraries
dotnet build DemiCode.Shared/DemiCode.Shared.csproj
dotnet build DemiCode.Scheduling/DemiCode.Scheduling.csproj  
dotnet build DemiCode.Logging/DemiCode.Logging.csproj
dotnet build DemiCode.Logging.log4net/DemiCode.Logging.log4net.csproj

# Build test projects (after completing NUnit migration)
dotnet build DemiCode.Shared.Test/DemiCode.Shared.Test.csproj
dotnet build DemiCode.Scheduling.Test/DemiCode.Scheduling.Test.csproj
dotnet build DemiCode.Logging.Test/DemiCode.Logging.Test.csproj

# Run tests
dotnet test --no-build
```

## Migration Completion Checklist

- [ ] Complete NUnit 2.x to 3.x migration in all test projects
- [ ] Fix DemiCode.Logging.Feeds compilation errors
- [ ] Test DemiCode.SchedulerService on Windows
- [ ] Replace Rhino.Mocks with Moq
- [ ] Clean up or recreate solution file
- [ ] Run full test suite
- [ ] Update documentation
- [ ] Create migration validation tests
- [ ] Performance testing (compare with .NET Framework version)
- [ ] Update CI/CD pipelines for .NET 8

## Important Notes

1. **Strong Naming**: The projects use strong naming with DemiCodeStrongName.snk. Ensure this file exists and is properly referenced.

2. **Multi-targeting**: Core libraries target both netstandard2.0 and net48 for backward compatibility. This is intentional and should be maintained.

3. **Language Version**: Set to C# 7.3 for compatibility. Can be upgraded to C# 12 if needed:
```xml
<LangVersion>12.0</LangVersion>
```

4. **Windows Compatibility**: Some projects (SchedulerService) are Windows-specific. These cannot be fully tested on Linux.

5. **Configuration System**: Heavy use of System.Configuration which works differently in .NET Core/5+. May need additional testing.

## Automated Migration Script

For the repetitive NUnit migrations, consider using this PowerShell script:

```powershell
# Fix ExpectedException
Get-ChildItem -Path . -Filter *.cs -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match '\[ExpectedException\(typeof\((\w+)\)\)\]') {
        # This needs manual review - automated replacement is risky
        Write-Host "Found ExpectedException in: $($_.FullName)"
    }
}

# Fix IsInstanceOfType
Get-ChildItem -Path . -Filter *.cs -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $newContent = $content -replace 'Assert\.IsInstanceOfType\(typeof\((\w+)\),\s*(.+?)\)', 'Assert.IsInstanceOf<$1>($2)'
    if ($content -ne $newContent) {
        Set-Content -Path $_.FullName -Value $newContent
        Write-Host "Updated: $($_.FullName)"
    }
}
```

## Support Resources

- [NUnit 2 to 3 Migration Guide](https://docs.nunit.org/articles/nunit/release-notes/Nunit3.0.html)
- [.NET Upgrade Assistant](https://dotnet.microsoft.com/platform/upgrade-assistant)
- [Breaking changes in .NET 5+](https://docs.microsoft.com/en-us/dotnet/core/compatibility/)
- [System.Configuration in .NET Core](https://docs.microsoft.com/en-us/dotnet/core/porting/configuration)

## Contact for Questions

If you encounter issues not covered here:
1. Check the git history for context on specific changes
2. Review the original migration.md document for requirements
3. Test thoroughly on both Windows and Linux platforms