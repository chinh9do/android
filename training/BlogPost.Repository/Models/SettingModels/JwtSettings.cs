namespace BlogPost.Repository.Models.SettingModels;

public class JwtSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Subject { get; set; }
    public byte TokenValidityInMinutes { get; set; }
    public byte RefreshTokenValidityInDays { get; set; }
}
