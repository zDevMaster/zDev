using System.Diagnostics;
using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 性能监控装饰器 - 测量执行时间
/// </summary>
public class PerformanceDecorator : DecoratorBase
{
    public PerformanceDecorator(IComponent component, string name = "性能监控装饰器") 
        : base(component, name)
    {
    }

    public override string Execute(string input)
    {
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"[{DecoratorName}] 开始计时...");
        
        var result = base.Execute(input);
        
        stopwatch.Stop();
        Console.WriteLine($"[{DecoratorName}] 执行耗时: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)");
        
        return result;
    }
}
