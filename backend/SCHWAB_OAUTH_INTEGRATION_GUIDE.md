# Schwab API OAuth Integration & Token Lifecycle Management

## Overview

This system implements the full Schwab API three-legged OAuth workflow with automatic token lifecycle management, ensuring uninterrupted access to Schwab's protected resources.

## Schwab API Specifications

### Token Lifetimes
- **Access Token**: Valid for **30 minutes** (1800 seconds)
- **Refresh Token**: Valid for **7 days** from creation
- **Authorization Code**: Must be URL decoded before use

### OAuth Flow Requirements
- Uses OAuth 2.0 authorization code grant type
- Requires Basic authentication (Client ID:Client Secret base64 encoded)
- Must restart full OAuth flow when refresh token expires

## API Endpoints

### 1. Get Authorization URL
```http
GET /api/auth/authorization-url
```

**Response:**
```json
{
  "authorizationUrl": "https://api.schwabapi.com/v1/oauth/authorize?client_id=...&redirect_uri=...",
  "instructions": {
    "step1": "Visit the authorization URL to begin the three-legged OAuth flow",
    "step2": "User will be redirected to Schwab Login Micro Site (LMS)",
    "step3": "After consent and grant (CAG) process, user will be redirected back with code",
    "step4": "Authorization code will be automatically exchanged for access and refresh tokens",
    "step5": "Token lifecycle management will automatically handle refreshes"
  },
  "tokenLifecycle": {
    "accessTokenValidMinutes": 30,
    "refreshTokenValidDays": 7,
    "automaticRefreshBeforeExpiryMinutes": 30
  }
}
```

### 2. OAuth Callback (Automatic)
```http
GET /api/auth/callback?code={AUTHORIZATION_CODE}&session={SESSION_ID}
```

**Response:**
```json
{
  "message": "OAuth flow completed successfully",
  "tokenInfo": {
    "accessTokenExpiresInMinutes": 30,
    "refreshTokenValidForDays": 7,
    "tokenType": "Bearer",
    "scope": "api"
  },
  "backgroundService": {
    "message": "Token lifecycle management is now active",
    "refreshWillOccurBeforeMinutes": 30
  }
}
```

### 3. Token Status
```http
GET /api/token/status
```

**Response (Active Token):**
```json
{
  "hasToken": true,
  "isValid": true,
  "tokenId": 123,
  "expiresAt": "2025-01-01T12:30:00Z",
  "timeUntilExpiry": "00.00:25:30",
  "createdAt": "2025-01-01T12:00:00Z",
  "refreshTokenAge": "00.02:00:00",
  "needsOAuthRestart": false,
  "schwabApiInfo": {
    "accessTokenValidMinutes": 30,
    "refreshTokenValidDays": 7,
    "refreshTokenDaysRemaining": 5.0
  },
  "nextStep": "Token is valid"
}
```

**Response (No Token):**
```json
{
  "hasToken": false,
  "message": "No token found - OAuth flow needs to be initiated",
  "nextStep": "Call GET /api/auth/authorization-url to start the OAuth flow"
}
```

**Response (Refresh Token Expired):**
```json
{
  "hasToken": true,
  "isValid": false,
  "needsOAuthRestart": true,
  "schwabApiInfo": {
    "refreshTokenDaysRemaining": 0
  },
  "nextStep": "Refresh token expired - restart OAuth flow with GET /api/auth/authorization-url"
}
```

### 4. Manual Token Refresh
```http
POST /api/token/refresh
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "tokenId": 124,
  "expiresAt": "2025-01-01T13:30:00Z"
}
```

**Response (Refresh Token Expired):**
```json
{
  "error": "Token refresh failed"
}
```

## Complete OAuth Flow

### Step 1: Initiate OAuth Flow
1. Call `GET /api/auth/authorization-url`
2. Redirect user to the returned `authorizationUrl`
3. User authenticates with Schwab and grants permissions

### Step 2: Automatic Token Exchange
1. Schwab redirects to `/api/auth/callback` with authorization code
2. System automatically exchanges code for access and refresh tokens
3. Background service begins monitoring token expiration

### Step 3: Automatic Token Management
1. Background service checks token status every 2-15 minutes (configurable)
2. Automatically refreshes access token 30 minutes before expiration
3. Handles refresh token expiration scenarios

### Step 4: Handle Refresh Token Expiration
1. When refresh token expires (after 7 days), system logs critical alert
2. Background service stops attempting refreshes
3. Application must restart OAuth flow from Step 1

## Configuration

