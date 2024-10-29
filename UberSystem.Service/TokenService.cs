using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using UberSystem.Domain.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace UberSystem.Service;
public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

	public string GenerateJwtToken(User user, IList<string> roles)
	{
		var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Email),
			//new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			//new Claim("UserId", user.Id.ToString()),  // Thêm thông tin độc nhất
		};

		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var token = new JwtSecurityToken(
			issuer: _configuration["JwtSettings:Issuer"],
			audience: _configuration["JwtSettings:Audience"],
			claims,
			expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
			signingCredentials: credentials
		);

		var result = new JwtSecurityTokenHandler().WriteToken(token);
		return result;
	}


	/// <summary>
	/// Extract and return the role from the claims
	/// </summary>
	/// <param name="claims"></param>
	/// <returns></returns>
	public string GetRoleFromClaims(IList<Claim> claims) 
    { 
        var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim is not null)
            return roleClaim.Value;

        return string.Empty;    // function error
    } 

}