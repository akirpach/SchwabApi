import React, { useEffect, useRef } from 'react';

interface GoogleSignUpButtonProps {
  onSuccess: (credential: string) => void;
  onError: (error: any) => void;
}

const GoogleSignUpButton: React.FC<GoogleSignUpButtonProps> = ({ onSuccess, onError }) => {
  const buttonRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Load Google Identity Services script
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    
    script.onload = () => {
      if (window.google && buttonRef.current) {
        const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
        
        if (!clientId) {
          onError('Google Client ID not configured. Please add VITE_GOOGLE_CLIENT_ID to your .env file.');
          return;
        }

        // Initialize Google Identity Services
        window.google.accounts.id.initialize({
          client_id: clientId,
          callback: (response: any) => {
            if (response.credential) {
              onSuccess(response.credential);
            } else {
              onError('No credential received');
            }
          },
          auto_select: false,
          cancel_on_tap_outside: true,
        });

        // Render the sign-in button
        window.google.accounts.id.renderButton(buttonRef.current, {
          theme: 'outline',
          size: 'large',
          text: 'signup_with',
          width: 300,
          shape: 'rectangular',
          type: 'standard',
        });
      }
    };

    script.onerror = () => {
      onError('Failed to load Google Identity Services');
    };

    document.head.appendChild(script);

    return () => {
      // Cleanup
      if (document.head.contains(script)) {
        document.head.removeChild(script);
      }
    };
  }, [onSuccess, onError]);

  return (
    <div className="flex justify-center">
      <div ref={buttonRef} />
    </div>
  );
};

export default GoogleSignUpButton;