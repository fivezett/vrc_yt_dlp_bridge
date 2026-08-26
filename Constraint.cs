internal static class Constraint
{
    public const string YtDlpRemoteUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
    public static readonly string VrcToolsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "VRChat", "VRChat", "Tools");
    public static readonly string YtDlpWorkingDirPath = Path.Combine(VrcToolsPath, "yt-dlp_bridge");
    public static readonly string YtDlpClientPath = Path.Combine(YtDlpWorkingDirPath, "yt-dlp.exe");
    public static readonly string DenoPath = Path.Combine(YtDlpWorkingDirPath, "deno.exe");
    public static readonly string TmpDirectory = Path.Combine(YtDlpWorkingDirPath, "tmp");
    public static readonly string HomeDirectory = Path.Combine(YtDlpWorkingDirPath, "home");
    public static readonly string YtDlpClientUpdateDatePath = Path.Combine(YtDlpWorkingDirPath, "yt-dlp_last_update.txt");
    public const string DenoVersionUrl = "https://dl.deno.land/release-latest.txt";
    public const string DenoDownloadUrlTemplate = "https://dl.deno.land/release/{0}/deno-x86_64-pc-windows-msvc.zip";
    public const int YtDlpUpdateIntervalDays = 1;
    public const int LogSeparatorLength = 80;
    public const int UrlProbeRetries = 2;
    public const int UrlProbeIntervalMs = 2000;
    public const int UrlProbeRequestTimeoutMs = 4000;
    public const int LogMaxBytes = 1024 * 1024;
    public const int LogMaxRollingFiles = 2;
}
    