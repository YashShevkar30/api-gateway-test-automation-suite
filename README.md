# API Gateway Test Automation Suite

End-to-end API gateway validation framework built with C#, NUnit, and Python.

## What this project covers
- Authentication flow validation (valid token, expired token, malformed token)
- Rate limiting boundaries and throttling behavior
- Route mapping and upstream service handoff checks
- Error response contract validation
- Concurrent load simulation for burst traffic

## Test design highlights
- Uses NUnit parameterized tests and reusable fixtures
- Structured around OOP test helpers and domain models
- Contains 200+ generated test scenarios for boundary and edge cases

## Project layout
- `tests/ApiGateway.Tests`: NUnit suite and gateway validation helpers
- `load-tests`: Python scripts for concurrent request simulation
- `.github/workflows`: CI pipeline configuration
