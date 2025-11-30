using awisk.common.DTOs.Responses;
using awisk.common.Classes;
using System.Security.Claims;

namespace awisk.common.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtTokenStr(TokenResponseDto response);
        string GenerateJwtTokenStr(ApplicationUser user, string roles);
        GenericResponseDto<TokenResponseDto> GenerateToken(ApplicationUser user, string roles, string message);
        List<Claim> CreateClaims(TokenResponseDto response);
    }

}
