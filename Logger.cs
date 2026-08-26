using Microsoft.Extensions.Logging;
using NReco.Logging.File;

internal static class Logger
{
    private static readonly Dictionary<LogSource, ILogger> Loggers = new();
    private static readonly List<ILoggerFactory> Factories = new();

    static Logger()
    {
        // NReco は背景スレッドで書き込むため、Dispose しないと終了直前のログが失われる
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            lock (Loggers) foreach (var f in Factories) f.Dispose();
        };
    }

    public enum LogSource
    {
        ManagementResource,
        YtDlpBridge,
    }

    public static void Info(string message, LogSource source) => Get(source).LogInformation("{Message}", message);
    public static void Warning(string message, LogSource source) => Get(source).LogWarning("{Message}", message);
    public static void Error(string message, LogSource source) => Get(source).LogError("{Message}", message);

    private static ILogger Get(LogSource source)
    {
        lock (Loggers)
        {
            if (Loggers.TryGetValue(source, out var logger)) return logger;
            var path = Path.Combine(Constraint.YtDlpWorkingDirPath, $"{source.ToString().ToLower()}_log.txt");
            // 出力先ファイルがソースごとに異なるため factory もソースごとに分ける
            var factory = LoggerFactory.Create(builder => builder.AddFile(path, opts =>
            {
                opts.Append = true;
                opts.FileSizeLimitBytes = Constraint.LogMaxBytes;
                opts.MaxRollingFiles = Constraint.LogMaxRollingFiles;
                opts.FormatLogEntry = msg => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg.LogLevel}] {msg.Message}";
            }));
            Factories.Add(factory);
            logger = factory.CreateLogger(source.ToString());
            Loggers.Add(source, logger);
            return logger;
        }
    }
}
