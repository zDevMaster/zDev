# 使用指南

## 快速开始

### 1. 运行项目

```bash
dotnet run
```

程序将自动执行三个示例：
- 示例1：完整装饰器链演示
- 示例2：简单装饰器链演示
- 示例3：缓存功能演示

### 2. 自定义配置

创建一个新的JSON配置文件，例如 `my-config.json`：

```json
{
  "BaseComponent": {
    "TypeName": "DecoratorPluginDemo.Plugins.TextProcessorComponent, DecoratorPluginDemo",
    "Name": "我的处理器"
  },
  "Decorators": [
    {
      "TypeName": "DecoratorPluginDemo.Decorators.LoggingDecorator, DecoratorPluginDemo",
      "Name": "日志",
      "Enabled": true,
      "Priority": 10,
      "Parameters": {
        "name": "我的日志",
        "logTimestamp": true
      }
    }
  ]
}
```

### 3. 在代码中使用

```csharp
using DecoratorPluginDemo.Infrastructure;

// 加载配置
var loader = new PluginLoader("my-config.json");

// 自动构建装饰器链
var component = loader.LoadComponent();

// 使用组件
var result = component.Execute("输入数据");
Console.WriteLine(result);
```

## 配置详解

### TypeName 格式

TypeName必须包含完整的类型信息：

```
命名空间.类名, 程序集名
```

例如：
```
DecoratorPluginDemo.Decorators.LoggingDecorator, DecoratorPluginDemo
```

### Priority（优先级）

数字越小，装饰器越先应用。例如：

```json
{
  "Decorators": [
    {"Priority": 10, "Name": "第一个"},  // 最先应用
    {"Priority": 20, "Name": "第二个"},
    {"Priority": 30, "Name": "第三个"}   // 最后应用
  ]
}
```

实际执行顺序（从外到内）：
```
第三个(第二个(第一个(基础组件)))
```

### Enabled（启用开关）

通过 `Enabled` 字段控制装饰器是否应用：

```json
{
  "TypeName": "...",
  "Enabled": false  // 禁用此装饰器
}
```

### Parameters（参数）

传递给构造函数的参数（JSON格式）：

```json
{
  "Parameters": {
    "name": "我的装饰器",
    "maxCacheSize": 100,
    "logTimestamp": true,
    "timeout": 5000
  }
}
```

**注意**：装饰器的第一个参数（IComponent）会自动注入，不需要在Parameters中指定。

## 内置装饰器

### LoggingDecorator（日志装饰器）

记录输入输出和执行流程。

**参数**：
- `name` (string): 装饰器名称
- `logTimestamp` (bool): 是否显示时间戳，默认true

**示例**：
```json
{
  "TypeName": "DecoratorPluginDemo.Decorators.LoggingDecorator, DecoratorPluginDemo",
  "Parameters": {
    "name": "详细日志",
    "logTimestamp": true
  }
}
```

### UpperCaseDecorator（大写转换）

将输出转换为大写。

**参数**：
- `name` (string): 装饰器名称

**示例**：
```json
{
  "TypeName": "DecoratorPluginDemo.Decorators.UpperCaseDecorator, DecoratorPluginDemo",
  "Parameters": {
    "name": "大写转换"
  }
}
```

### TrimDecorator（修剪装饰器）

去除输入的首尾空格。

**参数**：
- `name` (string): 装饰器名称

**示例**：
```json
{
  "TypeName": "DecoratorPluginDemo.Decorators.TrimDecorator, DecoratorPluginDemo",
  "Parameters": {
    "name": "空格修剪"
  }
}
```

### CacheDecorator（缓存装饰器）

缓存执行结果，避免重复计算。

**参数**：
- `name` (string): 装饰器名称
- `maxCacheSize` (int): 最大缓存条目数，默认100

**示例**：
```json
{
  "TypeName": "DecoratorPluginDemo.Decorators.CacheDecorator, DecoratorPluginDemo",
  "Parameters": {
    "name": "智能缓存",
    "maxCacheSize": 50
  }
}
```

### PerformanceDecorator（性能监控）

测量并显示执行时间。

**参数**：
- `name` (string): 装饰器名称

**示例**：
```json
{
  "TypeName": "DecoratorPluginDemo.Decorators.PerformanceDecorator, DecoratorPluginDemo",
  "Parameters": {
    "name": "性能分析"
  }
}
```

## 创建自定义组件

### 1. 继承 ComponentBase

```csharp
using DecoratorPluginDemo.Core;

namespace MyNamespace;

public class MyComponent : ComponentBase
{
    public MyComponent(string name = "我的组件") : base(name)
    {
    }

    public override string Execute(string input)
    {
        // 你的处理逻辑
        return input.ToLower();
    }
}
```

### 2. 在配置中使用

