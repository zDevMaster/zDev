using System.Reflection;
using DecoratorPluginDemo.Configuration;
using DecoratorPluginDemo.Core;
using Newtonsoft.Json;

namespace DecoratorPluginDemo.Infrastructure;

/// <summary>
/// 泛型插件加载器 - 负责从JSON配置加载并构建装饰器链
/// </summary>
/// <typeparam name="T">插件接口类型，必须实现 IPlugin 接口</typeparam>
public class PluginLoader<T> where T : class
{
    private readonly string _configPath;

    public PluginLoader(string configPath = "plugins.json")
    {
        _configPath = configPath;
    }

    /// <summary>
    /// 从配置文件加载并构建插件
    /// </summary>
    public T LoadPlugin()
    {
        // 读取配置文件
        var config = LoadConfiguration();
        
        if (config.BaseComponent == null)
        {
            throw new InvalidOperationException("未配置基础组件");
        }

        // 创建基础插件
        var plugin = CreatePlugin(config.BaseComponent);
        
        // 应用装饰器
        if (config.Decorators != null && config.Decorators.Any())
        {
            plugin = ApplyDecorators(plugin, config.Decorators);
        }

        return plugin;
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
    /// 创建插件实例
    /// </summary>
    private T CreatePlugin(ComponentConfig config)
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
        
        if (instance is not T plugin)
        {
            throw new InvalidOperationException($"类型 {config.TypeName} 未实现 {typeof(T).Name} 接口");
        }

        return plugin;
    }

    /// <summary>
    /// 应用装饰器链
    /// </summary>
    private T ApplyDecorators(T basePlugin, List<DecoratorConfig> decoratorConfigs)
    {
        // 过滤启用的装饰器并按优先级排序
        var enabledDecorators = decoratorConfigs
            .Where(d => d.Enabled)
            .OrderBy(d => d.Priority)
            .ToList();

        var currentPlugin = basePlugin;

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

                // 选择最佳构造函数（优先选择第一个参数是 T 类型或其兼容类型的构造函数）
                var constructor = SelectBestConstructor(type, typeof(T));
                if (constructor == null)
                {
                    Console.WriteLine($"警告: 无法找到适合的构造函数 {decoratorConfig.TypeName}，已跳过");
                    continue;
                }

                // 装饰器的第一个参数必须是插件接口类型
                var parameters = new List<object> { currentPlugin };
                
                // 添加其他构造参数
                if (decoratorConfig.Parameters != null)
                {
                    var constructorParams = PrepareConstructorParametersForConstructor(constructor, decoratorConfig.Parameters, skipFirst: true);
                    parameters.AddRange(constructorParams);
                }

                var decorator = constructor.Invoke(parameters.ToArray());
                
                if (decorator is not T decoratorPlugin)
                {
                    Console.WriteLine($"警告: 类型 {decoratorConfig.TypeName} 未实现 {typeof(T).Name} 接口，已跳过");
                    continue;
                }

                currentPlugin = decoratorPlugin;
                Console.WriteLine($"已应用装饰器: {decoratorConfig.Name ?? decoratorConfig.TypeName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"警告: 应用装饰器 {decoratorConfig.Name} 时出错: {ex.Message}");
            }
        }

        return currentPlugin;
    }

    /// <summary>
    /// 选择最佳构造函数
    /// </summary>
    private ConstructorInfo? SelectBestConstructor(Type type, Type firstParamType)
    {
        var constructors = type.GetConstructors();
        
        // 优先选择第一个参数精确匹配 T 类型的构造函数
        var exactMatch = constructors.FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType == firstParamType;
        });

        if (exactMatch != null)
            return exactMatch;

        // 其次选择第一个参数是 T 的基类或接口的构造函数
        var compatibleMatch = constructors.FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length > 0 && parameters[0].ParameterType.IsAssignableFrom(firstParamType);
        });

        return compatibleMatch ?? constructors.FirstOrDefault();
    }

    /// <summary>
    /// 为指定构造函数准备参数
    /// </summary>
    private object[] PrepareConstructorParametersForConstructor(ConstructorInfo constructor, Dictionary<string, object>? configParameters, bool skipFirst = false)
    {
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
                var convertedValue = ConvertValue(value, param.ParameterType);
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
                var convertedValue = ConvertValue(value, param.ParameterType);
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

    /// <summary>
    /// 类型转换辅助方法
    /// </summary>
    private object? ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return null;

        // 处理 Newtonsoft.Json 的 JValue/JToken
        if (value is Newtonsoft.Json.Linq.JToken jToken)
        {
            return jToken.ToObject(targetType);
        }

        // 常规类型转换
        return Convert.ChangeType(value, targetType);
    }
}

/// <summary>
/// 字符串插件加载器 - 向后兼容原有 PluginLoader
/// </summary>
public class PluginLoader : PluginLoader<IPlugin<string>>
{
    public PluginLoader(string configPath = "plugins.json") : base(configPath)
    {
    }

    /// <summary>
    /// 从配置文件加载并构建组件（向后兼容）
    /// </summary>
    public IPlugin<string> LoadComponent() => LoadPlugin();
}

/// <summary>
/// IComponent 专用加载器 - 完全向后兼容
/// </summary>
public class ComponentLoader : PluginLoader<IComponent>
{
    public ComponentLoader(string configPath = "plugins.json") : base(configPath)
    {
    }

    /// <summary>
    /// 从配置文件加载并构建组件
    /// </summary>
    public IComponent LoadComponent() => LoadPlugin();
}
