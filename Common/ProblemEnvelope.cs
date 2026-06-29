namespace CompX.Api.Common;

public sealed record ProblemEnvelope(string Type, string Title, int Status, string? Detail, string TraceId);
