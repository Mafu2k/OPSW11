using System.Diagnostics;
using System.Globalization;
using System.Text;
using OPSW11.Models;

namespace OPSW11.Helpers;

public static class ProcessHelper
{
    public static async Task<OperationResult> RunAsync(
        string executable,
        string arguments,
        Action<string>? outputCallback = null,
        CancellationToken cancellationToken = default)
    {
        var outputBuilder = new StringBuilder();

        var oemEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = oemEncoding,
            StandardErrorEncoding = oemEncoding
        };

        Process? process = null;
        try
        {
            process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is null) return;
                outputBuilder.AppendLine(e.Data);
                outputCallback?.Invoke(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is null) return;
                outputBuilder.AppendLine(e.Data);
                outputCallback?.Invoke(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);

            string output = outputBuilder.ToString();
            return process.ExitCode == 0
                ? OperationResult.Success($"Completed (exit code 0)", output)
                : OperationResult.Failure($"Process exited with code {process.ExitCode}", output: output);
        }
        catch (OperationCanceledException)
        {
            // The token was cancelled while waiting — make sure the child process
            // (SFC / DISM / defrag / netsh) is actually terminated instead of orphaned.
            TryKillProcessTree(process);
            return OperationResult.Failure("Operation was cancelled by the user.");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to start '{executable}': {ex.Message}", ex);
        }
        finally
        {
            process?.Dispose();
        }
    }

    private static void TryKillProcessTree(Process? process)
    {
        try
        {
            if (process is { HasExited: false })
                process.Kill(entireProcessTree: true);
        }
        catch
        {
            // Process may have already exited or be inaccessible — nothing else to do.
        }
    }
}
