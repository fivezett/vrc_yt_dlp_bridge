internal class Logger
{
    private static readonly Dictionary<LogSource, Stream> FileStreams = new();
    private static readonly Dictionary<LogSource, StreamWriter> WriteStreams = new();

    ~Logger()
    {
        foreach (var stream in WriteStreams.Values) stream.Dispose();
        foreach (var stream in FileStreams.Values) stream.Dispose();
    }

    public enum LogSource
    {
        ManagementResource,
        YtDlpBridge,
    }

    public enum Level
    {
        Info,
        Warning,
        Error
    }

    public static void Info(string message, LogSource source) => AddLog(Level.Info, message, source);
    public static void Warning(string message, LogSource source) => AddLog(Level.Warning, message, source);
    public static void Error(string message, LogSource source) => AddLog(Level.Error, message, source);

    private static void AddLog(Level level, string message, LogSource source)
    {
        if (message.Trim().Length == 0) return;
        if (!WriteStreams.ContainsKey(source))
        {
            if (!Directory.Exists(Constraint.YtDlpWorkingDirPath))
                Directory.CreateDirectory(Constraint.YtDlpWorkingDirPath);
            var filestream = new FileStream(
                Path.Combine(Constraint.YtDlpWorkingDirPath, $"{source.ToString().ToLower()}_log.txt"),
                FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite);
            FileStreams.Add(source, filestream);
            WriteStreams.Add(source, new StreamWriter(filestream) { AutoFlush = true });
        }
        if (WriteStreams.TryGetValue(source, out var writer))
        {
            lock (writer)
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} ${level}] {message}");
        }
        else
        {
            Error($"Failed to get log writer for source {source}", LogSource.ManagementResource);
        }
    }
}
