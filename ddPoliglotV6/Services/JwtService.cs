using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using ddPoliglotV6.Data.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using ddPoliglotV6.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ddPoliglotV6.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string _expInMinutes;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ddPoliglotDbContext _context;

        public JwtService(IConfiguration config, Microsoft.AspNetCore.Identity.UserManager<Data.Models.ApplicationUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager, ddPoliglotDbContext context)
        {
            _secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            _expInMinutes = config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;
        }

        public async Task<object> GenerateSecurityToken(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);
            var userGuid = new Guid(user.Id);
            var userLanguageLevels = _context.UserLanguageLevels.AsNoTracking()
                .Where(x => x.UserID == userGuid).ToList();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expInMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
          
            return new { 
                token= tokenHandler.WriteToken(token), 
                userName = user.UserName, 
                userId = user.Id,
                expiresYear = tokenDescriptor.Expires.Value.Year,
                expiresMonth = tokenDescriptor.Expires.Value.Month,
                expiresDay = tokenDescriptor.Expires.Value.Day,
                expiresHour = tokenDescriptor.Expires.Value.Hour,
                expiresMinute = tokenDescriptor.Expires.Value.Minute,
                roles = roles,
                userLanguageLevels = userLanguageLevels
            };
        }
    }
}
