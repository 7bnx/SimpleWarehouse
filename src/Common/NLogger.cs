using NLog;

namespace SimpleWarehouse.Common
{
  public class NLogger : ILogger
  {
    private readonly Logger _log;
    private readonly Dictionary<LogType, Action<string>> _loggerType;

    public NLogger(string logName)
    {
      _log = LogManager.GetLogger(logName);
      _loggerType = new()
      {
        {LogType.Trace, _log.Trace},
        {LogType.Info, _log.Info},
        {LogType.Warning, _log.Warn },
        {LogType.Debug, _log.Debug},
        {LogType.Error, _log.Error}
      };
    }
    public void Write(LogType type, string text, Exception? ex = null)
    {
      string exceptionStr = ex?.Message ?? string.Empty;
#if DEBUG
      exceptionStr = ex != null ? $"{Environment.NewLine}{ex}" : string.Empty;
      System.Diagnostics.Debug.WriteLine($"[{type}] {text} {exceptionStr}");
#endif

      if (type == LogType.Error)
      {
        var msg = text;
        if (exceptionStr.Any())
          msg += $"{Environment.NewLine}{exceptionStr}";
        Console.WriteLine(msg);
      }
      if (_loggerType.ContainsKey(type))
        _loggerType[type](text);
    }
  }
}
