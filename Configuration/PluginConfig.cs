namespace DecoratorPluginDemo.Configuration;

/// <summary>
/// 插件配置模型
/// </summary>
public class PluginConfig
{
    /// <summary>
    /// 基础组件配置
    /// </summary>
    public ComponentConfig? BaseComponent { get; set; }
    
    /// <summary>
    /// 装饰器列表（按顺序应用）
    /// </summary>
    public List<DecoratorConfig>? Decorators { get; set; }
}

/// <summary>
/// 组件配置
/// </summary>
public class ComponentConfig
{
    /// <summary>
    /// 类型全名（包含命名空间）
    /// </summary>
    public string? TypeName { get; set; }
    
    /// <summary>
    /// 组件名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 构造参数
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// 装饰器配置
/// </summary>
public class DecoratorConfig
{
    /// <summary>
    /// 装饰器类型全名
    /// </summary>
    public string? TypeName { get; set; }
    
    /// <summary>
    /// 装饰器名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// 优先级（数字越小优先级越高）
    /// </summary>
    public int Priority { get; set; } = 100;
    
    /// <summary>
    /// 构造参数
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}
