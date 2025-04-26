using awisk.common.DTOs.Responses;
using awisk.common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
