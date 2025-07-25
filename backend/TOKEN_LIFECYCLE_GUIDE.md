# Token Lifecycle Management Guide

## Overview
This system implements automatic token refreshing for Schwab API tokens before they expire, ensuring uninterrupted service.

## Architecture

### Core Components

1. **TokenService**: Enhanced with lifecycle management methods
2. **TokenRefreshBackgroundService**: Monitors and refreshes tokens automatically
3. **TokenController**: Provides endpoints for monitoring token status
4. **TokenRefreshSettings**: Configuration for refresh behavior

### Background Service Flow

```
Background Service → Check Token Status → Refresh if Needed → Log Results
     ↓                     ↓                    ↓              ↓
Every 15min         30min before expiry    Schwab API      Success/Failure
```

## Configuration

### appsettings.json
```json
{
  "TokenRefresh": {
    "CheckIntervalMinutes": 15,        // How often to check token status
    "RefreshBeforeExpiryMinutes": 30,  // Refresh token this many minutes before expiry
    "ErrorRetryDelayMinutes": 5,       // Delay after errors before retrying
    "MaxConsecutiveFailures": 3        // Alert threshold for consecutive failures
  }
}
```

### Development Settings (Faster for Testing)
```json
{
  "TokenRefresh": {
    "CheckIntervalMinutes": 2,         // Check every 2 minutes
    "RefreshBeforeExpiryMinutes": 10,  // Refresh 10 minutes before expiry
    "ErrorRetryDelayMinutes": 1,       // Retry after 1 minute on error
    "MaxConsecutiveFailures": 3
  }
}
```

## API Endpoints

### GET /api/token/status
Returns current token status:
```json
{
  "hasToken": true,
  "isValid": true,
  "tokenId": 123,
  "expiresAt": "2025-01-01T12:00:00Z",
  "timeUntilExpiry": "01.02:30:00",
  "createdAt": "2025-01-01T10:00:00Z"
}
```

### POST /api/token/refresh
Manually triggers token refresh:
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "tokenId": 124,
  "expiresAt": "2025-01-01T14:00:00Z"
}
```

### GET /api/token/health
Quick health check:
```json
{
  "status": "Healthy",          // "Healthy", "Expiring Soon", "Expired"
  "isValid": true,
  "timeUntilExpiry": "01.02:30:00",
  "timestamp": "2025-01-01T11:00:00Z"
}
```

## Testing Endpoints (Development Only)

### POST /api/tokentest/create-expiring-token
Creates a test token that expires in 5 minutes for testing automatic refresh.

### GET /api/tokentest/validate-configuration
Validates the token lifecycle configuration and current status.

## Logging

The system provides detailed logging at different levels:

- **Information**: Normal operations (token refreshed, service started/stopped)
- **Warning**: Token refresh failures, expiring tokens
- **Error**: Exceptions during refresh attempts
- **Critical**: Multiple consecutive failures requiring attention

### Sample Log Messages
```
[15:30:00 INF] Token Refresh Background Service started
[15:32:00 DBG] Token is valid for 01.05:30:00, no refresh needed
[16:25:00 INF] Token expires in 00:30:00, refreshing now
[16:25:01 INF] Token successfully refreshed. New expiry: 2025-01-01T18:25:01Z
[16:30:00 WRN] Token refresh failed. Consecutive failures: 1
[16:35:00 CRT] Token refresh has failed 3 consecutive times. Manual intervention may be required.
```

## Error Handling

### Automatic Recovery
- Background service continues running even after errors
- Failure counter resets on successful operations
- Configurable retry delays prevent rapid failure loops

### Failure Scenarios
1. **Network Issues**: Service waits and retries based on configuration
2. **Invalid Refresh Token**: Critical log generated, manual intervention needed
3. **API Rate Limits**: Configurable delays help avoid rate limit issues
4. **Database Issues**: Service logs errors but continues attempting refreshes

### Monitoring Consecutive Failures
The system tracks consecutive failures and logs critical alerts when the threshold is reached:
- Helps identify persistent issues
- Prevents silent failures
- Enables proactive monitoring

## Best Practices

### Production Configuration
- Set `CheckIntervalMinutes` to 15-30 minutes
- Set `RefreshBeforeExpiryMinutes` to 30-60 minutes
- Monitor logs for critical alerts
- Set up alerting on critical log messages

### Development Configuration
- Use shorter intervals for faster testing
- Enable debug logging to see detailed operation flow
- Use test endpoints to validate behavior

### Monitoring
1. **Health Endpoint**: Regular health checks via `/api/token/health`
2. **Log Monitoring**: Watch for consecutive failure patterns
3. **Token Status**: Monitor token expiry times via `/api/token/status`
4. **Database Monitoring**: Ensure token records are being created/updated

## Integration

### Using in Other Services
```csharp
public class MyApiService
{
    private readonly ITokenService _tokenService;
    
    public async Task<string> MakeApiCallAsync()
    {
        var token = await _tokenService.GetValidTokenAsync();
        // Token is guaranteed to be valid or refreshed automatically
        // Use token.AccessToken for API calls
    }
}
```

### Manual Refresh Triggers
```csharp
// In a controller or service
var refreshedToken = await _tokenService.RefreshTokenAsync();
if (refreshedToken == null)
{
    // Handle refresh failure
}
```

## Troubleshooting

### Common Issues

1. **Background Service Not Starting**
   - Check that `AddHostedService<TokenRefreshBackgroundService>()` is registered
   - Verify configuration section exists in appsettings

2. **Tokens Not Refreshing**
   - Check logs for error messages
   - Verify Schwab API credentials are correct
   - Ensure refresh token is not expired/invalid

3. **High Failure Rate**
   - Check network connectivity to Schwab API
   - Verify API rate limits are not being exceeded
   - Check system clock synchronization

### Debug Steps
1. Check background service logs
2. Call `/api/token/status` to see current token state
3. Try manual refresh via `/api/token/refresh`
4. Validate configuration via `/api/tokentest/validate-configuration`