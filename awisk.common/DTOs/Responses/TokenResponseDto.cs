using awisk.common.Helpers;

namespace awisk.common.DTOs.Responses
{
    public partial class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string Id { get; set; } = UniversalOpertaions.EmptyGuidStr;
        public string Email { get; set; } = string.Empty;

    }
}
