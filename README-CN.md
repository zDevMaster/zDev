# 泛型装饰器模式插件系统

这是一个基于C#的**泛型装饰器模式**实现，通过JSON配置文件实现插件的自动加载和注入。

## 📋 目录

- [功能特性](#功能特性)
- [项目结构](#项目结构)
- [快速开始](#快速开始)
- [泛型架构](#泛型架构)
- [核心概念](#核心概念)
- [配置说明](#配置说明)
- [自定义开发](#自定义开发)
- [示例](#示例)

## ✨ 功能特性

- 🔌 **泛型插件架构**: 支持任意输入/输出类型的组件
- 🎨 **泛型装饰器模式**: 动态添加功能，支持不同类型参数
- 🔧 **通用插件加载器**: `PluginLoader<T>` 可加载任意类型插件
- ⚙️ **JSON配置**: 通过配置文件灵活控制装饰器链
- 🔄 **自动注入**: 自动加载和组装装饰器链
- 📊 **优先级控制**: 通过Priority字段控制装饰器应用顺序
- 🎛️ **启用/禁用**: 通过Enabled字段动态控制装饰器
- 🏗️ **依赖注入友好**: 支持构造函数参数注入
- ✅ **向后兼容**: 保留 `IComponent` 接口兼容旧代码

## 📁 项目结构

```
DecoratorPluginDemo/
├── Core/                          # 核心接口和基类
│   ├── IPlugin.cs                 # 泛型插件接口
│   ├── PluginBase.cs              # 泛型插件基类
│   ├── DecoratorBase.cs           # 泛型装饰器基类
│   ├── IComponent.cs              # 组件接口（向后兼容）
│   └── ComponentBase.cs           # 组件基类（向后兼容）
├── Configuration/                 # 配置模型
│   └── PluginConfig.cs            # 插件配置类
├── Infrastructure/                # 基础设施
│   └── PluginLoader.cs            # 泛型插件加载器
├── Plugins/                       # 插件实现
│   ├── TextProcessorComponent.cs  # 文本处理器
│   └── DataValidatorComponent.cs  # 数据验证器
├── Decorators/                    # 装饰器实现（支持泛型）
│   ├── LoggingDecorator.cs        # 日志装饰器
│   ├── UpperCaseDecorator.cs      # 大写转换装饰器
│   ├── TrimDecorator.cs           # 修剪装饰器
│   ├── CacheDecorator.cs          # 缓存装饰器
│   └── PerformanceDecorator.cs    # 性能监控装饰器
├── plugins.json                   # 主配置文件
├── plugins-simple.json            # 简单配置示例
└── Program.cs                     # 主程序
```

## 🚀 快速开始

### 1. 环境要求

- .NET 8.0 或更高版本
- C# 12

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 编译项目

```bash
dotnet build
```

### 4. 运行项目

```bash
dotnet run
```

## 🔧 泛型架构

### 接口层次

```
IPlugin<TInput, TOutput>          # 完整泛型接口（不同输入/输出类型）
    └── IPlugin<T>                # 简化泛型接口（相同输入/输出类型）
        └── IStringPlugin         # 字符串插件接口
            └── IComponent        # 向后兼容接口
```

### 基类层次

```
PluginBase<TInput, TOutput>       # 完整泛型基类
    └── PluginBase<T>             # 简化泛型基类
        └── StringPluginBase      # 字符串插件基类
            └── ComponentBase     # 向后兼容基类

DecoratorBase<TInput, TOutput>    # 完整泛型装饰器基类
    └── DecoratorBase<T>          # 简化泛型装饰器基类
        └── StringDecoratorBase   # 字符串装饰器基类
```

### 插件加载器

```
PluginLoader<T>                   # 泛型插件加载器
    └── PluginLoader              # 字符串插件加载器（简化用法）
    └── ComponentLoader           # IComponent 加载器（向后兼容）
```

## 🎯 核心概念

### 1. IPlugin<TInput, TOutput> 泛型接口

支持任意输入/输出类型：

```csharp
public interface IPlugin<TInput, TOutput>
{
    TOutput Execute(TInput input);
    string GetName();
}

// 简化版 - 输入输出类型相同
public interface IPlugin<T> : IPlugin<T, T> { }
```

### 2. PluginBase<T> 泛型基类

为具体组件提供基础实现：

```csharp
public abstract class PluginBase<TInput, TOutput> : IPlugin<TInput, TOutput>
{
    protected readonly string PluginName;
    public abstract TOutput Execute(TInput input);
    public virtual string GetName() => PluginName;
}
```

### 3. DecoratorBase<T> 泛型装饰器基类

为装饰器提供装饰器模式的标准实现：

```csharp
public abstract class DecoratorBase<TInput, TOutput> : IPlugin<TInput, TOutput>
{
    protected IPlugin<TInput, TOutput> _wrappedPlugin;
    
    protected virtual TInput PreProcess(TInput input);    // 前置处理
    protected virtual TOutput PostProcess(TOutput output); // 后置处理
}
```

### 4. PluginLoader<T> 泛型插件加载器

负责从JSON配置加载并自动组装装饰器链：

```csharp
// 加载任意类型的插件
var loader = new PluginLoader<IPlugin<string>>("plugins.json");
var plugin = loader.LoadPlugin();

// 也可以加载其他类型
var mathLoader = new PluginLoader<IPlugin<int, int>>("math-plugins.json");
var mathPlugin = mathLoader.LoadPlugin();
```

## ⚙️ 配置说明

### JSON配置结构

```json
{
  "BaseComponent": {
    "TypeName": "完整类型名称，包含命名空间和程序集",
    "Name": "组件名称",
    "Parameters": {
      "参数名": "参数值"
    }
  },
  "Decorators": [
    {
      "TypeName": "装饰器完整类型名称",
      "Name": "装饰器名称",
      "Enabled": true,
      "Priority": 10,
      "Parameters": {
        "参数名": "参数值"
      }
    }
  ]
}
```

### 配置说明

#### BaseComponent（基础组件）

- **TypeName**: 类型全名，格式为 `命名空间.类名, 程序集名`
- **Name**: 组件显示名称
- **Parameters**: 构造函数参数（可选）

#### Decorators（装饰器数组）

- **TypeName**: 装饰器类型全名
- **Name**: 装饰器显示名称
- **Enabled**: 是否启用（true/false）
- **Priority**: 优先级，数字越小越先应用
- **Parameters**: 构造函数参数（可选，第一个参数IPlugin会自动注入）

## 🛠️ 自定义开发

### 创建自定义泛型组件

```csharp
using DecoratorPluginDemo.Core;

namespace YourNamespace;

// 处理整数的组件
public class MathProcessor : PluginBase<int, int>
{
    public MathProcessor(string name = "数学处理器") : base(name)
    {
    }

    public override int Execute(int input)
    {
        return input * 2 + 10;
    }
}

// 字符串组件（兼容旧接口）
public class CustomComponent : PluginBase<string>, IComponent
{
    public CustomComponent(string name = "自定义组件") : base(name)
    {
    }

    public override string Execute(string input)
    {
        return input;
    }
}
```

### 创建自定义泛型装饰器

```csharp
using DecoratorPluginDemo.Core;

namespace YourNamespace;

// 泛型装饰器 - 支持任意类型
public class ValidationDecorator<T> : DecoratorBase<T>
{
    private readonly Func<T, bool> _validator;

    public ValidationDecorator(IPlugin<T> plugin, Func<T, bool> validator, string name = "验证装饰器") 
        : base(plugin, name)
    {
        _validator = validator;
    }

    protected override T PreProcess(T input)
    {
        if (!_validator(input))
        {
            throw new ArgumentException("验证失败");
        }
        return input;
    }
}

// 字符串装饰器（兼容旧接口）
public class CustomDecorator : DecoratorBase<string>, IComponent
{
    public CustomDecorator(IPlugin<string> plugin, string name = "自定义装饰器") 
        : base(plugin, name)
    {
    }

    protected override string PreProcess(string input)
    {
        return input;
    }

    protected override string PostProcess(string output)
    {
        return output;
    }
}
```

## 📚 示例

### 示例1: 使用泛型插件加载器

```csharp
// 加载字符串处理插件
var loader = new PluginLoader<IPlugin<string>>("plugins.json");
var plugin = loader.LoadPlugin();
var result = plugin.Execute("hello world");
Console.WriteLine(result);
```

### 示例2: 代码方式构建泛型装饰器链

```csharp
// 字符串处理链
IPlugin<string> plugin = new TextProcessorComponent();
plugin = new LoggingDecorator<string>(plugin);
plugin = new CacheDecorator<string>(plugin);
plugin = new PerformanceDecorator<string>(plugin);

var result = plugin.Execute("泛型装饰器测试");
```

### 示例3: 自定义类型泛型插件

```csharp
// 数学处理链
IPlugin<int, int> mathPlugin = new MathProcessor();
mathPlugin = new LoggingDecorator<int, int>(mathPlugin);
mathPlugin = new PerformanceDecorator<int, int>(mathPlugin);

var result = mathPlugin.Execute(42); // 输出: 94
```

### 示例4: 查看装饰器链结构

```csharp
var loader = new PluginLoader<IPlugin<string>>("plugins.json");
var plugin = loader.LoadPlugin();
Console.WriteLine($"插件结构: {plugin.GetName()}");
// 输出: 大写转换器(详细日志(性能分析器(智能缓存(空格修剪器(核心文本处理器)))))
```

## 🎨 内置装饰器

所有内置装饰器都支持泛型版本：

### LoggingDecorator<T>（日志装饰器）
记录输入输出和执行流程，可选时间戳。

### CacheDecorator<T>（缓存装饰器）
缓存执行结果，避免重复计算。支持配置最大缓存大小。

### PerformanceDecorator<T>（性能监控装饰器）
测量并显示执行时间。

### TrimDecorator（修剪装饰器）
去除输入的首尾空格。（字符串专用）

### UpperCaseDecorator（大写转换装饰器）
将输出转换为大写。（字符串专用）

## 🔧 高级特性

### 优先级控制

装饰器按Priority从小到大应用：

```json
{
  "Decorators": [
    {"Priority": 10, "Name": "第一个应用"},
    {"Priority": 20, "Name": "第二个应用"},
    {"Priority": 30, "Name": "第三个应用"}
  ]
}
```

### 启用/禁用装饰器

通过Enabled字段控制：

```json
{
  "Decorators": [
    {"Enabled": true, "Name": "启用的装饰器"},
    {"Enabled": false, "Name": "禁用的装饰器"}
  ]
}
```

### 参数注入

支持通过Parameters注入构造函数参数：

```json
{
  "TypeName": "Namespace.MyDecorator, Assembly",
  "Parameters": {
    "timeout": 5000,
    "retryCount": 3,
    "enableDebug": true
  }
}
```

## 📝 设计模式

本项目演示了以下设计模式：

1. **装饰器模式**: 动态地给对象添加职责
2. **泛型编程**: 类型安全的可复用组件
3. **策略模式**: 通过配置选择不同的装饰器组合
4. **工厂模式**: PluginLoader作为组件工厂
5. **依赖注入**: 通过构造函数注入依赖
6. **适配器模式**: 兼容旧接口

## 🔄 向后兼容

新的泛型架构完全向后兼容旧代码：

```csharp
// 旧代码仍然可以正常工作
var loader = new PluginLoader("plugins.json");
IPlugin<string> component = loader.LoadComponent();

// 或者使用 ComponentLoader
var componentLoader = new ComponentLoader("plugins.json");
IComponent component = componentLoader.LoadComponent();
```

## 🤝 贡献

欢迎贡献新的装饰器实现或改进建议！

## 📄 许可证

MIT License

## 👨‍💻 作者

泛型装饰器模式插件系统演示项目

---

**享受使用泛型装饰器模式的乐趣！** 🎉
