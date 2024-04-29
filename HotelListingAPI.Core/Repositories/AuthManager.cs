using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListingAPI.Core.Contracts;
using HotelListingAPI.Core.Models.Users;
using HotelListingAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Repositories;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthManager> _logger;
    private ApiUser _user;

    private readonly string _loginProvider = "HotelListingApi";
    private readonly string _refreshToken = "RefreshToken";

    public AuthManager(
        IMapper mapper,
        UserManager<ApiUser> userManager,
        IConfiguration configuration,
        ILogger<AuthManager> logger
    )
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        _logger.LogInformation($"Looking for user with email {loginDto.Email}.");
        _user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (_user is null)
        {
            return default;
        }

        var isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
        if (!isValidUser)
        {
            _logger.LogWarning($"User with email {loginDto.Email} was not found.");
            return null;
        }

        var token = await GenerateToken();
        _logger.LogInformation(
            $"Token generated for user with email {loginDto.Email} | Token: {token}"
        );

        return new AuthResponseDto
        {
            Token = token,
            UserId = _user.Id,
            RefreshToken = await CreateRefreshToken()
        };
    }

    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
    {
        _user = _mapper.Map<ApiUser>(userDto);
        _user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(_user, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(_user, "User");
        }

        return result.Errors;
    }

    public async Task<string> CreateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);

        var newRefreshToken = await _userManager.GenerateUserTokenAsync(
            _user,
            _loginProvider,
            _refreshToken
        );

        await _userManager.SetAuthenticationTokenAsync(
            _user,
            _loginProvider,
            _refreshToken,
            newRefreshToken
        );

        return newRefreshToken;
    }

    public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
    {
        var JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = JwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var username = tokenContent
            .Claims.ToList()
            .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)
            ?.Value;

        _user = await _userManager.FindByNameAsync(username);

        if (_user is null || _user.Id != request.UserId)
        {
            return null;
        }

        var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(
            _user,
            _loginProvider,
            _refreshToken,
            request.RefreshToken
        );

        if (isValidRefreshToken)
        {
            var token = await GenerateToken();
            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        await _userManager.UpdateSecurityStampAsync(_user);
        return null;
    }

    private async Task<string> GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"])
        );

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);

        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email),
            new Claim("uid", _user.Id),
        }
            .Union(userClaims)
            .Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
