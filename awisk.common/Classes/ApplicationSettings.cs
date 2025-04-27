namespace awisk.common.Classes
{
    public partial class ApplicationSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public ApiSettings ApiSettings { get; set; } = new();
        public JwtSettings JwtSettings { get; set; } = new();
        public AuthConfig Auth { get; set; } = new();
        public PasswordSettings PasswordSettings { get; set; } = new();
    }
}
