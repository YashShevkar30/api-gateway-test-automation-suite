using NUnit.Framework;

namespace ApiGateway.Tests.Support;

public static class TestScenarioFactory
{
    public static IEnumerable<TestCaseData> AuthScenarios()
    {
        yield return new TestCaseData(new GatewayRequest("GET", "/billing/invoices", "valid-token"), 200, "");
        yield return new TestCaseData(new GatewayRequest("GET", "/billing/invoices", ""), 401, "AUTH_MISSING_TOKEN");
        yield return new TestCaseData(new GatewayRequest("GET", "/billing/invoices", "expired-abc"), 401, "AUTH_TOKEN_EXPIRED");
    }

    public static IEnumerable<TestCaseData> RoutingScenarios()
    {
        var routes = new[]
        {
            "/billing/invoices",
            "/billing/payments",
            "/inventory/items",
            "/inventory/stock",
            "/unknown/path"
        };

        foreach (var route in routes)
        {
            var expected = route.StartsWith("/unknown", StringComparison.OrdinalIgnoreCase) ? 404 : 200;
            yield return new TestCaseData(new GatewayRequest("GET", route, "valid-token"), expected);
        }
    }

    public static IEnumerable<TestCaseData> RateLimitBoundaryScenarios()
    {
        // 210 generated scenarios to model 200+ boundary checks.
        for (var requestsPerMinute = 1; requestsPerMinute <= 210; requestsPerMinute++)
        {
            var expected = requestsPerMinute <= 120 ? 200 : 429;
            yield return new TestCaseData(requestsPerMinute, expected)
                .SetName($"rpm_{requestsPerMinute}_expects_{expected}");
        }
    }
}
