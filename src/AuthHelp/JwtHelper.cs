using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthHelp;

public class JwtHelper
{
    public static string GenerateJwtToken(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidOperationException("Name is not specified.");
        }

        var claims = new[] {new Claim(ClaimTypes.Name, name)};
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken("ExampleServer", "ExampleClients", claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);
        return JwtTokenHandler.WriteToken(token);
    }

    public static readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
    public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.Parse("cb155a12-7717-4d0a-9837-6872af2360a1").ToByteArray());
}