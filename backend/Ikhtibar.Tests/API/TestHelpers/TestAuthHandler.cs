using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Tests.API.TestHelpers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
#pragma warning disable CS0618
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
#pragma warning restore CS0618
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Only authenticate if the test client supplies the X-Test-Auth header.
        // This lets tests opt-out of authentication by removing the header on the client.
        if (!Request.Headers.ContainsKey("X-Test-Auth"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Provide both legacy/admin and modern system-admin roles to satisfy different Authorize checks in controllers
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1"), new Claim(ClaimTypes.Name, "testuser"), new Claim(ClaimTypes.Role, "system-admin"), new Claim(ClaimTypes.Role, "ADMIN") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
