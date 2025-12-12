# 装饰器模式插件自动注入系统

这是一个基于C#的装饰器模式实现，通过JSON配置文件实现插件的自动加载和注入。

## 📋 目录

- [功能特性](#功能特性)
- [项目结构](#项目结构)
- [快速开始](#快速开始)
- [核心概念](#核心概念)
- [配置说明](#配置说明)
- [自定义开发](#自定义开发)
- [示例](#示例)

## ✨ 功能特性

- 🔌 **插件化架构**: 基于接口的组件系统，易于扩展
- 🎨 **装饰器模式**: 动态添加功能，不修改原有代码
- ⚙️ **JSON配置**: 通过配置文件灵活控制装饰器链
- 🔄 **自动注入**: 自动加载和组装装饰器链
- 📊 **优先级控制**: 通过Priority字段控制装饰器应用顺序
- 🎛️ **启用/禁用**: 通过Enabled字段动态控制装饰器
- 🏗️ **依赖注入友好**: 支持构造函数参数注入

## 📁 项目结构

```
DecoratorPluginDemo/
├── Core/                          # 核心接口和基类
│   ├── IComponent.cs              # 组件接口
│   ├── ComponentBase.cs           # 组件基类
│   └── DecoratorBase.cs           # 装饰器基类
├── Configuration/                 # 配置模型
│   └── PluginConfig.cs            # 插件配置类
├── Infrastructure/                # 基础设施
│   └── PluginLoader.cs            # 插件加载器
├── Plugins/                       # 插件实现
│   ├── TextProcessorComponent.cs  # 文本处理器
│   └── DataValidatorComponent.cs  # 数据验证器
├── Decorators/                    # 装饰器实现
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

## 🎯 核心概念

### 1. IComponent 接口

所有组件和装饰器都实现此接口：

```csharp
public interface IComponent
{
    string Execute(string input);
    string GetName();
}
```

### 2. ComponentBase 基类

为具体组件提供基础实现：

```csharp
public abstract class ComponentBase : IComponent
{
    protected readonly string ComponentName;
    public abstract string Execute(string input);
    public virtual string GetName() => ComponentName;
}
```

### 3. DecoratorBase 基类

为装饰器提供装饰器模式的标准实现：

```csharp
public abstract class DecoratorBase : IComponent
{
    protected IComponent _wrappedComponent;
    
    protected virtual string PreProcess(string input);   // 前置处理
    protected virtual string PostProcess(string output); // 后置处理
}
```

### 4. PluginLoader 插件加载器

负责从JSON配置加载并自动组装装饰器链：

- 读取JSON配置文件
- 实例化基础组件
- 按优先级应用装饰器
- 处理参数注入

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
- **Parameters**: 构造函数参数（可选，第一个参数IComponent会自动注入）

### 示例配置

#### 完整配置 (plugins.json)

```json
{
  "BaseComponent": {
    "TypeName": "DecoratorPluginDemo.Plugins.TextProcessorComponent, DecoratorPluginDemo",
    "Name": "文本处理器",
    "Parameters": {
      "name": "核心文本处理器"
    }
  },
  "Decorators": [
    {
      "TypeName": "DecoratorPluginDemo.Decorators.TrimDecorator, DecoratorPluginDemo",
      "Name": "修剪装饰器",
      "Enabled": true,
      "Priority": 10
    },
    {
      "TypeName": "DecoratorPluginDemo.Decorators.CacheDecorator, DecoratorPluginDemo",
      "Name": "缓存装饰器",
      "Enabled": true,
      "Priority": 20,
      "Parameters": {
        "name": "智能缓存",
        "maxCacheSize": 50
      }
    }
  ]
}
```

## 🛠️ 自定义开发

### 创建自定义组件

```csharp
using DecoratorPluginDemo.Core;

namespace YourNamespace;

public class CustomComponent : ComponentBase
{
    public CustomComponent(string name = "自定义组件") : base(name)
    {
    }

    public override string Execute(string input)
    {
        // 你的处理逻辑
        return input;
    }
}
```

### 创建自定义装饰器

```csharp
using DecoratorPluginDemo.Core;

namespace YourNamespace;

public class CustomDecorator : DecoratorBase
{
    public CustomDecorator(IComponent component, string name = "自定义装饰器") 
        : base(component, name)
    {
    }

    protected override string PreProcess(string input)
    {
        // 前置处理逻辑
        return input;
    }

    protected override string PostProcess(string output)
    {
        // 后置处理逻辑
        return output;
    }
}
```

### 在配置文件中使用

```json
{
  "BaseComponent": {
    "TypeName": "YourNamespace.CustomComponent, YourAssembly",
    "Name": "我的组件"
  },
  "Decorators": [
    {
      "TypeName": "YourNamespace.CustomDecorator, YourAssembly",
      "Name": "我的装饰器",
      "Enabled": true,
      "Priority": 10
    }
  ]
}
```

## 📚 示例

### 示例1: 基本使用

```csharp
var loader = new PluginLoader("plugins.json");
var component = loader.LoadComponent();
var result = component.Execute("hello world");
Console.WriteLine(result);
```

### 示例2: 动态切换配置

```csharp
// 使用配置A
var loader1 = new PluginLoader("config-a.json");
var component1 = loader1.LoadComponent();

// 使用配置B
var loader2 = new PluginLoader("config-b.json");
var component2 = loader2.LoadComponent();
```

### 示例3: 查看装饰器链

```csharp
var loader = new PluginLoader("plugins.json");
var component = loader.LoadComponent();
Console.WriteLine($"组件结构: {component.GetName()}");
// 输出: 大写转换器(详细日志(性能分析器(智能缓存(空格修剪器(核心文本处理器)))))
```

## 🎨 内置装饰器

### LoggingDecorator（日志装饰器）
记录输入输出和执行流程，可选时间戳。

### UpperCaseDecorator（大写转换装饰器）
将输出转换为大写。

### TrimDecorator（修剪装饰器）
去除输入的首尾空格。

### CacheDecorator（缓存装饰器）
缓存执行结果，避免重复计算。支持配置最大缓存大小。

### PerformanceDecorator（性能监控装饰器）
测量并显示执行时间。

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
2. **策略模式**: 通过配置选择不同的装饰器组合
3. **工厂模式**: PluginLoader作为组件工厂
4. **依赖注入**: 通过构造函数注入依赖

## 🤝 贡献

欢迎贡献新的装饰器实现或改进建议！

## 📄 许可证

MIT License

## 👨‍💻 作者

装饰器模式插件系统演示项目

---

**享受使用装饰器模式的乐趣！** 🎉
