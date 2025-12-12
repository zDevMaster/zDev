using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 日志装饰器 - 记录方法调用
/// </summary>
public class LoggingDecorator : DecoratorBase
{
    private readonly bool _logTimestamp;

    public LoggingDecorator(IComponent component, string name = "日志装饰器", bool logTimestamp = true) 
        : base(component, name)
    {
        _logTimestamp = logTimestamp;
    }

    protected override string PreProcess(string input)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 开始执行 =====");
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输入: {input}");
        return input;
    }

    protected override string PostProcess(string output)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输出: {output}");
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 执行完成 =====");
        return output;
    }
}
