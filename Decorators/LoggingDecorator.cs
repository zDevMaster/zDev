using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 泛型日志装饰器 - 记录方法调用
/// </summary>
public class LoggingDecorator<T> : DecoratorBase<T>
{
    private readonly bool _logTimestamp;

    public LoggingDecorator(IPlugin<T> plugin, string name = "日志装饰器", bool logTimestamp = true) 
        : base(plugin, name)
    {
        _logTimestamp = logTimestamp;
    }

    protected override T PreProcess(T input)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 开始执行 =====");
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输入: {input}");
        return input;
    }

    protected override T PostProcess(T output)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输出: {output}");
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 执行完成 =====");
        return output;
    }
}

/// <summary>
/// 字符串日志装饰器 - 向后兼容
/// </summary>
public class LoggingDecorator : LoggingDecorator<string>, IComponent
{
    public LoggingDecorator(IPlugin<string> plugin, string name = "日志装饰器", bool logTimestamp = true) 
        : base(plugin, name)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public LoggingDecorator(IComponent component, string name = "日志装饰器", bool logTimestamp = true) 
        : base(new ComponentToPluginAdapter(component), name)
    {
    }
}

/// <summary>
/// IComponent 到 IPlugin<string> 的适配器
/// </summary>
internal class ComponentToPluginAdapter : IPlugin<string>
{
    private readonly IComponent _component;

    public ComponentToPluginAdapter(IComponent component)
    {
        _component = component;
    }

    public string Execute(string input) => _component.Execute(input);
    public string GetName() => _component.GetName();
}
