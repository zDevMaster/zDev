namespace DecoratorPluginDemo.Core;

/// <summary>
/// 组件接口 - 定义所有组件的通用行为
/// </summary>
public interface IComponent
{
    /// <summary>
    /// 执行操作
    /// </summary>
    /// <param name="input">输入数据</param>
    /// <returns>处理后的数据</returns>
    string Execute(string input);
    
    /// <summary>
    /// 获取组件名称
    /// </summary>
    string GetName();
}
