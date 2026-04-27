namespace ApiGateway.Tests.Support;

public sealed record GatewayRequest(
    string Method,
    string Path,
    string Token,
    string Payload = ""
);

public sealed record GatewayResponse(
    int StatusCode,
    string RouteTarget,
    string ErrorCode = ""
);

public interface IApiGatewayClient
{
    Task<GatewayResponse> SendAsync(GatewayRequest request, CancellationToken cancellationToken = default);
}

public sealed class FakeApiGatewayClient : IApiGatewayClient
{
    public Task<GatewayResponse> SendAsync(GatewayRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Task.FromResult(new GatewayResponse(401, "none", "AUTH_MISSING_TOKEN"));
        }

        if (request.Token.StartsWith("expired-", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new GatewayResponse(401, "none", "AUTH_TOKEN_EXPIRED"));
        }

        if (request.Path.StartsWith("/billing", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new GatewayResponse(200, "billing-service"));
        }

        if (request.Path.StartsWith("/inventory", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new GatewayResponse(200, "inventory-service"));
        }

        return Task.FromResult(new GatewayResponse(404, "none", "ROUTE_NOT_FOUND"));
    }
}