### appsettings.json
```json
{
  "SchwabOAuth": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "RedirectUri": "https://yourdomain.com/api/auth/callback",
    "TokenUrl": "https://api.schwabapi.com/v1/oauth/token",
    "AuthorizeUrl": "https://api.schwabapi.com/v1/oauth/authorize"
  },
  "TokenRefresh": {
    "CheckIntervalMinutes": 15,
    "RefreshBeforeExpiryMinutes": 30,
    "ErrorRetryDelayMinutes": 5,
    "MaxConsecutiveFailures": 3
  }
}
```

### Development Settings (Faster Testing)
```json
{
  "TokenRefresh": {
    "CheckIntervalMinutes": 2,
    "RefreshBeforeExpiryMinutes": 10,
    "ErrorRetryDelayMinutes": 1,
    "MaxConsecutiveFailures": 3
  }
}
```

## Background Service Features

### Automatic Monitoring
- Continuously monitors token expiration
- Proactively refreshes before expiration
- Handles network errors and API failures gracefully

### Intelligent Error Handling
```csharp
// Detects refresh token expiration
if (tokenAge.TotalDays >= 7) {
    // Logs critical alert for manual intervention
    // Stops automatic refresh attempts
}

// Handles Schwab API-specific errors
if (response.StatusCode == BadRequest || Unauthorized) {
    // Likely expired refresh token scenario
}
```

### Logging & Monitoring
```
[INFO] Schwab OAuth token saved successfully. Access token expires at: 2025-01-01T12:30:00Z
[INFO] Schwab access token expires in 00:25:00, refreshing now
[INFO] Schwab token successfully refreshed. New token ID: 124, Expires: 2025-01-01T13:30:00Z
[WARN] Schwab token refresh failed. Consecutive failures: 1
[CRIT] Schwab refresh token has expired (age: 7.02:15:30). OAuth flow must be restarted manually.
```

## Usage in Your Application

### Getting Valid Access Token
```csharp
public class MyApiService
{
    private readonly ITokenService _tokenService;
    
    public async Task<string> CallSchwabApiAsync()
    {
        var token = await _tokenService.GetValidTokenAsync();
        if (token == null)
        {
            // Handle OAuth restart scenario
            throw new InvalidOperationException("OAuth flow needs to be restarted");
        }
        
        // Use token.AccessToken for Schwab API calls
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
            
        return await client.GetStringAsync("https://api.schwabapi.com/v1/accounts");
    }
}
```

### Checking OAuth Status
```csharp
public async Task<bool> IsOAuthConfiguredAsync()
{
    var needsRestart = await _tokenService.NeedsOAuthFlowRestartAsync();
    return !needsRestart;
}
```

## Error Scenarios & Solutions

### 1. Refresh Token Expired
**Symptom**: Background service logs critical alerts about expired refresh token
**Solution**: Restart OAuth flow from Step 1

### 2. Network Connectivity Issues
**Symptom**: Consecutive refresh failures in logs
**Solution**: System automatically retries with exponential backoff

### 3. Invalid Client Credentials
**Symptom**: Authorization failures during token exchange
**Solution**: Verify Client ID and Client Secret in configuration

### 4. Callback URL Mismatch
**Symptom**: OAuth callback fails with redirect URI error
**Solution**: Ensure callback URL matches exactly with Schwab app configuration

## Security Best Practices

1. **Store Client Secret Securely**: Use environment variables or secure configuration
2. **HTTPS Only**: All OAuth endpoints must use HTTPS
3. **Token Storage**: Tokens are stored securely in database with proper access controls
4. **Logging**: Sensitive information is not logged (tokens, secrets)
5. **Error Handling**: Detailed error information is not exposed to clients

## Monitoring & Alerting

### Key Metrics to Monitor
- Token refresh success rate
- Time until refresh token expiration
- Background service health
- OAuth flow completion rate

### Critical Alerts
- Refresh token expiration (7-day warning)
- Multiple consecutive refresh failures
- Background service failures

### Dashboards
- Current token status
- Token age and expiration times
- Refresh history and patterns
- Error rates and types

## Testing

### Manual Testing Flow
1. Call `GET /api/auth/authorization-url`
2. Complete OAuth flow in browser
3. Monitor `GET /api/token/status` for token information
4. Wait for automatic refresh (or trigger with `POST /api/token/refresh`)
5. Test refresh token expiration after 7 days

### Automated Testing
- Unit tests for token service methods
- Integration tests for OAuth flow
- Background service tests with mock scenarios
- Error handling tests for various failure modes

This integrated system ensures reliable, automatic management of Schwab API tokens while providing full visibility and control over the OAuth lifecycle.