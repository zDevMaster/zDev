using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 修剪装饰器 - 去除首尾空格（字符串专用）
/// </summary>
public class TrimDecorator : DecoratorBase<string>, IComponent
{
    public TrimDecorator(IPlugin<string> plugin, string name = "修剪装饰器") 
        : base(plugin, name)
    {
    }

    // 支持使用 IComponent 构造（向后兼容）
    public TrimDecorator(IComponent component, string name = "修剪装饰器") 
        : base(new ComponentToPluginAdapter(component), name)
    {
    }

    protected override string PreProcess(string input)
    {
        var result = input.Trim();
        if (result != input)
        {
            Console.WriteLine($"[{DecoratorName}] 修剪空格: '{input}' => '{result}'");
        }
        return result;
    }
}
