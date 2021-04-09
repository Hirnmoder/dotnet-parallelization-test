namespace PerformanceTests
{
    public interface ITest
    {
        public void Run(AutoStopwatch parent);
        public string Name { get; }
    }
}
