# Test Migration Completion Plan - DemiCode.Scheduling

**Document Version**: 1.0  
**Date**: 2025-08-31  
**Current State**: 75% Complete  
**Target Completion**: 90%+ Test Pass Rate

## Executive Summary

This document outlines a comprehensive plan to complete the test migration for the DemiCode.Scheduling solution from .NET Framework to .NET 8. The migration is currently 75% complete with core libraries successfully building but test infrastructure requiring critical fixes.

## Current Test Status

### Test Metrics
| Project | Build Status | Test Pass Rate | Issues |
|---------|-------------|----------------|--------|
| DemiCode.Shared.Test | ✅ Success | 100% (71/71) | None |
| DemiCode.Scheduling.Test | ✅ Success | 70% (241/345) | Configuration file issues |
| DemiCode.Logging.Test | ❌ Failed | N/A | 7 compilation errors |
| DemiCode.Logging.Feeds.Test | ❌ Failed | N/A | Dependencies missing |

### Key Issues Identified

#### 1. Configuration File Runtime Issues (95 failures)
- **Root Cause**: Tests expect `testhost.dll.config` but the actual file is `DemiCode.Scheduling.Test.dll.config`
- **Impact**: All configuration-dependent tests fail at runtime
- **Affected Tests**: 
  - SchedulerSettingsManager tests
  - ConfigurationFileWatcher tests
  - Configuration section loading tests

#### 2. Constructor/Interface Compilation Issues (7 errors)
- **Root Cause**: `Log4netLogger` constructors are marked `internal`, tests trying to instantiate directly
- **Impact**: DemiCode.Logging.Test cannot compile
- **Error Types**:
  - CS1729: Constructor not found
  - CS0266: Interface casting issues

#### 3. Platform-Specific Dependencies
- **DemiCode.SchedulerService**: Missing `System.ServiceProcess` namespace
- **DemiCode.Logging.Feeds**: Missing WCF (`System.ServiceModel`) dependencies
- **Impact**: Windows-specific projects don't compile

## Solution Options

### Option A: Pragmatic Quick Fix (1-2 days)
**Goal**: Get tests passing with minimal changes

#### Pros
- ✅ Fastest path to working tests
- ✅ Minimal code changes
- ✅ Low risk of introducing new issues

#### Cons
- ❌ Technical debt accumulation
- ❌ Doesn't address underlying architectural issues
- ❌ May require rework later

#### Implementation
1. Copy config files with correct naming
2. Make constructors public or add InternalsVisibleTo
3. Skip Windows-specific tests on non-Windows platforms

---

### Option B: Proper Architectural Fix (3-5 days)
**Goal**: Modernize test infrastructure completely

#### Pros
- ✅ Long-term maintainability
- ✅ Modern .NET patterns
- ✅ Better test isolation

#### Cons
- ❌ Significant time investment
- ❌ Risk of breaking existing functionality
- ❌ Requires extensive testing

#### Implementation
1. Migrate to Microsoft.Extensions.Configuration
2. Implement dependency injection
3. Create proper abstractions for platform-specific code

---

### Option C: Hybrid Approach (2-3 days) - **RECOMMENDED** ⭐
**Goal**: Balance quick fixes with sustainable improvements

#### Pros
- ✅ Reasonable timeline
- ✅ Addresses critical issues properly
- ✅ Leaves room for future improvements
- ✅ Minimal breaking changes

#### Cons
- ❌ Some technical debt remains
- ❌ Not fully modernized

## Recommended Implementation Plan (Hybrid Approach)

### Phase 1: Critical Fixes (Day 1)

#### Task 1.1: Fix Configuration File Issue
**Problem**: Tests look for `testhost.dll.config` instead of actual config file

**Solution**: Add MSBuild target to copy config file
```xml
<!-- Add to DemiCode.Scheduling.Test/DemiCode.Scheduling.Test.csproj -->
<Target Name="CopyConfigForTestHost" AfterTargets="Build">
  <Copy SourceFiles="$(OutDir)$(AssemblyName).dll.config" 
        DestinationFiles="$(OutDir)testhost.dll.config" 
        SkipUnchangedFiles="true" />
</Target>

<!-- Alternative: Update app.config handling -->
<ItemGroup>
  <None Update="app.config">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <TargetPath>testhost.dll.config</TargetPath>
  </None>
</ItemGroup>
```

