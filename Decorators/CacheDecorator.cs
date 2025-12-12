using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 缓存装饰器 - 缓存执行结果
/// </summary>
public class CacheDecorator : DecoratorBase
{
    private readonly Dictionary<string, string> _cache = new();
    private readonly int _maxCacheSize;

    public CacheDecorator(IComponent component, string name = "缓存装饰器", int maxCacheSize = 100) 
        : base(component, name)
    {
        _maxCacheSize = maxCacheSize;
    }

    public override string Execute(string input)
    {
        // 检查缓存
        if (_cache.TryGetValue(input, out var cachedResult))
        {
            Console.WriteLine($"[{DecoratorName}] 缓存命中! 键: {input}");
            return cachedResult;
        }

        Console.WriteLine($"[{DecoratorName}] 缓存未命中，执行组件");
        
        // 执行被包装的组件
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
