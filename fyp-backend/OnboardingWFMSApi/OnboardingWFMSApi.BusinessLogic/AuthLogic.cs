using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAuthLogic
    {
        public Task<HTTPResponse<AuthenticatedAccountDTO, string>> LoginUser(string email, string password);
    }

    public class AuthLogic : IAuthLogic
    {
        private readonly ILogger<AuthLogic> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly string _issuer;
        private readonly SymmetricSecurityKey _key;
        private readonly string _audience;
        private readonly double _tokenLifespan;

        private readonly IAccountRepository _accountRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public AuthLogic(IConfiguration configuration, IAccountRepository accountRepository, IMapper mapper, ILogger<AuthLogic> logger, 
            IDepartmentRepository departmentRepository)
        {
            _logger = logger;
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

            _departmentRepository = departmentRepository;
        }

        public async Task<HTTPResponse<AuthenticatedAccountDTO, string>> LoginUser(string email, string password)
        {
            // hash password
            password = GetHashString(password);
            
            // check account exists and is registered
            var account = await _accountRepository.GetByEmailAddress(email);
            if (account == null)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Account doesn't exist" };
            }
            if (account.AccountStatus != AccountConstants.REGISTERED_STATUS)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Account isn't registered" };
            }

            // check hashed passwords match
            if (account.HashedPassword != password)
            {
                return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Incorrect password" };
            }

            // generate token 
            var token = GenerateTokenFromAccount(account);
            var authenticatedAccount = await GetAuthenticatedAccountDTO(account, token);            

            _logger.LogInformation($"Authenticated user with email address: {account.EmailAddress}");

            return new HTTPResponse<AuthenticatedAccountDTO, string>() { Success = true, HttpCode = 200, Data = authenticatedAccount };
        }

        private async Task<AuthenticatedAccountDTO> GetAuthenticatedAccountDTO(AccountTable account, JwtSecurityToken token)
        {
            var authenticatedAccount = _mapper.Map<AuthenticatedAccountDTO>(account);
            authenticatedAccount.JwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var department = await _departmentRepository.GetById(account.DepartmentId);
            authenticatedAccount.DepartmentName = department.DisplayName;

            return authenticatedAccount;
        }

        private JwtSecurityToken GenerateTokenFromAccount(AccountTable account)
        {            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id),
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
