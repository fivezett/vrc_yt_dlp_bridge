using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

public class Clients
{
    private static readonly HttpClient Client = new();

    ~Clients()
    {
        Client.Dispose();
    }

    private static DateTime? lastUpdateDate
    {
        get
        {
            try
            {
                if (File.Exists(Constraint.YtDlpClientUpdateDatePath))
                {
                    var text = File.ReadAllText(Constraint.YtDlpClientUpdateDatePath);
                    if (DateTime.TryParse(text, out var dt)) return dt;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to read last update date:\n" + e, Logger.LogSource.ManagementResource);
            }
            return null;
        }

        set
        {
            try
            {
                File.WriteAllText(Constraint.YtDlpClientUpdateDatePath, value?.ToString("o") ?? "");
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to write last update date:\n" + e, Logger.LogSource.ManagementResource);
            }
        }
    }


    public static async Task YtDlpDownloadAndUpdate()
    {
        if (File.Exists(Constraint.YtDlpClientPath))
        {
            if (lastUpdateDate.HasValue && (DateTime.Now - lastUpdateDate.Value).TotalDays < Constraint.YtDlpUpdateIntervalDays)
            {
                Logger.Info($"yt-dlp was updated within the last {Constraint.YtDlpUpdateIntervalDays} day(s). Skipping update.", Logger.LogSource.ManagementResource);
                return;
            }

            var process = new Process();
            process.StartInfo.FileName = Constraint.YtDlpClientPath;
            process.StartInfo.Arguments = "--update";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null) Logger.Info(e.Data, Logger.LogSource.ManagementResource);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null) Logger.Warning(e.Data, Logger.LogSource.ManagementResource);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            lastUpdateDate = DateTime.Now;
        }
        else
        {
            if (!Directory.Exists(Path.GetDirectoryName(Constraint.YtDlpClientPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(Constraint.YtDlpClientPath)!);

            try
            {
                await File.WriteAllBytesAsync(Constraint.YtDlpClientPath, await Client.GetByteArrayAsync(Constraint.YtDlpRemoteUrl));
                Logger.Info("yt-dlp downloaded successfully.", Logger.LogSource.ManagementResource);
                lastUpdateDate = DateTime.Now;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed Download yt-dlp:\n" + e, Logger.LogSource.ManagementResource);
            }
        }
    }

    public static async Task DownloadDeno()
    {
        if (File.Exists(Constraint.DenoPath))
            return;
        try
        {
            var version = (await Client.GetStringAsync(Constraint.DenoVersionUrl)).Trim();
            Logger.Info($"Downloading Deno version: {version}", Logger.LogSource.ManagementResource);
            var denoZipUrl = string.Format(Constraint.DenoDownloadUrlTemplate, version);
            Logger.Info($"Deno download URL: {denoZipUrl}", Logger.LogSource.ManagementResource);
            var response = await Client.GetAsync(denoZipUrl);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            var archive = new ZipArchive(stream);
            await archive.ExtractToDirectoryAsync(Constraint.YtDlpWorkingDirPath, true);
            Logger.Info("Deno downloaded successfully.", Logger.LogSource.ManagementResource);
        }
        catch (Exception e)
        {
            Logger.Error($"Failed Download Deno:\n" + e, Logger.LogSource.ManagementResource);
        }
    }

    public static async Task ExecYtDlp(string[] args)
    {
        if (!Directory.Exists(Constraint.TmpDirectory))
            Directory.CreateDirectory(Constraint.TmpDirectory);
        if (!Directory.Exists(Constraint.HomeDirectory))
            Directory.CreateDirectory(Constraint.HomeDirectory);

        var startInfo = new ProcessStartInfo
        {
            FileName = Constraint.YtDlpClientPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Constraint.YtDlpWorkingDirPath,
        };

        List<string> pargs = [.. args, "--ignore-config", "--js-runtimes", "deno:./deno.exe"];
        pargs = pargs.Where(arg => arg != "--no-cache-dir" && arg != "--rm-cache-dir").ToList();
        foreach (var arg in pargs) startInfo.ArgumentList.Add(arg);

        startInfo.EnvironmentVariables["TEMP"] = startInfo.EnvironmentVariables["TMP"] = Constraint.TmpDirectory;
        startInfo.EnvironmentVariables["HOME"] = Constraint.HomeDirectory;

        Logger.Info(new string('=', Constraint.LogSeparatorLength) + $"\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]  {string.Join(" ", startInfo.ArgumentList)}", Logger.LogSource.YtDlpBridge);

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                var isUrl = Regex.IsMatch(e.Data, @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$");
                if (isUrl)
                    Console.WriteLine(e.Data);
                Logger.Info((isUrl ? "[URL] " : "") + e.Data, Logger.LogSource.YtDlpBridge);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                Logger.Error(e.Data, Logger.LogSource.YtDlpBridge);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        Environment.Exit(process.ExitCode);
    }
}
