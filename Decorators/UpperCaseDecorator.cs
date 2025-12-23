using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 大写转换装饰器 - 将输出转换为大写（字符串专用）
/// </summary>
public class UpperCaseDecorator : DecoratorBase<string>, IComponent
{
    public UpperCaseDecorator(IPlugin<string> plugin, string name = "大写转换装饰器") 
        : base(plugin, name)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public UpperCaseDecorator(IComponent component, string name = "大写转换装饰器") 
        : base(new ComponentToPluginAdapter(component), name)
    {
    }

    protected override string PostProcess(string output)
    {
        var result = output.ToUpper();
        Console.WriteLine($"[{DecoratorName}] 转换为大写: {output} => {result}");
        return result;
    }
}
