# OPSW11 System Optimizer

> A lightweight, open-source Windows system optimization tool built with WPF and .NET 10.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows)
![License](https://img.shields.io/badge/license-MIT-green)
![Version](https://img.shields.io/badge/version-1.1.0--alpha-blue)

---

## Features

### 🚀 Quick Fix
One-click cleanup that runs a safe set of operations with no restart required:
- User temp files (`%TEMP%`)
- Windows temp directory
- Prefetch cache
- DNS cache flush
- Windows Update download cache

### 🔧 Advanced Fix
Deep system repair pipeline — automatically creates a restore point before starting:
- **SFC /scannow** — scans and repairs protected system files
- **DISM /RestoreHealth** — repairs the Windows component store
- **Windows Update reset** — stops WU services, wipes `SoftwareDistribution` and `catroot2`, re-registers DLLs
- **Netsh full stack reset** — resets Winsock and TCP/IP

### 🛠 Custom Fix
Build your own repair checklist from 14 individual operations across five categories: Cleanup, Network, System Repair, Services, and Disk.

All long-running operations support **live cancellation** via an "Anuluj" button.

### 📊 Dashboard
Real-time system metrics refreshed every 2 seconds:
- CPU load
- RAM usage
- Disk usage (drive C:)
- System uptime
- OS version and hostname

### 📋 Logs
Session-level event journal with severity filtering (Info / Warning / Error / Success).  
Log files are stored in `%APPDATA%\OPSW11\Logi\` and automatically rotated after 30 days.
Entries can be exported to a `.log` / `.txt` file from the toolbar.

### 🌐 Languages
The whole UI is localizable and ships with **six languages** — Polish, English, German,
Spanish, French and Ukrainian. Switch instantly from the sidebar; the choice (and the
light/dark theme) is remembered in `%APPDATA%\OPSW11\settings.json` between sessions.

---

## Requirements

| Requirement | Value |
|---|---|
| OS | Windows 10 / 11 (64-bit) |
| Runtime | [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) |
| Privileges | **Administrator** (required — UAC prompt on launch) |

---

## Build from Source

```bash
git clone https://github.com/Mafu2k/OPSW11.git
cd OPSW11/OPSW11
dotnet build OPSW11.csproj -c Release
```

> Requires the **.NET 10 SDK** (the project targets `net10.0-windows`).

### Publish as self-contained executable

```bash
dotnet publish OPSW11.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist/
```

The output binary (`dist\OPSW11.exe`) requires no separate runtime installation.

---

## Project Structure

```
OPSW11/
├── Helpers/
│   ├── AdminHelper.cs        # UAC elevation & admin check
│   ├── AppServices.cs        # Shared service instances (DI-lite)
│   ├── FormatHelper.cs       # Byte / uptime formatting
│   ├── ProcessHelper.cs      # Async process runner; kills the child tree on cancel
│   └── SystemPaths.cs        # Full paths to Windows system tools (anti PATH-hijacking)
│
├── Localization/
│   ├── LocalizationManager.cs # Runtime language source (live-switchable)
│   ├── LocExtension.cs        # {loc:Loc Key} XAML markup extension
│   └── Translations.cs        # 6-language string table
│
├── Models/
│   ├── LogEntry.cs           # Log entry model
│   ├── OperationResult.cs    # Result type for all operations
│   └── SelectableOperation.cs
│
├── Services/
│   ├── BackupService.cs      # System restore point + service state backup
│   ├── CleanupService.cs     # Temp files, Prefetch, Recycle Bin, WU cache
│   ├── DiskService.cs        # SSD TRIM / HDD defrag (auto-detection)
│   ├── LoggingService.cs     # Singleton logger with 30-day file rotation
│   ├── NetworkService.cs     # DNS flush, service restart, netsh stack reset
│   ├── RepairService.cs      # SFC, DISM, Windows Update reset
│   ├── ServiceManagerService.cs # Service startup config (validated names)
│   ├── SettingsService.cs    # Persisted language + theme preferences
│   └── SystemInfoService.cs  # Real-time CPU / RAM / disk metrics via Win32
│
└── Views/
    ├── AdvancedFixView       # Advanced repair pipeline UI
    ├── CustomFixView         # Checklist-based custom repair UI
    ├── DashboardView         # Real-time system metrics dashboard
    ├── LogsView              # Session log viewer
    └── QuickFixView          # One-click quick cleanup UI
```

---

## Security Notes

- All system tools are referenced by **full absolute path** (`System32\sc.exe`, `System32\net.exe`, etc.) to eliminate PATH-hijacking risk.
- Drive letter input is validated (`char.IsAsciiLetter`) before being interpolated into process arguments.
- Service names are validated against a strict allowlist regex before being passed to `sc.exe`, preventing command-line argument injection.
- Cancelling a long-running repair **kills the entire child process tree** (`Process.Kill(entireProcessTree: true)`) so no elevated helper (SFC/DISM/defrag/netsh) is left orphaned.
- The `SHEmptyRecycleBin` P/Invoke is pinned to the Unicode entry point with `SetLastError`.
- The application requires administrator privileges and declares this in `app.manifest` — it will not silently downgrade.
- `BackupService` creates a **VSS restore point** before every destructive operation.

---

## License

MIT © Łukasz Janicki — see [LICENSE](LICENSE) for details.
