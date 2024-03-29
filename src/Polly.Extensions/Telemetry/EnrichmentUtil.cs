namespace Polly.Extensions.Telemetry;

internal static class EnrichmentUtil
{
    public static void Enrich(
        ref TagList tags,
        List<Action<EnrichmentContext>> enrichers,
        ResilienceContext resilienceContext,
        Outcome<object>? outcome,
        object? resilienceArguments)
    {
        if (enrichers.Count == 0)
        {
            return;
        }

        var context = EnrichmentContext.Get(resilienceContext, resilienceArguments, outcome);

        foreach (var enricher in enrichers)
        {
            enricher(context);
        }

        foreach (var pair in context.Tags)
        {
            tags.Add(pair.Key, pair.Value);
        }

        EnrichmentContext.Return(context);
    }
}
