# Fix Google OAuth "invalid_request" Error

The `Error 400: invalid_request, flowName=GeneralOAuthFlow` error occurs because your Google OAuth client isn't properly configured. Here's how to fix it:

## Step 1: Set Up Google Cloud Console

1. **Go to Google Cloud Console**: https://console.cloud.google.com/
2. **Create or Select Project**: Create a new project or select an existing one
3. **Enable APIs**:
   - Navigate to "APIs & Services" → "Library"
   - Search for and enable "Google Identity API" or "Google+ API"

4. **Create OAuth 2.0 Credentials**:
   - Go to "APIs & Services" → "Credentials"
   - Click "Create Credentials" → "OAuth 2.0 Client ID"
   - Choose "Web application" as the application type

5. **Configure OAuth Consent Screen**:
   - Go to "APIs & Services" → "OAuth consent screen"
   - Fill in required fields:
     - App name: "SchwabSaaS"
     - User support email: your email
     - Developer contact: your email
   - Add your domain to authorized domains if you have one

6. **Set Authorized Origins**:
   ```
   http://localhost:3000
   http://localhost:5173 (if using Vite dev server)
   ```

7. **Set Authorized Redirect URIs**:
   ```
   http://localhost:3000
   http://localhost:5173
   http://localhost:3000/sign-up
   http://localhost:5173/sign-up
   ```

## Step 2: Configure Your Application

1. **Update your `.env` file** in `/client/` directory:
   ```env
   VITE_GOOGLE_CLIENT_ID=your_actual_client_id_here.apps.googleusercontent.com
   VITE_API_BASE_URL=http://localhost:5000
   ```
   Replace `your_actual_client_id_here` with your real Google Client ID from step 1.

2. **Update backend configuration** in `appsettings.json`:
   ```json
   {
     "GoogleOAuth": {
       "ClientId": "your_actual_client_id_here.apps.googleusercontent.com",
       "ClientSecret": "your_actual_client_secret_here",
       "RedirectUri": "http://localhost:5000/api/auth/google/callback"
     }
   }
   ```

## Step 3: Database Setup (If Not Already Done)

Run this SQL on your PostgreSQL database:
```sql
ALTER TABLE users 
ADD COLUMN IF NOT EXISTS google_id VARCHAR(255) NULL,
ADD COLUMN IF NOT EXISTS google_picture_url TEXT NULL,
ADD COLUMN IF NOT EXISTS is_google_user BOOLEAN DEFAULT FALSE;
```

## Step 4: Test the Fix

1. **Restart your development servers**:
   ```bash
   # Frontend
   cd client && npm run dev
   
   # Backend
   cd backend && dotnet run
   ```

2. **Clear browser cache** or open an incognito window
3. **Try the Google sign-up again**

## Common Issues and Solutions

### Issue: "unauthorized_client" error
**Solution**: Make sure your authorized origins and redirect URIs exactly match your development server URLs.

### Issue: "access_blocked" error
**Solution**: Your OAuth consent screen needs to be properly configured with all required fields.

### Issue: Still getting "invalid_request"
**Solutions**:
- Double-check your Client ID is correctly copied (no extra spaces)
- Ensure your domain is added to authorized domains in OAuth consent screen
- Try using a different browser or incognito mode

### Issue: Backend errors
**Solution**: Make sure your backend is running on port 5000 and the database has the new columns.

## Verification Steps

1. **Check browser console** for any JavaScript errors
2. **Check network tab** to see if the API call is being made
3. **Check backend logs** for any authentication errors
4. **Verify environment variables** are loaded correctly

The error should be resolved once you have:
- ✅ Valid Google Client ID in `.env` file
- ✅ Properly configured OAuth consent screen
- ✅ Correct authorized origins and redirect URIs
- ✅ Both frontend and backend servers running