# C# 装饰器模式插件自动注入系统

一个功能完整的C#项目，演示如何使用JSON配置文件实现装饰器模式的插件自动注入。

## 🎯 项目简介

本项目实现了一个灵活的插件系统，通过JSON配置文件动态加载和组装装饰器链。支持：

- ✅ 装饰器模式的完整实现
- ✅ JSON配置驱动的插件加载
- ✅ 自动依赖注入
- ✅ 优先级控制
- ✅ 动态启用/禁用装饰器
- ✅ 多种内置装饰器（日志、缓存、性能监控等）

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
├── Configuration/     # 配置模型
├── Infrastructure/    # 插件加载器
├── Plugins/          # 示例插件
├── Decorators/       # 内置装饰器
├── Examples/         # 自定义示例
└── plugins.json      # 配置文件
```

## 📚 更多信息

完整文档、API说明和高级用法请参阅 [README-CN.md](README-CN.md)