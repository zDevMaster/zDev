namespace DecoratorPluginDemo.Core;

/// <summary>
/// 组件接口 - 定义所有组件的通用行为
/// 继承自泛型 IPlugin<string> 接口，保持向后兼容
/// </summary>
public interface IComponent : IPlugin<string>
{
}
