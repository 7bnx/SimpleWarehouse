namespace SimpleWarehouse.Common
{
  public interface ILogger
  {
    void Write(LogType type, string text, Exception? ex = null);
  }

  public enum LogType
  {
    Info,
    Debug,
    Error,
    Trace,
    Warning
  }
}
