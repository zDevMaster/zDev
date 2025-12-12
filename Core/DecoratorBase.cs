namespace DecoratorPluginDemo.Core;

/// <summary>
/// 装饰器基类 - 实现装饰器模式的核心逻辑
/// </summary>
public abstract class DecoratorBase : IComponent
{
    protected IComponent _wrappedComponent;
    protected readonly string DecoratorName;

    protected DecoratorBase(IComponent component, string decoratorName)
    {
        _wrappedComponent = component;
        DecoratorName = decoratorName;
    }

    public virtual string Execute(string input)
    {
        // 前置处理
        string processedInput = PreProcess(input);
        
        // 调用被包装组件
        string result = _wrappedComponent.Execute(processedInput);
        
        // 后置处理
        return PostProcess(result);
    }

    public virtual string GetName()
    {
        return $"{DecoratorName}({_wrappedComponent.GetName()})";
    }

    /// <summary>
    /// 前置处理
    /// </summary>
    protected virtual string PreProcess(string input)
    {
        return input;
    }

    /// <summary>
    /// 后置处理
    /// </summary>
    protected virtual string PostProcess(string output)
    {
        return output;
    }
}
