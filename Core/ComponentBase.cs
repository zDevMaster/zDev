namespace DecoratorPluginDemo.Core;

/// <summary>
/// 组件基类 - 提供基础实现
/// 继承自泛型 PluginBase<string>，保持向后兼容
/// </summary>
public abstract class ComponentBase : PluginBase<string>, IComponent
{
    protected string ComponentName => PluginName;

    protected ComponentBase(string componentName) : base(componentName)
    {
    }
}
