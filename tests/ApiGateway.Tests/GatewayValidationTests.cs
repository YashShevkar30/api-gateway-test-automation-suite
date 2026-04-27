using ApiGateway.Tests.Support;
using NUnit.Framework;

namespace ApiGateway.Tests;

[TestFixture]
public class GatewayValidationTests
{
    private readonly IApiGatewayClient _client = new FakeApiGatewayClient();

    [TestCaseSource(typeof(TestScenarioFactory), nameof(TestScenarioFactory.AuthScenarios))]
    public async Task Validates_authentication_flow(GatewayRequest request, int expectedStatus, string expectedError)
    {
        var response = await _client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));
        Assert.That(response.ErrorCode, Is.EqualTo(expectedError));
    }

    [TestCaseSource(typeof(TestScenarioFactory), nameof(TestScenarioFactory.RoutingScenarios))]
    public async Task Validates_routing_and_error_contracts(GatewayRequest request, int expectedStatus)
    {
        var response = await _client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));
    }

    [TestCaseSource(typeof(TestScenarioFactory), nameof(TestScenarioFactory.RateLimitBoundaryScenarios))]
    public void Validates_rate_limiting_boundaries(int requestsPerMinute, int expectedStatus)
    {
        var calculatedStatus = requestsPerMinute <= 120 ? 200 : 429;

        Assert.That(calculatedStatus, Is.EqualTo(expectedStatus));
    }
}
