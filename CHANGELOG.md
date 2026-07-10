# Changelog

All notable changes to this project will be documented in this file.  
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [1.1.0-alpha] — 2026-07-10

### Added
- **Multi-language UI** — full localization with six languages (Polish, English, German, Spanish, French, Ukrainian). Language switches live from the sidebar via a lightweight `{loc:Loc Key}` markup extension bound to a `LocalizationManager` singleton.
- **Persisted preferences** — selected language and light/dark theme are saved to `%APPDATA%\OPSW11\settings.json` and restored on next launch (`SettingsService`).
- **Log export** — the Logs view can export the current session journal to a `.log` / `.txt` file.

### Security Hardening
- Service names are validated against a strict allowlist regex (`^[A-Za-z0-9_.\-]{1,256}$`) before being interpolated into an `sc.exe` argument — closes an argument-injection vector.
- `SHEmptyRecycleBin` P/Invoke pinned to the Unicode entry point (`SHEmptyRecycleBinW`) with `SetLastError`.

### Fixed
- **Cancellation now actually stops work** — `ProcessHelper` kills the child process tree on cancel, so SFC/DISM/defrag/netsh are no longer left running in the background after "Anuluj".
- `SystemInfoService` no longer crashes the dashboard when the CPU performance counter is unavailable — it degrades to a 0% reading.
- Unified the app-data folder name from the legacy `WO11` to `OPSW11` (logs, backups, settings).

---

## [1.0.0] — 2026-04-20

### Added
- **Dashboard** — real-time CPU, RAM, disk and uptime metrics (2-second refresh via `DispatcherTimer`)
- **Quick Fix** — one-click safe cleanup: temp files, Prefetch, DNS cache, Windows Update cache
- **Advanced Fix** — full repair pipeline: SFC, DISM, Windows Update reset, netsh stack reset
- **Custom Fix** — 14 individually selectable operations across five categories, with live progress and a Cancel button
- **Logs view** — session-level event journal with severity filter and 30-day file rotation
- **Dark / light theme** toggle
- System restore point automatically created before every destructive operation
- All operations support `CancellationToken` — Cancel button visible during execution

### Security Hardening
- All system executables referenced by full path (`System32\sc.exe`, `net.exe`, etc.) to eliminate PATH hijacking
- Drive letter validated with `char.IsAsciiLetter` before process-argument interpolation
- Administrator privilege enforced via `app.manifest` (requestedExecutionLevel = requireAdministrator)
- `BackupService` catches only `InvalidOperationException` (service not found), not broad `Exception`
- Hardcoded `C:\Windows\...` paths replaced with `Environment.GetFolderPath(SpecialFolder.Windows)`

### Fixed
- `CustomFixView`: missing `_backup` / `_logger` fields caused compile error
- `AdminHelper.RestartWithElevation`: catch narrowed from `Exception` to `Win32Exception`
- `LogsView`: dead null-checks on `readonly` logger field removed
- `OperationResult.Success()`: removed unused `Exception` parameter

---

## [Unreleased]

- Log export to clipboard / file
- Scheduled task support (auto-run Quick Fix weekly)
- Settings page (configurable thresholds, theme persistence)