```json
{
  "BaseComponent": {
    "TypeName": "MyNamespace.MyComponent, YourAssembly",
    "Name": "自定义组件"
  }
}
```

## 创建自定义装饰器

### 1. 继承 DecoratorBase

```csharp
using DecoratorPluginDemo.Core;

namespace MyNamespace;

public class MyDecorator : DecoratorBase
{
    private readonly string _prefix;

    // 第一个参数必须是 IComponent
    public MyDecorator(
        IComponent component, 
        string name = "我的装饰器",
        string prefix = ">>>") 
        : base(component, name)
    {
        _prefix = prefix;
    }

    protected override string PostProcess(string output)
    {
        return $"{_prefix} {output}";
    }
}
```

### 2. 在配置中使用

```json
{
  "Decorators": [
    {
      "TypeName": "MyNamespace.MyDecorator, YourAssembly",
      "Name": "前缀装饰器",
      "Enabled": true,
      "Priority": 10,
      "Parameters": {
        "name": "前缀添加器",
        "prefix": "==="
      }
    }
  ]
}
```

## 常见场景

### 场景1：数据验证 + 日志

```json
{
  "BaseComponent": {
    "TypeName": "DecoratorPluginDemo.Plugins.DataValidatorComponent, DecoratorPluginDemo",
    "Parameters": {
      "name": "验证器",
      "minLength": 5
    }
  },
  "Decorators": [
    {
      "TypeName": "DecoratorPluginDemo.Decorators.TrimDecorator, DecoratorPluginDemo",
      "Priority": 10
    },
    {
      "TypeName": "DecoratorPluginDemo.Decorators.LoggingDecorator, DecoratorPluginDemo",
      "Priority": 20
    }
  ]
}
```

### 场景2：性能优化（缓存 + 监控）

```json
{
  "Decorators": [
    {
      "TypeName": "DecoratorPluginDemo.Decorators.CacheDecorator, DecoratorPluginDemo",
      "Priority": 10,
      "Parameters": {
        "maxCacheSize": 1000
      }
    },
    {
      "TypeName": "DecoratorPluginDemo.Decorators.PerformanceDecorator, DecoratorPluginDemo",
      "Priority": 20
    }
  ]
}
```

### 场景3：完整处理流程

```json
{
  "Decorators": [
    {"TypeName": "...TrimDecorator...", "Priority": 10},
    {"TypeName": "...CacheDecorator...", "Priority": 20},
    {"TypeName": "...PerformanceDecorator...", "Priority": 30},
    {"TypeName": "...LoggingDecorator...", "Priority": 40},
    {"TypeName": "...UpperCaseDecorator...", "Priority": 50}
  ]
}
```

执行流程：
1. 输入被Trim清理
2. 检查缓存
3. 测量性能
4. 记录日志
5. 转换为大写
6. 返回结果

## 调试技巧

### 1. 查看装饰器链结构

```csharp
var component = loader.LoadComponent();
Console.WriteLine(component.GetName());
```

输出示例：
```
大写转换(日志(缓存(修剪(基础组件))))
```

### 2. 禁用特定装饰器

临时禁用某个装饰器进行调试：

```json
{
  "TypeName": "...",
  "Enabled": false  // 暂时禁用
}
```

### 3. 调整优先级

修改Priority来改变装饰器顺序：

```json
{
  "Decorators": [
    {"Name": "A", "Priority": 20},  // 修改优先级测试不同顺序
    {"Name": "B", "Priority": 10}
  ]
}
```

## 最佳实践

1. **按功能分层**：将装饰器按功能分组（验证、缓存、日志等）
2. **合理设置优先级**：
   - 10-19: 输入清理（Trim等）
   - 20-29: 缓存
   - 30-39: 性能监控
   - 40-49: 日志
   - 50+: 输出转换
3. **保持装饰器单一职责**：每个装饰器只做一件事
4. **使用有意义的名称**：便于调试和维护
5. **合理配置缓存大小**：避免内存溢出

## 故障排除

### 错误：类型加载失败

**问题**：`无法加载类型: XXX`

**解决**：检查TypeName格式是否正确，必须包含命名空间和程序集名。

### 错误：参数类型不匹配

**问题**：构造函数参数类型错误

**解决**：确保Parameters中的参数类型与构造函数定义一致。

### 装饰器未生效

**问题**：装饰器没有按预期工作

**解决**：
1. 检查 `Enabled` 是否为 `true`
2. 检查 `Priority` 顺序是否正确
3. 查看控制台输出的装饰器链结构

## 更多示例

查看项目中的示例配置文件：
- `plugins.json` - 完整功能演示
- `plugins-simple.json` - 简单示例
- `plugins-validation.json` - 数据验证示例
- `Examples/plugins-custom.json` - 自定义装饰器示例
