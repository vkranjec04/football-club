using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Web.Dto;
using FootballClub.Web.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FootballClub.Web.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserService(
        ApplicationDbContext context,
        IOptions<JwtOptions> jwtOptions,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<RegisterResult> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = (username ?? string.Empty).Trim();
        var normalizedEmail = (email ?? string.Empty).Trim();

        var user = new AppUser
        {
            UserName = normalizedUsername,
            Email = normalizedEmail,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // CreateAsync enforces unique username/email (RequireUniqueEmail) and the password
        // policy, so duplicate accounts and weak passwords surface as Identity errors.
        var createResult = await _userManager.CreateAsync(user, password ?? string.Empty);
        if (!createResult.Succeeded)
        {
            return RegisterResult.Failure(createResult.Errors.Select(error => error.Description));
        }

        var roleName = Models.Enums.Role.User.ToString();
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addToRoleResult.Succeeded)
        {
            return RegisterResult.Failure(addToRoleResult.Errors.Select(error => error.Description));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var claims = CreateClaims(user, roles);
        var token = CreateToken(claims, expiresAtUtc);

        return RegisterResult.Success(new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        });
    }

    public async Task<AuthResponseDto?> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = (usernameOrEmail ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedIdentifier) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(candidate =>
            candidate.IsActive &&
            (candidate.UserName == normalizedIdentifier || candidate.Email == normalizedIdentifier),
            cancellationToken);

        if (user == null)
        {
            return null;
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var claims = CreateClaims(user, roles);

        var token = CreateToken(claims, expiresAtUtc);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<AuthResponseDto?> AuthenticateExternalAsync(ClaimsPrincipal externalPrincipal, CancellationToken cancellationToken = default)
    {
        var email = externalPrincipal.FindFirstValue(ClaimTypes.Email) ?? externalPrincipal.FindFirstValue("email");
        var name = externalPrincipal.FindFirstValue(ClaimTypes.Name) ?? externalPrincipal.FindFirstValue("name");
        var providerKey = externalPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? externalPrincipal.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(providerKey))
        {
            return null;
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user != null && !user.IsActive)
        {
            return null;
        }

        if (user == null)
        {
            var usernameBase = CreateUsernameBase(name, email);
            var username = await EnsureUniqueUsernameAsync(usernameBase, cancellationToken);

            user = new AppUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var createResult = await _userManager.CreateAsync(user, GenerateExternalPassword());
            if (!createResult.Succeeded)
            {
                return null;
            }

            if (!await _roleManager.RoleExistsAsync(Models.Enums.Role.User.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(Models.Enums.Role.User.ToString()));
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, Models.Enums.Role.User.ToString());
            if (!addToRoleResult.Succeeded)
            {
                return null;
            }
        }

        user.LastLoginAt = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var claims = CreateClaims(user, roles, includeExternalProviderClaim: true);

        var token = CreateToken(claims, expiresAtUtc);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    private List<Claim> CreateClaims(AppUser user, IEnumerable<string> roles, bool includeExternalProviderClaim = false)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (includeExternalProviderClaim)
        {
            claims.Add(new Claim("auth_provider", "Google"));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAtUtc)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> EnsureUniqueUsernameAsync(string usernameBase, CancellationToken cancellationToken)
    {
        var candidate = usernameBase;
        var suffix = 1;

        while (await _userManager.Users.AnyAsync(user => user.UserName == candidate, cancellationToken))
        {
            candidate = $"{usernameBase}{suffix}";
            suffix++;
        }

        return candidate;
    }

    private static string GenerateExternalPassword() => $"{Guid.NewGuid():N}Aa1!";

    private static string CreateUsernameBase(string? name, string email)
    {
        var source = !string.IsNullOrWhiteSpace(name) ? name : email.Split('@')[0];
        var filtered = new string(source.Where(character => char.IsLetterOrDigit(character)).ToArray());
        return string.IsNullOrWhiteSpace(filtered) ? "googleuser" : filtered;
    }
}