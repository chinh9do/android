using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogPost.Repository.Entities;
using BlogPost.Repository.Repositories;
using BlogPost.Repository.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BlogPost.Repository.Models.SettingModels;
using AutoMapper;
using BlogPost.Service.Helpers;

namespace BlogPost.Service.Services;

public interface IUserService : IBaseService<UserModel>
{
    Task<AuthenticatedResponse> Login(UserModel model);
    Task<AuthenticatedResponse> RefreshToken(TokenModel tokenModel);
    Task RevokeToken(string userName);
    Task RevokeAllToken();
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public JwtSettings _jwtSettings;

    public UserService(IUserRepository userRepository, IMapper mapper, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthenticatedResponse> Login(UserModel model)
    {
        Console.WriteLine("into login");
        try
        {
            var user = await _userRepository.Get(model.UserName, model.Password);

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            Console.WriteLine("done get user login");
            var token = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenValidityInDays);

            await _userRepository.UpdateAsync(user);
            Console.WriteLine("updated user {0}", refreshToken);
            return new AuthenticatedResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string GenerateAccessToken(User user)
    {
        // authentication successful so generate jwt token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _jwtSettings.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Role", user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityInMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public async Task<AuthenticatedResponse> RefreshToken(TokenModel tokenModel)
    {
        try
        {
            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return null;
            }

            Console.WriteLine("refresh token 2");

            string username = principal.Identity.Name;
            Console.WriteLine("refresh token 2.5 {0}", username);
            var user = await _userRepository.FindByUserName(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                  Console.WriteLine("{0} || {1}", user.RefreshToken, refreshToken) ;
                return null;
            }
          
            var token = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine("refresh token 3");
            return new AuthenticatedResponse
            {
                AccessToken = token,
                RefreshToken = user.RefreshToken
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task RevokeToken(string userName)
    {
        try
        {
            var user = await _userRepository.FindByUserName(userName);

            user.RefreshToken = null;
            await _userRepository.UpdateAsync(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RevokeAllToken()
    {
        try
        {
            var users = await _userRepository.GetAll();

            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userRepository.UpdateAsync(user);
            }
        }
        catch
        {
            throw;
        }
    }

    public Task<IEnumerable<UserModel>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(UserModel item)
    {
        await _userRepository.UpdateAsync(_mapper.Map<User>(item));
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}
