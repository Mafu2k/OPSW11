# Changelog

All notable changes to this project will be documented in this file.  
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

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
