using DecoratorPluginDemo.Core;

namespace DecoratorPluginDemo.Plugins;

/// <summary>
/// 数据验证器组件 - 另一个基础组件示例
/// 同时实现 IComponent（向后兼容）和 IPlugin<string>（新泛型接口）
/// </summary>
public class DataValidatorComponent : PluginBase<string>, IComponent
{
    private readonly int _minLength;

    public DataValidatorComponent(string name = "数据验证器", int minLength = 0) : base(name)
    {
        _minLength = minLength;
    }

    public override string Execute(string input)
    {
        Console.WriteLine($"[{PluginName}] 验证输入 (最小长度: {_minLength})");
        
        if (input.Length < _minLength)
        {
            throw new ArgumentException($"输入长度不足，最小长度为 {_minLength}");
        }
        
        return input;
    }
}
