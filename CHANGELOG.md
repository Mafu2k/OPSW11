# Changelog

## 1.1.0-alpha (2026-07-10)

- UI in six languages (PL, EN, DE, ES, FR, UK), switched live from the sidebar
- Language and theme are saved in `%APPDATA%\OPSW11\settings.json`
- Session log can be exported to a file
- Service names are validated before they are passed to `sc.exe`
- Cancelling kills the whole process tree, so `sfc`/`dism` no longer keep running in the background
- Dashboard no longer crashes when the CPU performance counter is not available
- App data folder renamed from `WO11` to `OPSW11`

## 1.0.0 (2026-04-20)

- Dashboard with CPU, RAM, disk usage and uptime
- Quick Fix, Advanced Fix and Custom Fix (14 operations to pick from)
- Session log with a severity filter, log files rotated after 30 days
- Light and dark theme
- Restore point created before anything that changes the system
- Every long-running operation can be cancelled
