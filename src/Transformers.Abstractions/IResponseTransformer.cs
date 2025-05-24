namespace Transformers.Abstractions;

public interface IResponseTransformer
{
    string Id { get; }
    string Name { get; }
    int DisplayOrder { get; }
    string ContentType { get; }
    string Transform(string executionResult);
}