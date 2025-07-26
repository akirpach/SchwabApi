# Google OAuth Setup Guide

## Overview
This guide walks you through setting up Google OAuth authentication for your SchwabAPI application's Sign Up button.

## What's Been Implemented

### Backend Changes
1. **New Google OAuth Settings Model** - `Models/GoogleOAuthSettings.cs`
2. **Updated User Entity** - Added Google OAuth fields:
   - `GoogleId` - Google user identifier
   - `GooglePictureUrl` - User's Google profile picture
   - `IsGoogleUser` - Boolean flag for Google users
3. **Google OAuth DTO** - `DTOs/AuthDtos.cs` includes `GoogleAuthRequest`
4. **Updated AuthService** - New `GoogleAuthAsync` method for Google authentication
5. **Google OAuth Controller** - `Controllers/Auth/GoogleAuthController.cs`

### Frontend Changes
1. **Google Sign-Up Component** - `components/GoogleSignUpButton.tsx`
2. **Updated Navbar** - Replaced regular Sign Up button with Google OAuth button
3. **Type Definitions** - `types/google.d.ts` for Google Identity Services

## Setup Instructions

### 1. Google Cloud Console Setup
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable the Google+ API
4. Go to "Credentials" → "Create Credentials" → "OAuth 2.0 Client ID"
5. Configure consent screen with your app details
6. Set authorized redirect URIs:
   - `http://localhost:3000` (for frontend)
   - `http://localhost:5000/api/auth/google/callback` (for backend)

### 2. Backend Configuration
Update `appsettings.json`:
```json
{
  "GoogleOAuth": {
    "ClientId": "your_google_client_id_here",
    "ClientSecret": "your_google_client_secret_here",
    "RedirectUri": "http://localhost:5000/api/auth/google/callback"
  }
}
```

### 3. Frontend Configuration
Create `.env` file in client directory:
```env
VITE_GOOGLE_CLIENT_ID=your_google_client_id_here
VITE_API_BASE_URL=http://localhost:5000
```

### 4. Database Migration (Manual)
Add these columns to your `users` table:
```sql
ALTER TABLE users 
ADD COLUMN google_id VARCHAR(255) NULL,
ADD COLUMN google_picture_url TEXT NULL,
ADD COLUMN is_google_user BOOLEAN DEFAULT FALSE;
```

## How It Works

1. **User clicks Google Sign Up button** in the navbar
2. **Google Identity Services popup** opens for authentication
3. **Google returns ID token** to the frontend
4. **Frontend sends token** to `/api/auth/google/signin` endpoint
5. **Backend verifies token** with Google's servers
6. **User is created/logged in** and saved to PostgreSQL database

## Features

- **Automatic user creation** from Google profile data
- **Multi-tenant support** - users can create new tenants or join existing ones
- **Existing user detection** - handles returning Google users
- **Error handling** - comprehensive error handling throughout the flow
- **Mobile responsive** - Google button works on desktop and mobile

## Security Features

- **Token verification** using Google's official library
- **Client ID validation** ensures tokens are for your app
- **No password storage** for Google users
- **Tenant isolation** maintains multi-tenant architecture

## Next Steps

1. Set up your Google Cloud Console project
2. Add your actual client credentials to configuration files
3. Run the database migration
4. Test the authentication flow
5. Customize tenant creation flow as needed

## Troubleshooting

- **"Invalid Google token"** - Check client ID configuration
- **CORS errors** - Ensure frontend URL is in Google Console authorized origins
- **Database errors** - Run the manual SQL migration above
- **Network errors** - Check API endpoint URLs in frontend code