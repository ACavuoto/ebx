using Octokit;
using System.Collections.ObjectModel;
using System.Net;

namespace Ebx.Test.WebApi.IntegrationTests.Mocks;

internal class MockedResponse(HttpStatusCode statusCode, object body, IDictionary<string, string> headers, string contentType) : IResponse
{
    public object Body { get; set; } = body;
    public IReadOnlyDictionary<string, string> Headers { get; set; } = new ReadOnlyDictionary<string, string>(headers);
    public ApiInfo ApiInfo { get; set; } = null!;
    public HttpStatusCode StatusCode { get; set; } = statusCode;
    public string ContentType { get; set; } = contentType;
}