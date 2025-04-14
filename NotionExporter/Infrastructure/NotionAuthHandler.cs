namespace NotionExporter.Infrastructure;

public class NotionAuthHandler : DelegatingHandler
{
    private string? _token;

    public void SetToken(string token)
    {
        _token = token;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}