# Task Completion Checklist
- Ensure affected projects build: run `dotnet build DemiCode.Scheduling.sln`.
- Execute test suite covering scheduler changes: `dotnet test DemiCode.Scheduling.sln`.
- If configuration-related changes, verify sample apps or service hosts still start (`dotnet run ...`).
- Update XML docs or README when public API changes.
- Review logging configs if touching logging adapters.