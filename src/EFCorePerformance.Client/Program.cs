using BenchmarkDotNet.Running;

namespace EFCorePerformance.Client;

public class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
