namespace DecoratorPluginDemo.Core;

/// <summary>
/// 泛型插件接口 - 定义所有插件的通用行为
/// </summary>
/// <typeparam name="TInput">输入类型</typeparam>
/// <typeparam name="TOutput">输出类型</typeparam>
public interface IPlugin<TInput, TOutput>
{
    /// <summary>
    /// 执行操作
    /// </summary>
    /// <param name="input">输入数据</param>
    /// <returns>处理后的数据</returns>
    TOutput Execute(TInput input);
    
    /// <summary>
    /// 获取插件名称
    /// </summary>
    string GetName();
}

/// <summary>
/// 简化版插件接口 - 输入输出类型相同
/// </summary>
/// <typeparam name="T">输入输出类型</typeparam>
public interface IPlugin<T> : IPlugin<T, T>
{
}

/// <summary>
/// 字符串处理插件接口 - 向后兼容 IComponent
/// </summary>
public interface IStringPlugin : IPlugin<string>
{
}
