import React from "react";
import { Link } from "react-router-dom";

const SignIn: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <div className="sm:mx-auto sm:w-full sm:max-w-md text-center mb-8">
            <h2 className="text-3xl font-bold text-gray-900">
              Sign in
            </h2>
            <p className="mt-2 text-sm text-gray-600">
              Don't have an account?{" "}
              <Link
                to="/sign-up"
                className="font-medium text-gray-900 hover:text-gray-700 transition-colors"
              >
                Sign up
              </Link>
            </p>
          </div>

          <div className="mt-6">
            <p className="text-center text-gray-500 text-sm">
              Sign in functionality coming soon
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SignIn;