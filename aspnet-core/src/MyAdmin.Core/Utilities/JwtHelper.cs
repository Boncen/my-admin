using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyAdmin.Core.Exception;

namespace MyAdmin.Core.Utilities;

public class JwtHelper
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _accessor;

    public JwtHelper(IConfiguration configuration, IHttpContextAccessor accessor)
    {
        _configuration = configuration;
        _accessor = accessor;
    }

    public string CreateToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var algorithm = SecurityAlgorithms.HmacSha256;
        var signingCredentials = new SigningCredentials(secretKey, algorithm);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(48),
             signingCredentials: signingCredentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    /// <summary>
    /// 从请求头获取用户的token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MAException"></exception>
    public string GetUserToken()
    {
        if (_accessor == null || _accessor.HttpContext == null)
        {
            throw new MAException("Cannot inject IHttpContextAccessor");
        }
        var authorizationHeader = _accessor.HttpContext.Request.Headers["Authorization"];
        var value = authorizationHeader.FirstOrDefault();
        return Check.HasValue(value) ? value!.Replace("Bearer ", "") : string.Empty;
    }
}