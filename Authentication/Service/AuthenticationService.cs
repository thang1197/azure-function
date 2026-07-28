using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AzureFunction.Configuration.App;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;

namespace AzureFunction.Authentication.Service
{
    public class AuthenticationService(ILogger<AuthenticationService> logger, IOptions<JwtSettings> jwtSettings, ClientSecretCredential clientSecretCredential, TelemetryClient telemetry) : IAuthenticationService
    {
        private readonly TelemetryClient _telemetry = telemetry;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly ClientSecretCredential _clientSecretCredential = clientSecretCredential;

        private readonly ILogger<AuthenticationService> _logger = logger;
        public string GenerateToken(string username, string password)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "user") // Useful for Role-Based Authorization
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GrantClientCredentials()
        {
            try
            {
                _telemetry.TrackTrace("Manual telemetry test");
                _telemetry.Flush();
                var token = _clientSecretCredential.GetToken(new TokenRequestContext(["api://18b03cb2-0463-44dc-9ce6-50e641df38c5/.default"]));
                return token.Token;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error granting client credentials");
                throw new Exception("Error granting client credentials: " + ex.Message);
            }
        }
    }
}