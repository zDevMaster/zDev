using DecoratorPluginDemo.Core;
using DecoratorPluginDemo.Decorators;
using DecoratorPluginDemo.Infrastructure;
using DecoratorPluginDemo.Plugins;

namespace DecoratorPluginDemo;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("泛型装饰器模式插件系统 - 演示");
        Console.WriteLine("========================================\n");

        // 示例1: 使用泛型插件加载器（新API）
        Console.WriteLine("\n【示例1: 泛型插件加载器】");
        Console.WriteLine("----------------------------------------");
        RunGenericExample("plugins.json");

        // 示例2: 使用简单配置
        Console.WriteLine("\n\n【示例2: 简单装饰器链】");
        Console.WriteLine("----------------------------------------");
        RunGenericExample("plugins-simple.json");

        // 示例3: 演示缓存功能
        Console.WriteLine("\n\n【示例3: 缓存功能演示】");
        Console.WriteLine("----------------------------------------");
        DemonstrateCaching();

        // 示例4: 代码方式构建泛型装饰器链
        Console.WriteLine("\n\n【示例4: 代码方式构建泛型装饰器链】");
        Console.WriteLine("----------------------------------------");
        DemonstrateCodeBasedDecoratorChain();

        // 示例5: 自定义类型泛型插件演示
        Console.WriteLine("\n\n【示例5: 自定义类型泛型插件】");
        Console.WriteLine("----------------------------------------");
        DemonstrateCustomTypePlugin();

        Console.WriteLine("\n\n========================================");
        Console.WriteLine("演示完成！");
        Console.WriteLine("========================================");
    }

    /// <summary>
    /// 使用泛型插件加载器运行示例
    /// </summary>
    static void RunGenericExample(string configFile)
    {
        try
        {
            // 使用泛型插件加载器 - 加载 IPlugin<string> 类型的插件
            var loader = new PluginLoader<IPlugin<string>>(configFile);
            
            Console.WriteLine($"\n正在从 {configFile} 加载插件...\n");
            var plugin = loader.LoadPlugin();
            
            // 显示插件结构
            Console.WriteLine($"\n插件结构: {plugin.GetName()}\n");
            
            // 执行测试
            Console.WriteLine("执行测试:\n");
            var result = plugin.Execute("  hello world  ");
            
            Console.WriteLine($"\n最终结果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// 演示缓存功能
    /// </summary>
    static void DemonstrateCaching()
    {
        try
        {
            var loader = new PluginLoader<IPlugin<string>>("plugins.json");
            var plugin = loader.LoadPlugin();
            
            Console.WriteLine("第一次调用 'test':");
            var result1 = plugin.Execute("test");
            
            Console.WriteLine("\n第二次调用 'test' (应该从缓存读取):");
            var result2 = plugin.Execute("test");
            
            Console.WriteLine("\n第一次调用 'another':");
            var result3 = plugin.Execute("another");
            
            Console.WriteLine("\n第三次调用 'test' (应该从缓存读取):");
            var result4 = plugin.Execute("test");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 演示代码方式构建泛型装饰器链
    /// </summary>
    static void DemonstrateCodeBasedDecoratorChain()
    {
        try
        {
            // 手动构建泛型装饰器链
            IPlugin<string> plugin = new TextProcessorComponent("基础处理器");
            
            // 应用泛型装饰器
            plugin = new LoggingDecorator<string>(plugin, "日志");
            plugin = new CacheDecorator<string>(plugin, "缓存");
            plugin = new PerformanceDecorator<string>(plugin, "性能监控");
            
            Console.WriteLine($"插件结构: {plugin.GetName()}\n");
            
            var result = plugin.Execute("泛型装饰器测试");
            Console.WriteLine($"\n最终结果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 演示自定义类型泛型插件
    /// </summary>
    static void DemonstrateCustomTypePlugin()
    {
        try
        {
            // 创建一个处理整数的泛型插件链
            IPlugin<int, int> mathPlugin = new MathProcessor();
            mathPlugin = new LoggingDecorator<int, int>(mathPlugin, "数学日志");
            mathPlugin = new PerformanceDecorator<int, int>(mathPlugin, "数学性能监控");
            
            Console.WriteLine($"数学插件结构: {mathPlugin.GetName()}\n");
            
            var result = mathPlugin.Execute(42);
            Console.WriteLine($"\n计算结果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}

/// <summary>
/// 自定义数学处理器 - 演示泛型插件的灵活性
/// </summary>
public class MathProcessor : PluginBase<int, int>
{
    public MathProcessor(string name = "数学处理器") : base(name)
    {
    }

    public override int Execute(int input)
    {
        Console.WriteLine($"[{PluginName}] 计算输入: {input}");
        return input * 2 + 10; // 简单的数学运算
    }
}

/// <summary>
/// 泛型日志装饰器 - 支持不同的输入输出类型
/// </summary>
public class LoggingDecorator<TInput, TOutput> : DecoratorBase<TInput, TOutput>
{
    private readonly bool _logTimestamp;

    public LoggingDecorator(IPlugin<TInput, TOutput> plugin, string name = "日志装饰器", bool logTimestamp = true) 
        : base(plugin, name)
    {
        _logTimestamp = logTimestamp;
    }

    protected override TInput PreProcess(TInput input)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 开始执行 =====");
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输入: {input}");
        return input;
    }

    protected override TOutput PostProcess(TOutput output)
    {
        var timestamp = _logTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " : "";
        Console.WriteLine($"{timestamp}[{DecoratorName}] 输出: {output}");
        Console.WriteLine($"{timestamp}[{DecoratorName}] ===== 执行完成 =====");
        return output;
    }
}

/// <summary>
/// 泛型性能监控装饰器 - 支持不同的输入输出类型
/// </summary>
public class PerformanceDecorator<TInput, TOutput> : DecoratorBase<TInput, TOutput>
{
    public PerformanceDecorator(IPlugin<TInput, TOutput> plugin, string name = "性能监控装饰器") 
        : base(plugin, name)
    {
    }

    public override TOutput Execute(TInput input)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        Console.WriteLine($"[{DecoratorName}] 开始计时...");
        
        var result = base.Execute(input);
        
        stopwatch.Stop();
        Console.WriteLine($"[{DecoratorName}] 执行耗时: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)");
        
        return result;
    }
}
