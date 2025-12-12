using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Decorators;

/// <summary>
/// 修剪装饰器 - 去除首尾空格
/// </summary>
public class TrimDecorator : DecoratorBase
{
    public TrimDecorator(IComponent component, string name = "修剪装饰器") 
        : base(component, name)
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
