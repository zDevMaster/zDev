using System.Diagnostics;
using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 泛型性能监控装饰器 - 测量执行时间
/// </summary>
public class PerformanceDecorator<TInput, TOutput> : DecoratorBase<TInput, TOutput>
{
    public PerformanceDecorator(IPlugin<TInput, TOutput> plugin, string name = "性能监控装饰器") 
        : base(plugin, name)
    {
    }

    public override TOutput Execute(TInput input)
    {
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"[{DecoratorName}] 开始计时...");
        
        var result = base.Execute(input);
        
        stopwatch.Stop();
        Console.WriteLine($"[{DecoratorName}] 执行耗时: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)");
        
        return result;
    }
}

/// <summary>
/// 简化版泛型性能监控装饰器 - 输入输出类型相同
/// </summary>
public class PerformanceDecorator<T> : PerformanceDecorator<T, T>, IPlugin<T>
{
    public PerformanceDecorator(IPlugin<T> plugin, string name = "性能监控装饰器") 
        : base(plugin, name)
    {
    }
}

/// <summary>
/// 字符串性能监控装饰器 - 向后兼容
/// </summary>
public class PerformanceDecorator : PerformanceDecorator<string>, IComponent
{
    public PerformanceDecorator(IPlugin<string> plugin, string name = "性能监控装饰器") 
        : base(plugin, name)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public PerformanceDecorator(IComponent component, string name = "性能监控装饰器") 
        : base(new ComponentToPluginAdapter(component), name)
    {
    }
}