#### Task 1.2: Fix Logging Test Compilation
**Problem**: Internal constructors preventing test instantiation

**Solution A**: Add InternalsVisibleTo
```csharp
// Add to DemiCode.Logging.log4net/Properties/AssemblyInfo.cs
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DemiCode.Logging.Test")]
```

**Solution B**: Create Test-Specific Factory
```csharp
// Add to DemiCode.Logging.Test/TestHelpers/TestLoggerFactory.cs
public static class TestLoggerFactory
{
    public static ILogger CreateLogger(string name)
    {
        var factory = new Log4netLoggerFactory();
        return ((ILoggerFactory)factory).CreateLogger(name);
    }
}
```

#### Task 1.3: Fix Interface Casting Issues
```csharp
// In test files, change from:
ILoggerFactory loggerFactory = new Log4netLoggerFactory();

// To:
ILoggerFactory loggerFactory = (ILoggerFactory)new Log4netLoggerFactory();
// Or use explicit interface implementation pattern
```

### Phase 2: Test Modernization (Day 2)

#### Task 2.1: Update FileHelper for .NET 8
```csharp
// DemiCode.Scheduling.Test/FileHelper.cs
public static class FileHelper
{
    public static string GetConfigFilePath()
    {
        // .NET 8 compatible approach
        var configFile = Path.Combine(
            AppContext.BaseDirectory, 
            "testhost.dll.config");
        
        if (!File.Exists(configFile))
        {
            // Fallback to assembly-specific config
            var assemblyConfig = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.dll.config");
            
            if (File.Exists(assemblyConfig))
                return assemblyConfig;
        }
        
        return configFile;
    }
    
    public static void TouchFile(string file)
    {
        if (!File.Exists(file))
            File.WriteAllText(file, string.Empty);
        
        File.SetLastWriteTime(file, DateTime.Now);
    }
}
```

#### Task 2.2: Create Configuration Test Helpers
```csharp
// DemiCode.Scheduling.Test/TestHelpers/ConfigurationHelper.cs
public static class ConfigurationHelper
{
    public static Configuration GetTestConfiguration()
    {
        var map = new ExeConfigurationFileMap
        {
            ExeConfigFilename = FileHelper.GetConfigFilePath()
        };
        
        return ConfigurationManager.OpenMappedExeConfiguration(
            map, 
            ConfigurationUserLevel.None);
    }
}
```

#### Task 2.3: Fix Remaining Test Patterns
- Update all uses of `AppDomain.CurrentDomain.SetupInformation.ConfigurationFile`
- Replace with `ConfigurationHelper.GetTestConfiguration()`
- Ensure all test data files are copied to output directory

### Phase 3: Platform-Specific Handling (Day 3)

#### Task 3.1: Add Windows Compatibility Packages
```xml
<!-- DemiCode.SchedulerService/DemiCode.SchedulerService.csproj -->
<ItemGroup Condition="'$(TargetFramework)' == 'net8.0-windows'">
  <PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
  <PackageReference Include="Microsoft.Windows.Compatibility" Version="8.0.0" />
</ItemGroup>

<!-- DemiCode.Logging.Feeds/DemiCode.Logging.Feeds.csproj -->
<ItemGroup>
  <PackageReference Include="System.ServiceModel.Primitives" Version="8.0.0" />
  <PackageReference Include="System.ServiceModel.Http" Version="8.0.0" />
  <PackageReference Include="System.ServiceModel.Syndication" Version="8.0.0" />
</ItemGroup>
```

#### Task 3.2: Platform-Specific Test Attributes
```csharp
// For Windows-only tests
[Test]
[Platform("Win")]
public void WindowsServiceTest()
{
    // Windows-specific test code
}

// For cross-platform tests that need adjustment
[Test]
public void ConfigurationTest()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        // Windows-specific assertions
    }
    else
    {
        // Linux/Mac assertions or skip
        Assert.Ignore("Windows-specific test");
    }
}
```

