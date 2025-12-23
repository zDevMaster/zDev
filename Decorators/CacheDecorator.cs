using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 泛型缓存装饰器 - 缓存执行结果
/// </summary>
public class CacheDecorator<TInput, TOutput> : DecoratorBase<TInput, TOutput> 
    where TInput : notnull
{
    private readonly Dictionary<TInput, TOutput> _cache = new();
    private readonly int _maxCacheSize;

    public CacheDecorator(IPlugin<TInput, TOutput> plugin, string name = "缓存装饰器", int maxCacheSize = 100) 
        : base(plugin, name)
    {
        _maxCacheSize = maxCacheSize;
    }

    public override TOutput Execute(TInput input)
    {
        // 检查缓存
        if (_cache.TryGetValue(input, out var cachedResult))
        {
            Console.WriteLine($"[{DecoratorName}] 缓存命中! 键: {input}");
            return cachedResult;
        }

        Console.WriteLine($"[{DecoratorName}] 缓存未命中，执行组件");
        
        // 执行被包装的插件
        var result = base.Execute(input);

        // 存入缓存
        if (_cache.Count >= _maxCacheSize)
        {
            // 简单的缓存淘汰策略：移除第一个元素
            var firstKey = _cache.Keys.First();
            _cache.Remove(firstKey);
            Console.WriteLine($"[{DecoratorName}] 缓存已满，移除: {firstKey}");
        }

        _cache[input] = result;
        Console.WriteLine($"[{DecoratorName}] 结果已缓存，缓存大小: {_cache.Count}");

        return result;
    }
}

/// <summary>
/// 简化版泛型缓存装饰器 - 输入输出类型相同
/// </summary>
public class CacheDecorator<T> : CacheDecorator<T, T>, IPlugin<T> 
    where T : notnull
{
    public CacheDecorator(IPlugin<T> plugin, string name = "缓存装饰器", int maxCacheSize = 100) 
        : base(plugin, name)
    {
    }
}

/// <summary>
/// 字符串缓存装饰器 - 向后兼容
/// </summary>
public class CacheDecorator : CacheDecorator<string>, IComponent
{
    public CacheDecorator(IPlugin<string> plugin, string name = "缓存装饰器", int maxCacheSize = 100) 
        : base(plugin, name)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public CacheDecorator(IComponent component, string name = "缓存装饰器", int maxCacheSize = 100) 
        : base(new ComponentToPluginAdapter(component), name)
    {
    }
}
