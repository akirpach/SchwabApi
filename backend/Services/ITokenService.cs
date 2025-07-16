using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Services
{
    public interface ITokenService
    {
        Task SaveTokenAsync(SchwabTokenResponse tokenResponse);
        Task<OAuthToken?> GetValidTokenAsync();
        Task<OAuthToken?> RefreshTokenAsync();
    }
}