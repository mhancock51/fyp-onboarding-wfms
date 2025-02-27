using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAuthLogic
    {
        public Task<HTTPResponse<AuthenticatedAccountDTO, string>> LoginUser(string email, string hashedPassword);
    }

    public class AuthLogic : IAuthLogic
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly string _issuer;
        private readonly SymmetricSecurityKey _key;
        private readonly string _audience;
        private readonly double _tokenLifespan;

        public AuthLogic(IConfiguration configuration, IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _mapper = mapper;

            _issuer = _configuration["Auth:Issuer"] ?? "";
            if (string.IsNullOrEmpty(_issuer))
            {
                throw new Exception("JWT Issuer not set!");
            }
            string keyStr = _configuration["Auth:Key"] ?? "";            
            if (string.IsNullOrEmpty(keyStr))
            {
                throw new Exception("JWT Key not set!");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

            _audience = _configuration["Auth:Audience"] ?? "";
            if (string.IsNullOrEmpty(_audience))
            {
                throw new Exception("JWT Audience not set!");
            }
            _tokenLifespan = double.Parse(_configuration["Auth:TokenLifespanInMinutes"] ?? "");
            if (_tokenLifespan == default)
            {
                throw new Exception("JWT Token Lifespan not set!");
            }
        }

        public async Task<HTTPResponse<AuthenticatedAccountDTO, string>> LoginUser(string email, string hashedPassword)
        {
            // check account exists and is registered
            var account = await _accountRepository.GetByEmailAddress(email);
            if (account == null)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Account doesn't exist" };
            }
            if (account.AccountStatus != AccountLogic.REGISTERED_STATUS)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Account isn't registered" };
            }

            // check hashed passwords match
            if (account.HashedPassword != hashedPassword)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Incorrect password" };
            }

            // generate token 
            var token = GenerateTokenFromAccount(account);
            var authenticatedAccount = _mapper.Map<AuthenticatedAccountDTO>(account);
            authenticatedAccount.JwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = true, HttpCode = 200, Data = authenticatedAccount };
        }

        private JwtSecurityToken GenerateTokenFromAccount(AccountTable account)
        {            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId),
            };


            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenLifespan),
                signingCredentials: creds
            );
            return token;
        }
    }
}
