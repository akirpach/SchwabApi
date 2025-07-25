// src/components/Navigation.tsx
import React, { useState } from "react";
import { Database, Menu, X } from "lucide-react";

const Navbar: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <header className="fixed top-0 left-0 right-0 bg-white z-50 border-b border-gray-200">
      <nav className="container mx-auto px-6 py-4 flex justify-between items-center">
        <div className="flex items-center">
          <div className="text-gray-900 mr-2">
            <Database size={24} />
          </div>
          <span className="text-xl font-semibold text-gray-900">
            SchwabSaaS
          </span>
        </div>

        {/* Desktop Navigation */}
        <div className="hidden md:flex items-center space-x-8">
          {["How It Works", "Features", "Use Cases", "Pricing", "FAQ"].map(
            (item) => (
              <button
                key={item}
                className="text-sm font-medium hover:text-gray-900 transition-colors text-gray-600"
              >
                {item}
              </button>
            )
          )}
          <button className="bg-gray-900 hover:bg-gray-800 text-white px-5 py-2 rounded text-sm font-medium transition-colors">
            Get Started
          </button>
        </div>

        {/* Mobile Menu Button */}
        <button
          className="md:hidden text-gray-700"
          onClick={() => setIsMenuOpen(!isMenuOpen)}
        >
          {isMenuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>
      </nav>

      {/* Mobile Navigation Menu */}
      {isMenuOpen && (
        <div className="md:hidden bg-white border-t border-gray-200 px-6 py-4">
          <div className="flex flex-col space-y-4">
            {["How It Works", "Features", "Use Cases", "Pricing", "FAQ"].map(
              (item) => (
                <button
                  key={item}
                  className="text-sm font-medium hover:text-gray-900 transition-colors text-gray-600"
                >
                  {item}
                </button>
              )
            )}
            <button className="bg-gray-900 hover:bg-gray-800 text-white px-5 py-2 rounded text-sm font-medium transition-colors mt-2 text-center">
              Get Started
            </button>
          </div>
        </div>
      )}
    </header>
  );
};

export default Navbar;