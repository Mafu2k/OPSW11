# OPSW11

A small Windows 10/11 maintenance tool I wrote in C# (WPF, .NET 10). It wraps the usual
"fix my PC" routine (temp cleanup, SFC, DISM, resetting Windows Update or the network stack)
in one window, so I don't have to remember the commands or run them one by one.

The app needs administrator rights (it asks via UAC on launch) and creates a system restore
point before anything that changes the system.

## What it does

- **Quick Fix** is a safe one-click cleanup with no restart needed: user and Windows temp
  files, Prefetch, DNS cache and the Windows Update download cache.
- **Advanced Fix** runs `sfc /scannow`, `DISM /RestoreHealth`, a full Windows Update reset
  and `netsh` Winsock/TCP-IP reset, one after another.
- **Custom Fix** lets you pick any of the 14 individual operations (cleanup, network,
  system repair, services, disk TRIM/defrag).
- **Dashboard** shows live CPU, RAM, C: usage and uptime.
- **Logs** keeps a filterable session log that can be exported. Log files go to
  `%APPDATA%\OPSW11\Logi` and are rotated after 30 days.

Every long-running step can be cancelled. Cancelling kills the whole child process tree,
so no elevated `sfc`/`dism` stays behind. The UI is available in Polish, English, German,
Spanish, French and Ukrainian, with a light and dark theme. Both choices are remembered
in `%APPDATA%\OPSW11\settings.json`.

## Building

You need the .NET 10 SDK on Windows.

```bash
cd OPSW11
dotnet build -c Release

# single self-contained exe in dist/
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist/
```

## A few implementation notes

Since the app runs elevated, I tried not to give it easy ways to be abused:

- system tools (`sc.exe`, `net.exe`, ...) are always called by their full `System32` path
  (`Helpers/SystemPaths.cs`), never through `PATH`,
- service names and drive letters are validated before they go into command-line arguments,
- the manifest requires admin, so the app never quietly runs with half the permissions.

The code is split into `Services/` (one class per area: cleanup, repair, network, disk,
backup, ...), `Views/` for the five screens and `Localization/` with a tiny `{loc:Loc Key}`
markup extension that switches language live. Changes are listed in [CHANGELOG.md](CHANGELOG.md).

## License

MIT
