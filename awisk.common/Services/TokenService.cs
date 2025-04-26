using awisk.common.DTOs.Responses;
using awisk.common.Helpers;
using awisk.common.Interfaces;
using awisk.common.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace awisk.common.Services
{

    public class TokenService(ApplicationSettings applicationSettings) : ITokenService
    {
        private readonly ApplicationSettings _applicationSettings = applicationSettings;

        public string GenerateJwtTokenStr(TokenResponseDto response)
        {
            var claims = CreateClaims(response); 
            GetJwtHandler(claims, out JwtSecurityToken token, out JwtSecurityTokenHandler tokenHandler);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateJwtTokenStr(ApplicationUser user, string roles)
        {
            List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Email, UniversalOpertaions.IfNullEmptyString(user?.Email)),
                new Claim(ClaimTypes.Role, UniversalOpertaions.IfNullEmptyString(string.Join(",", roles))),
                new Claim(JwtRegisteredClaimNames.Jti, UniversalOpertaions.NewGuidStr),
                new Claim(ClaimTypes.NameIdentifier, UniversalOpertaions.IfNullEmptyString(user?.Id)),
                new Claim(ClaimTypes.Name, UniversalOpertaions.IfNullEmptyString(user?.FullName))
            ];
            GetJwtHandler(claims, out JwtSecurityToken token, out JwtSecurityTokenHandler tokenHandler);
            return tokenHandler.WriteToken(token);
        }

        private void GetJwtHandler(IEnumerable<Claim> claims, out JwtSecurityToken token, out JwtSecurityTokenHandler tokenHandler)
        {
            var _jwtSettings = _applicationSettings.JwtSettings;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.Expiry),
                signingCredentials: creds
            );
            tokenHandler = new JwtSecurityTokenHandler();
        }

        public List<Claim> CreateClaims(TokenResponseDto response)
        {
            return
            [
                new Claim(JwtRegisteredClaimNames.Email, response.Email),
                new Claim(ClaimTypes.Role, response.Roles),
                new Claim(JwtRegisteredClaimNames.Jti, UniversalOpertaions.NewGuidStr),
                new Claim(ClaimTypes.NameIdentifier, response.Id),
                new Claim(ClaimTypes.Name, response.FullName),
                new Claim("token", response.Token)
            ];
        }

        public GenericResponseDto<TokenResponseDto> GenerateToken(ApplicationUser user, string roles, string message)
        {
            var token = GenerateJwtTokenStr(user, roles);
            var tokenResponse = new TokenResponseDto
            {
                FullName = UniversalOpertaions.IfNullEmptyString(user.FullName),
                Id = UniversalOpertaions.IfNullEmptyString(user.Id),
                Roles = UniversalOpertaions.IfNullEmptyString(string.Join(",", roles)),
                Token = token,
                Username = UniversalOpertaions.IfNullEmptyString(user.UserName),
                Email = UniversalOpertaions.IfNullEmptyString(user.Email),
            };
            return new GenericResponseDto<TokenResponseDto>
            {
                StatusCode = HttpStatusCode.Created,
                Message = message,
                Response = tokenResponse
            };
        }
    }

}
