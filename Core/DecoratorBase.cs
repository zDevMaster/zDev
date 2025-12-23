namespace DecoratorPluginDemo.Core;

/// <summary>
/// 泛型装饰器基类 - 实现装饰器模式的核心逻辑
/// </summary>
/// <typeparam name="TInput">输入类型</typeparam>
/// <typeparam name="TOutput">输出类型</typeparam>
public abstract class DecoratorBase<TInput, TOutput> : IPlugin<TInput, TOutput>
{
    protected IPlugin<TInput, TOutput> _wrappedPlugin;
    protected readonly string DecoratorName;

    protected DecoratorBase(IPlugin<TInput, TOutput> plugin, string decoratorName)
    {
        _wrappedPlugin = plugin;
        DecoratorName = decoratorName;
    }

    public virtual TOutput Execute(TInput input)
    {
        // 前置处理
        TInput processedInput = PreProcess(input);
        
        // 调用被包装插件
        TOutput result = _wrappedPlugin.Execute(processedInput);
        
        // 后置处理
        return PostProcess(result);
    }

    public virtual string GetName()
    {
        return $"{DecoratorName}({_wrappedPlugin.GetName()})";
    }

    /// <summary>
    /// 前置处理
    /// </summary>
    protected virtual TInput PreProcess(TInput input)
    {
        return input;
    }

    /// <summary>
    /// 后置处理
    /// </summary>
    protected virtual TOutput PostProcess(TOutput output)
    {
        return output;
    }
}

/// <summary>
/// 简化版装饰器基类 - 输入输出类型相同
/// </summary>
/// <typeparam name="T">输入输出类型</typeparam>
public abstract class DecoratorBase<T> : DecoratorBase<T, T>, IPlugin<T>
{
    protected DecoratorBase(IPlugin<T> plugin, string decoratorName) 
        : base(plugin, decoratorName)
    {
    }
}

/// <summary>
/// 字符串装饰器基类 - 向后兼容原有 DecoratorBase
/// </summary>
public abstract class StringDecoratorBase : DecoratorBase<string>, IStringPlugin
{
    protected StringDecoratorBase(IPlugin<string> plugin, string decoratorName) 
        : base(plugin, decoratorName)
    {
    }
    
    // 支持使用 IComponent 构造（向后兼容）
    protected StringDecoratorBase(IComponent component, string decoratorName) 
        : base(new ComponentAdapter(component), decoratorName)
    {
    }
}

/// <summary>
/// IComponent 到 IPlugin<string> 的适配器
/// </summary>
internal class ComponentAdapter : IPlugin<string>
{
    private readonly IComponent _component;

    public ComponentAdapter(IComponent component)
    {
        _component = component;
    }

    public string Execute(string input) => _component.Execute(input);
    public string GetName() => _component.GetName();
}
