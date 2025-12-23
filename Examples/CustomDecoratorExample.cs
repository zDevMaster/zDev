using DecoratorPluginDemo.Core;
using DecoratorPluginDemo.Decorators;

namespace DecoratorPluginDemo.Examples;

/// <summary>
/// 自定义装饰器示例 - 添加前缀和后缀
/// </summary>
public class PrefixSuffixDecorator : DecoratorBase<string>, IComponent
{
    private readonly string _prefix;
    private readonly string _suffix;

    public PrefixSuffixDecorator(
        IPlugin<string> plugin, 
        string name = "前缀后缀装饰器",
        string prefix = "[",
        string suffix = "]") 
        : base(plugin, name)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    // 支持使用 IComponent 构造（向后兼容）
    public PrefixSuffixDecorator(
        IComponent component, 
        string name = "前缀后缀装饰器",
        string prefix = "[",
        string suffix = "]") 
        : base(new ComponentToPluginAdapter(component), name)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    protected override string PostProcess(string output)
    {
        var result = $"{_prefix}{output}{_suffix}";
        Console.WriteLine($"[{DecoratorName}] 添加前缀后缀: {output} => {result}");
        return result;
    }
}

/// <summary>
/// 自定义装饰器示例 - 字符替换
/// </summary>
public class ReplaceDecorator : DecoratorBase<string>, IComponent
{
    private readonly string _oldValue;
    private readonly string _newValue;

    public ReplaceDecorator(
        IPlugin<string> plugin, 
        string name = "替换装饰器",
        string oldValue = " ",
        string newValue = "_") 
        : base(plugin, name)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    // 支持使用 IComponent 构造（向后兼容）
    public ReplaceDecorator(
        IComponent component, 
        string name = "替换装饰器",
        string oldValue = " ",
        string newValue = "_") 
        : base(new ComponentToPluginAdapter(component), name)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    protected override string PostProcess(string output)
    {
        var result = output.Replace(_oldValue, _newValue);
        Console.WriteLine($"[{DecoratorName}] 字符替换: '{_oldValue}' => '{_newValue}'");
        return result;
    }
}

/// <summary>
/// 泛型重试装饰器 - 重试机制
/// </summary>
public class RetryDecorator<TInput, TOutput> : DecoratorBase<TInput, TOutput>
{
    private readonly int _maxRetries;
    private readonly int _delayMs;

    public RetryDecorator(
        IPlugin<TInput, TOutput> plugin, 
        string name = "重试装饰器",
        int maxRetries = 3,
        int delayMs = 100) 
        : base(plugin, name)
    {
        _maxRetries = maxRetries;
        _delayMs = delayMs;
    }

    public override TOutput Execute(TInput input)
    {
        int attempt = 0;
        Exception? lastException = null;

        while (attempt < _maxRetries)
        {
            try
            {
                attempt++;
                Console.WriteLine($"[{DecoratorName}] 尝试 {attempt}/{_maxRetries}");
                return base.Execute(input);
            }
            catch (Exception ex)
            {
                lastException = ex;
                Console.WriteLine($"[{DecoratorName}] 失败: {ex.Message}");
                
                if (attempt < _maxRetries)
                {
                    Console.WriteLine($"[{DecoratorName}] 等待 {_delayMs}ms 后重试...");
                    Thread.Sleep(_delayMs);
                }
            }
        }

        throw new Exception($"执行失败，已重试 {_maxRetries} 次", lastException);
    }
}

/// <summary>
/// 简化版泛型重试装饰器 - 输入输出类型相同
/// </summary>
public class RetryDecorator<T> : RetryDecorator<T, T>, IPlugin<T>
{
    public RetryDecorator(
        IPlugin<T> plugin, 
        string name = "重试装饰器",
        int maxRetries = 3,
        int delayMs = 100) 
        : base(plugin, name, maxRetries, delayMs)
    {
    }
}

/// <summary>
/// 字符串重试装饰器 - 向后兼容
/// </summary>
public class RetryDecorator : RetryDecorator<string>, IComponent
{
    public RetryDecorator(
        IPlugin<string> plugin, 
        string name = "重试装饰器",
        int maxRetries = 3,
        int delayMs = 100) 
        : base(plugin, name, maxRetries, delayMs)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public RetryDecorator(
        IComponent component, 
        string name = "重试装饰器",
        int maxRetries = 3,
        int delayMs = 100) 
        : base(new ComponentToPluginAdapter(component), name, maxRetries, delayMs)
    {
    }
}
