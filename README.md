# C# 泛型装饰器模式插件系统

一个功能完整的C#项目，演示如何使用泛型和JSON配置文件实现通用装饰器模式的插件自动注入。

## 🎯 项目简介

本项目实现了一个灵活的**泛型插件系统**，通过JSON配置文件动态加载和组装装饰器链。支持：

- ✅ **泛型装饰器模式** - 支持任意输入/输出类型
- ✅ **通用插件加载器** - `PluginLoader<T>` 可加载任意类型插件
- ✅ JSON配置驱动的插件加载
- ✅ 自动依赖注入
- ✅ 优先级控制
- ✅ 动态启用/禁用装饰器
- ✅ 多种内置装饰器（日志、缓存、性能监控等）
- ✅ **向后兼容** - 保留 `IComponent` 接口兼容旧代码

## 📖 文档

详细文档请查看 [README-CN.md](README-CN.md)

## 🚀 快速开始

```bash
# 还原依赖
dotnet restore

# 编译项目
dotnet build

# 运行示例
dotnet run
```

## 🔧 泛型架构

### 核心接口

```csharp
// 泛型插件接口 - 支持不同的输入/输出类型
public interface IPlugin<TInput, TOutput>
{
    TOutput Execute(TInput input);
    string GetName();
}

// 简化版 - 输入输出类型相同
public interface IPlugin<T> : IPlugin<T, T> { }

// 字符串插件（向后兼容 IComponent）
public interface IStringPlugin : IPlugin<string> { }
```

### 通用插件加载器

```csharp
// 加载任意类型的插件
var loader = new PluginLoader<IPlugin<string>>("plugins.json");
var plugin = loader.LoadPlugin();

// 也可以加载其他类型
var mathLoader = new PluginLoader<IPlugin<int, int>>("math-plugins.json");
```

### 代码方式构建装饰器链

```csharp
// 字符串处理链
IPlugin<string> plugin = new TextProcessorComponent();
plugin = new LoggingDecorator<string>(plugin);
plugin = new CacheDecorator<string>(plugin);

// 数学处理链（自定义类型）
IPlugin<int, int> mathPlugin = new MathProcessor();
mathPlugin = new LoggingDecorator<int, int>(mathPlugin);
mathPlugin = new PerformanceDecorator<int, int>(mathPlugin);
```

## 📝 示例配置

```json
{
  "BaseComponent": {
    "TypeName": "DecoratorPluginDemo.Plugins.TextProcessorComponent, DecoratorPluginDemo",
    "Name": "文本处理器"
  },
  "Decorators": [
    {
      "TypeName": "DecoratorPluginDemo.Decorators.LoggingDecorator, DecoratorPluginDemo",
      "Name": "日志装饰器",
      "Enabled": true,
      "Priority": 10
    }
  ]
}
```

## 🏗️ 项目结构

```
├── Core/              # 核心接口和基类
│   ├── IPlugin.cs     # 泛型插件接口
│   ├── PluginBase.cs  # 泛型插件基类
│   ├── DecoratorBase.cs # 泛型装饰器基类
│   ├── IComponent.cs  # 向后兼容接口
│   └── ComponentBase.cs # 向后兼容基类
├── Configuration/     # 配置模型
├── Infrastructure/    # 插件加载器
│   └── PluginLoader.cs # 泛型插件加载器 PluginLoader<T>
├── Plugins/          # 示例插件
├── Decorators/       # 内置装饰器（支持泛型）
├── Examples/         # 自定义示例
└── plugins.json      # 配置文件
```

## 🔄 泛型装饰器层次

```
DecoratorBase<TInput, TOutput>     # 完整泛型（不同输入/输出类型）
    └── DecoratorBase<T>           # 简化泛型（相同输入/输出类型）
        └── StringDecoratorBase    # 字符串专用（向后兼容）
```

## 📚 更多信息

完整文档、API说明和高级用法请参阅 [README-CN.md](README-CN.md)
