using System.Reflection;
using DecoratorPluginDemo.Configuration;
using DecoratorPluginDemo.Core;
using Newtonsoft.Json;

namespace DecoratorPluginDemo.Infrastructure;

/// <summary>
/// 插件加载器 - 负责从JSON配置加载并构建装饰器链
/// </summary>
public class PluginLoader
{
    private readonly string _configPath;

    public PluginLoader(string configPath = "plugins.json")
    {
        _configPath = configPath;
    }

    /// <summary>
    /// 从配置文件加载并构建组件
    /// </summary>
    public IComponent LoadComponent()
    {
        // 读取配置文件
        var config = LoadConfiguration();
        
        if (config.BaseComponent == null)
        {
            throw new InvalidOperationException("未配置基础组件");
        }

        // 创建基础组件
        var component = CreateComponent(config.BaseComponent);
        
        // 应用装饰器
        if (config.Decorators != null && config.Decorators.Any())
        {
            component = ApplyDecorators(component, config.Decorators);
        }

        return component;
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private PluginConfig LoadConfiguration()
    {
        if (!File.Exists(_configPath))
        {
            throw new FileNotFoundException($"配置文件不存在: {_configPath}");
        }

        var json = File.ReadAllText(_configPath);
        var config = JsonConvert.DeserializeObject<PluginConfig>(json);

        if (config == null)
        {
            throw new InvalidOperationException("配置文件格式错误");
        }

        return config;
    }

    /// <summary>
    /// 创建组件实例
    /// </summary>
    private IComponent CreateComponent(ComponentConfig config)
    {
        if (string.IsNullOrEmpty(config.TypeName))
        {
            throw new ArgumentException("组件类型名称不能为空");
        }

        var type = Type.GetType(config.TypeName);
        if (type == null)
        {
            throw new TypeLoadException($"无法加载类型: {config.TypeName}");
        }

        // 准备构造参数
        var parameters = PrepareConstructorParameters(type, config.Parameters);
        
        var instance = Activator.CreateInstance(type, parameters);
        
        if (instance is not IComponent component)
        {
            throw new InvalidOperationException($"类型 {config.TypeName} 未实现 IComponent 接口");
        }

        return component;
    }

    /// <summary>
    /// 应用装饰器链
    /// </summary>
    private IComponent ApplyDecorators(IComponent baseComponent, List<DecoratorConfig> decoratorConfigs)
    {
        // 过滤启用的装饰器并按优先级排序
        var enabledDecorators = decoratorConfigs
            .Where(d => d.Enabled)
            .OrderBy(d => d.Priority)
            .ToList();

        var currentComponent = baseComponent;

        foreach (var decoratorConfig in enabledDecorators)
        {
            if (string.IsNullOrEmpty(decoratorConfig.TypeName))
            {
                Console.WriteLine($"警告: 装饰器配置缺少类型名称，已跳过");
                continue;
            }

            try
            {
                var type = Type.GetType(decoratorConfig.TypeName);
                if (type == null)
                {
                    Console.WriteLine($"警告: 无法加载装饰器类型 {decoratorConfig.TypeName}，已跳过");
                    continue;
                }

                // 装饰器的第一个参数必须是 IComponent
                var parameters = new List<object> { currentComponent };
                
                // 添加其他构造参数
                if (decoratorConfig.Parameters != null)
                {
                    var constructorParams = PrepareConstructorParameters(type, decoratorConfig.Parameters, skipFirst: true);
                    parameters.AddRange(constructorParams);
                }

                var decorator = Activator.CreateInstance(type, parameters.ToArray());
                
                if (decorator is not IComponent decoratorComponent)
                {
                    Console.WriteLine($"警告: 类型 {decoratorConfig.TypeName} 未实现 IComponent 接口，已跳过");
                    continue;
                }

                currentComponent = decoratorComponent;
                Console.WriteLine($"已应用装饰器: {decoratorConfig.Name ?? decoratorConfig.TypeName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"警告: 应用装饰器 {decoratorConfig.Name} 时出错: {ex.Message}");
            }
        }

        return currentComponent;
    }

    /// <summary>
    /// 准备构造函数参数
    /// </summary>
    private object[] PrepareConstructorParameters(Type type, Dictionary<string, object>? configParameters, bool skipFirst = false)
    {
        var constructors = type.GetConstructors();
        if (!constructors.Any())
        {
            return Array.Empty<object>();
        }

        // 选择第一个构造函数
        var constructor = constructors[0];
        var parameters = constructor.GetParameters();

        if (skipFirst)
        {
            parameters = parameters.Skip(1).ToArray();
        }

        var args = new List<object>();

        foreach (var param in parameters)
        {
            if (configParameters != null && configParameters.TryGetValue(param.Name ?? "", out var value))
            {
                // 类型转换
                var convertedValue = Convert.ChangeType(value, param.ParameterType);
                args.Add(convertedValue!);
            }
            else if (param.HasDefaultValue)
            {
                args.Add(param.DefaultValue!);
            }
            else if (param.ParameterType.IsValueType)
            {
                args.Add(Activator.CreateInstance(param.ParameterType)!);
            }
            else
            {
                args.Add(null!);
            }
        }

        return args.ToArray();
    }
}
