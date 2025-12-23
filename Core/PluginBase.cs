namespace DecoratorPluginDemo.Core;

/// <summary>
/// 泛型插件基类 - 提供基础实现
/// </summary>
/// <typeparam name="TInput">输入类型</typeparam>
/// <typeparam name="TOutput">输出类型</typeparam>
public abstract class PluginBase<TInput, TOutput> : IPlugin<TInput, TOutput>
{
    protected readonly string PluginName;

    protected PluginBase(string pluginName)
    {
        PluginName = pluginName;
    }

    public abstract TOutput Execute(TInput input);

    public virtual string GetName()
    {
        return PluginName;
    }
}

/// <summary>
/// 简化版插件基类 - 输入输出类型相同
/// </summary>
/// <typeparam name="T">输入输出类型</typeparam>
public abstract class PluginBase<T> : PluginBase<T, T>, IPlugin<T>
{
    protected PluginBase(string pluginName) : base(pluginName)
    {
    }
}

/// <summary>
/// 字符串处理插件基类 - 向后兼容 ComponentBase
/// </summary>
public abstract class StringPluginBase : PluginBase<string>, IStringPlugin
{
    protected StringPluginBase(string pluginName) : base(pluginName)
    {
    }
}
