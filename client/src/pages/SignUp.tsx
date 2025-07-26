import React, { useState } from "react";
import { Link } from "react-router-dom";
import GoogleSignUpButton from "../components/GoogleSignUpButton";

const SignUp: React.FC = () => {
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const handleGoogleSuccess = async (credential: string) => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch('/api/auth/google/signin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          idToken: credential,
          isNewTenant: true,
          tenantName: 'My Company', // This should come from a form in a real app
          subdomain: 'mycompany' // This should come from a form in a real app
        }),
      });

      if (response.ok) {
        const result = await response.json();
        console.log('Google authentication successful:', result);
        // Handle successful authentication - redirect, store tokens, etc.
        alert('Sign up successful! Check console for details.');
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Authentication failed');
        console.error('Google authentication failed:', errorData);
      }
    } catch (error) {
      setError('Network error during authentication');
      console.error('Network error during Google authentication:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleError = (error: any) => {
    setError(typeof error === 'string' ? error : 'Google sign-in failed');
    console.error('Google sign-in error:', error);
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <div className="sm:mx-auto sm:w-full sm:max-w-md text-center mb-8">
            <h2 className="text-3xl font-bold text-gray-900">
              Sign up
            </h2>
            <p className="mt-2 text-sm text-gray-600">
              Already a user?{" "}
              <Link
                to="/sign-in"
                className="font-medium text-gray-900 hover:text-gray-700 transition-colors"
              >
                Sign in
              </Link>
            </p>
          </div>

          {error && (
            <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-md">
              <p className="text-sm text-red-600">{error}</p>
            </div>
          )}

          <div className="mt-6">
            {loading ? (
              <div className="flex justify-center items-center py-4">
                <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-gray-900"></div>
                <span className="ml-2 text-sm text-gray-600">Signing up...</span>
              </div>
            ) : (
              <GoogleSignUpButton 
                onSuccess={handleGoogleSuccess}
                onError={handleGoogleError}
              />
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default SignUp;