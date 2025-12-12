namespace DecoratorPluginDemo.Core;

/// <summary>
/// 组件基类 - 提供基础实现
/// </summary>
public abstract class ComponentBase : IComponent
{
    protected readonly string ComponentName;

    protected ComponentBase(string componentName)
    {
        ComponentName = componentName;
    }

    public abstract string Execute(string input);

    public virtual string GetName()
    {
        return ComponentName;
    }
}
