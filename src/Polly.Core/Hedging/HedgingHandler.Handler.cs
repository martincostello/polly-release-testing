namespace Polly.Hedging;

internal partial class HedgingHandler
{
    internal sealed class Handler
    {
        private readonly Dictionary<Type, object> _predicates;
        private readonly Dictionary<Type, object> _generators;

        internal Handler(Dictionary<Type, object> predicates, Dictionary<Type, object> generators)
        {
            _predicates = predicates;
            _generators = generators;
        }

        public bool HandlesHedging<TResult>() => _generators.ContainsKey(typeof(TResult));

        public ValueTask<bool> ShouldHandleAsync<TResult>(OutcomeArguments<TResult, HandleHedgingArguments> args)
        {
            if (!_predicates.TryGetValue(typeof(TResult), out var predicate))
            {
                return new ValueTask<bool>(false);
            }

            if (typeof(TResult) == typeof(VoidResult))
            {
                return ((Func<OutcomeArguments<object, HandleHedgingArguments>, ValueTask<bool>>)predicate)(args.AsObjectArguments());
            }
            else
            {
                return ((Func<OutcomeArguments<TResult, HandleHedgingArguments>, ValueTask<bool>>)predicate)(args);

            }
        }

        public Func<ValueTask<Outcome<TResult>>>? TryCreateHedgedAction<TResult>(ResilienceContext context, int attempt, Func<ResilienceContext, ValueTask<Outcome<TResult>>> callback)
        {
            if (!_generators.TryGetValue(typeof(TResult), out var generator))
            {
                return null;
            }

            return ((Func<HedgingActionGeneratorArguments<TResult>, Func<ValueTask<Outcome<TResult>>>?>)generator)(new HedgingActionGeneratorArguments<TResult>(context, attempt, callback));
        }
    }
}

