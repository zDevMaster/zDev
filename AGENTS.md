# AGENTS.md

## Cursor Cloud specific instructions

This is a self-contained C# .NET 8.0 console application (no web server, no database, no Docker). The only system dependency is the .NET 8.0 SDK.

### Quick reference

| Action | Command |
|---|---|
| Restore deps | `dotnet restore` |
| Build | `dotnet build` |
| Run | `dotnet run` |
| Format check | `dotnet format --verify-no-changes` |

- **No test project exists** in this repository. There is no `*.sln` file and no test framework configured.
- `dotnet format --verify-no-changes` reports pre-existing whitespace issues (exit code 2). This is the repository's baseline state; do not treat these as regressions.
- The .NET 8.0 SDK must be installed at `/usr/share/dotnet` with a symlink at `/usr/local/bin/dotnet`. The update script handles `dotnet restore` only.
- JSON config files (`plugins.json`, `plugins-simple.json`, `plugins-validation.json`, `Examples/plugins-custom.json`) are read at runtime from the working directory. Always run `dotnet run` from the repository root (`/workspace`).
