using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Plugins;

/// <summary>
/// 文本处理器组件 - 基础组件示例
/// </summary>
public class TextProcessorComponent : ComponentBase
{
    public TextProcessorComponent(string name = "文本处理器") : base(name)
    {
    }

    public override string Execute(string input)
    {
        Console.WriteLine($"[{ComponentName}] 处理输入: {input}");
        return input;
    }
}