#### Task 3.3: Conditional Compilation
```csharp
#if WINDOWS
    // Windows-specific implementation
    using System.ServiceProcess;
#else
    // Cross-platform alternative or stub
#endif
```

## Implementation Checklist

### Day 1 - Critical Fixes
- [ ] Add MSBuild target for config file copying
- [ ] Fix InternalsVisibleTo for Log4net tests
- [ ] Fix interface casting issues in Logging.Test
- [ ] Verify DemiCode.Scheduling.Test runs with config fix
- [ ] Verify DemiCode.Logging.Test compiles

### Day 2 - Test Modernization
- [ ] Update FileHelper for .NET 8 configuration
- [ ] Create ConfigurationHelper test utility
- [ ] Fix AppDomain configuration references
- [ ] Update test data file copying
- [ ] Run full test suite and document failures

### Day 3 - Platform Handling
- [ ] Add Windows compatibility packages
- [ ] Mark platform-specific tests
- [ ] Add conditional compilation where needed
- [ ] Test on Windows platform (if available)
- [ ] Document platform requirements

## Success Criteria

### Minimum Viable Completion
- ✅ All test projects compile without errors
- ✅ DemiCode.Scheduling.Test: >85% pass rate
- ✅ DemiCode.Shared.Test: 100% pass rate maintained
- ✅ DemiCode.Logging.Test: Compiles and >75% pass rate
- ✅ Solution builds on both Windows and Linux

### Stretch Goals
- ⭐ All tests passing (>95% pass rate)
- ⭐ Windows Service tests validated on Windows
- ⭐ Zero compilation warnings
- ⭐ Updated documentation for all breaking changes

## Risk Mitigation

### Risk 1: Config Changes Break Existing Tests
**Mitigation**: Keep both config file names during transition, remove duplicate after validation

### Risk 2: Platform-Specific Code Fails
**Mitigation**: Use platform detection and conditional test execution

### Risk 3: Performance Regression
**Mitigation**: Add benchmark tests before and after migration

## Rollback Plan

If critical issues arise:
1. Revert test project changes via git
2. Focus on core library stability
3. Document blocking issues for future resolution
4. Consider maintaining separate test projects for .NET 8 vs Framework

## Next Steps After Completion

1. **Documentation Updates**
   - Update README with .NET 8 requirements
   - Document breaking changes for consumers
   - Create migration guide for dependent projects

2. **CI/CD Integration**
   - Set up GitHub Actions for automated testing
   - Add both Windows and Linux build agents
   - Configure code coverage reporting

3. **Future Improvements**
   - Migrate to Microsoft.Extensions.Configuration
   - Implement proper dependency injection
   - Consider xUnit or MSTest v3 migration
   - Add integration test suite

## Appendix: Common Issues and Solutions

### Issue: "Could not load file or assembly"
**Solution**: Ensure all NuGet packages are restored and compatible with .NET 8

### Issue: "The type or namespace name 'X' does not exist"
**Solution**: Add appropriate compatibility packages or update using statements

### Issue: "Platform not supported"
**Solution**: Use conditional compilation or platform-specific packages

### Issue: Test discovery not working
**Solution**: Ensure test adapter packages are installed and up to date

## Resources

- [.NET 8 Migration Guide](https://docs.microsoft.com/en-us/dotnet/core/porting/)
- [NUnit 3 Migration Documentation](https://docs.nunit.org/articles/nunit/release-notes/Nunit3.0.html)
- [System.Configuration in .NET Core](https://docs.microsoft.com/en-us/dotnet/core/porting/configuration)
- [Windows Compatibility Pack](https://docs.microsoft.com/en-us/dotnet/core/porting/windows-compat-pack)

---

**Document Status**: Ready for Implementation  
**Author**: Migration Team  
**Review Date**: Before starting Phase 1  
**Approval**: Pending