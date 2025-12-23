using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Plugins;

/// <summary>
/// 文本处理器组件 - 基础组件示例
/// 同时实现 IComponent（向后兼容）和 IPlugin<string>（新泛型接口）
/// </summary>
public class TextProcessorComponent : PluginBase<string>, IComponent
{
    public TextProcessorComponent(string name = "文本处理器") : base(name)
    {
    }

    public override string Execute(string input)
    {
        Console.WriteLine($"[{PluginName}] 处理输入: {input}");
        return input;
    }
}
