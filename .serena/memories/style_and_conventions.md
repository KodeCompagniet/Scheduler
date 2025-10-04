# Coding Style and Conventions
- C# codebase targeting language version 7.3; .NET Standard/core libs with explicit XML doc comments enabled (CS1591 suppressed globally).
- Favors classic .NET patterns: `#region` blocks, explicit private fields prefixed with `_`, PascalCase members, XML documentation on public APIs.
- Uses IDisposable patterns, factory classes, configuration-driven design. Null checks throw `ArgumentNullException`; prefers `try/finally` for resource cleanup.
- Tests use NUnit with descriptive test names, helper dummy projects, and Moq for fakes.