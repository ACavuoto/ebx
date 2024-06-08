namespace Ebx.Test.WebApi.Responses
{
    public record RetrieveAuthorsResponse(IReadOnlyList<string> Authors)
    {
        public int Count => Authors.Count;
    }
}