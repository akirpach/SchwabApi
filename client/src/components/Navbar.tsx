// src/components/Navigation.tsx
import React, { useState, useEffect } from "react";
import { Database, Menu, X } from "lucide-react";

const Navbar: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);

  // Track scroll position for background blur effect
  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 50);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <header className="fixed top-0 left-0 right-0 bg-white/80 backdrop-blur-md z-50 border-b border-gray-100">
      <nav className="container mx-auto px-6 py-4 flex justify-between items-center">
        <div className="flex items-center">
          <div className="text-blue-600 mr-2">
            <Database size={28} />
          </div>
          <span className="text-xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
            SchwabSaaS
          </span>
        </div>

        {/* Desktop Navigation */}
        <div className="hidden md:flex items-center space-x-8">
          {["How It Works", "Features", "Use Cases", "Pricing", "FAQ"].map(
            (item) => (
              <button
                key={item}
                className="text-sm font-medium hover:text-blue-600 transition-colors text-gray-600"
              >
                {item}
              </button>
            )
          )}
          <button className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg text-sm font-medium transition-all shadow-lg shadow-blue-600/20 hover:shadow-blue-600/30">
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
        <div className="md:hidden bg-white border-t border-gray-100 px-6 py-4 shadow-lg">
          <div className="flex flex-col space-y-4">
            {["How It Works", "Features", "Use Cases", "Pricing", "FAQ"].map(
              (item) => (
                <button
                  key={item}
                  className="text-sm font-medium hover:text-blue-600 transition-colors text-gray-600"
                >
                  {item}
                </button>
              )
            )}
            <button className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg text-sm font-medium transition-all shadow-md shadow-blue-600/20 mt-2 text-center">
              Get Started
            </button>
          </div>
        </div>
      )}
    </header>
  );
};

export default Navbar;