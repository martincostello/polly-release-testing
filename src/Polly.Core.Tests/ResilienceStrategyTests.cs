namespace Polly.Core.Tests;

public partial class ResilienceStrategyTests
{
    public static readonly CancellationToken CancellationToken = new CancellationTokenSource().Token;

    public class ExecuteParameters<T> : ExecuteParameters
    {
        public ExecuteParameters(Func<ResilienceStrategy, Task<T>> execute, T resultValue)
        {
            Execute = async strategy =>
            {
                var result = await execute(strategy);
                return result!;
            };

            AssertResult = result => result.Should().BeOfType<T>().And.Be(resultValue);
        }

        public ExecuteParameters(Func<ResilienceStrategy, ValueTask<T>> execute, T resultValue)
        {
            Execute = async strategy =>
            {
                var result = await execute(strategy);
                return result!;
            };

            AssertResult = result => result.Should().BeOfType<T>().And.Be(resultValue);
        }

        public ExecuteParameters(Func<ResilienceStrategy, T> execute, T resultValue)
        {
            Execute = strategy => new ValueTask<object>(execute(strategy)!);
            AssertResult = result => result.Should().BeOfType<T>().And.Be(resultValue);
        }
    }

    public class ExecuteParameters
    {
        public ExecuteParameters()
        {
        }

        public ExecuteParameters(Func<ResilienceStrategy, ValueTask> execute)
        {
            Execute = async r =>
            {
                await execute(r);
                return VoidResult.Instance;
            };

            AssertResult = r => r.Should().Be(VoidResult.Instance);
        }

        public ExecuteParameters(Func<ResilienceStrategy, Task> execute)
        {
            Execute = async r =>
            {
                await execute(r);
                return VoidResult.Instance;
            };

            AssertResult = r => r.Should().Be(VoidResult.Instance);
        }

        public ExecuteParameters(Action<ResilienceStrategy> execute)
        {
            Execute = r =>
            {
                execute(r);
                return new ValueTask<object>(VoidResult.Instance);
            };

            AssertResult = r => r.Should().Be(VoidResult.Instance);
        }

        public Func<ResilienceStrategy, ValueTask<object>> Execute { get; set; } = r => new ValueTask<object>(VoidResult.Instance);

        public Action<ResilienceContext> AssertContext { get; set; } = _ => { };

        public Action<object> AssertResult { get; set; } = _ => { };

        public Action<ResilienceContext> AssertContextAfter { get; set; } = _ => { };

        public string Caption { get; set; } = "unknown";

        public override string ToString() => Caption;
    }

    public static IEnumerable<object[]> ConvertExecuteParameters(Func<IEnumerable<ExecuteParameters>> parameters) => parameters().Select(p => new object[] { p }).ToArray();
}
