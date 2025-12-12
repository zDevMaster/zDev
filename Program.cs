using DecoratorPluginDemo.Infrastructure;

namespace DecoratorPluginDemo;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("装饰器模式插件自动注入系统 - 演示");
        Console.WriteLine("========================================\n");

        // 示例1: 使用完整配置
        Console.WriteLine("\n【示例1: 完整装饰器链】");
        Console.WriteLine("----------------------------------------");
        RunExample("plugins.json");

        // 示例2: 使用简单配置
        Console.WriteLine("\n\n【示例2: 简单装饰器链】");
        Console.WriteLine("----------------------------------------");
        RunExample("plugins-simple.json");

        // 示例3: 演示缓存功能
        Console.WriteLine("\n\n【示例3: 缓存功能演示】");
        Console.WriteLine("----------------------------------------");
        DemonstrateCaching();

        Console.WriteLine("\n\n========================================");
        Console.WriteLine("演示完成！");
        Console.WriteLine("========================================");
    }

    static void RunExample(string configFile)
    {
        try
        {
            // 创建插件加载器
            var loader = new PluginLoader(configFile);
            
            // 加载并构建组件（自动应用装饰器）
            Console.WriteLine($"\n正在从 {configFile} 加载组件...\n");
            var component = loader.LoadComponent();
            
            // 显示组件结构
            Console.WriteLine($"\n组件结构: {component.GetName()}\n");
            
            // 执行测试
            Console.WriteLine("执行测试:\n");
            var result = component.Execute("  hello world  ");
            
            Console.WriteLine($"\n最终结果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
    }

    static void DemonstrateCaching()
    {
        try
        {
            var loader = new PluginLoader("plugins.json");
            var component = loader.LoadComponent();
            
            Console.WriteLine("第一次调用 'test':");
            var result1 = component.Execute("test");
            
            Console.WriteLine("\n第二次调用 'test' (应该从缓存读取):");
            var result2 = component.Execute("test");
            
            Console.WriteLine("\n第一次调用 'another':");
            var result3 = component.Execute("another");
            
            Console.WriteLine("\n第三次调用 'test' (应该从缓存读取):");
            var result4 = component.Execute("test");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
