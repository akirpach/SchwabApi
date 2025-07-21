// src/components/Hero.tsx
import React from 'react';
import { ArrowRight } from 'lucide-react';
import Button from './Button';
import SimpleTable from './SimpleTable';
import JsonDisplay from './JsonDisplay';

const Hero: React.FC = () => {
  const handleGetStarted = () => {
    alert('Starting your free trial! 🚀');
    // Later you can replace this with: window.location.href = '/signup'
  };

  const handleViewDocs = () => {
    alert('Opening documentation! 📚');
    // Later you can replace this with: window.location.href = '/docs'
  };

  return (
    <section className="pt-32 pb-20 px-4 sm:px-6 lg:px-8 relative overflow-hidden">
      <div className="max-w-7xl mx-auto">
        
        {/* Hero Text Section */}
        <div className="text-center mb-16">
          <h1 className="text-4xl md:text-6xl lg:text-7xl font-bold text-white mb-6 leading-tight">
            Transform Raw{' '}
            <span className="bg-gradient-to-r from-purple-400 to-pink-600 bg-clip-text text-transparent">
              Schwab Data
            </span>{' '}
            into Insights
          </h1>
          
          <p className="text-xl md:text-2xl text-slate-300 mb-8 leading-relaxed max-w-4xl mx-auto">
            Stop wrestling with complex JSON responses. Our platform transforms Schwab's raw API data 
            into beautiful, interactive tables that your users will love.
          </p>

          {/* Feature Pills */}
          <div className="flex flex-wrap justify-center gap-3 mb-8">
            <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-full px-4 py-2 text-sm text-slate-300">
              ⚡ Real-time Data
            </div>
            <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-full px-4 py-2 text-sm text-slate-300">
              🔒 Bank-Grade Security
            </div>
            <div className="bg-slate-800/50 backdrop-blur-sm border border-slate-700 rounded-full px-4 py-2 text-sm text-slate-300">
              📊 Interactive Tables
            </div>
          </div>
        </div>

        {/* Visual Demo Section: JSON → Table */}
        <div className="grid lg:grid-cols-12 gap-8 items-center mb-16">
          
          {/* Raw JSON Side (Left) */}
          <div className="lg:col-span-4">
            <div className="mb-4">
              <h3 className="text-lg font-semibold text-white mb-2">
                📄 Raw Schwab API Response
              </h3>
              <p className="text-sm text-slate-400">
                Complex, unreadable JSON data that's hard to work with
              </p>
            </div>
            <JsonDisplay />
          </div>

          {/* Transformation Arrow (Center) */}
          <div className="lg:col-span-1 flex justify-center">
            <div className="bg-gradient-to-r from-purple-500 to-pink-600 p-4 rounded-full shadow-lg transform hover:scale-110 transition-transform duration-300">
              <ArrowRight className="w-6 h-6 text-white" />
            </div>
          </div>

          {/* Beautiful Table Side (Right) */}
          <div className="lg:col-span-7">
            <div className="mb-4">
              <h3 className="text-lg font-semibold text-white mb-2">
                ✨ Beautiful, Interactive Table
              </h3>
              <p className="text-sm text-slate-400">
                Clean, searchable, sortable data that users actually enjoy using
              </p>
            </div>
            <SimpleTable />
          </div>
        </div>

        {/* Call-to-Action Buttons */}
        <div className="text-center">
          <div className="flex flex-col sm:flex-row gap-4 justify-center items-center mb-8">
            <Button primary onClick={handleGetStarted}>
              Start Free Trial
            </Button>
            <Button onClick={handleViewDocs}>
              View Documentation
            </Button>
          </div>
          
          {/* Trust Indicators */}
          <div className="text-sm text-slate-400">
            <p className="mb-2">Trusted by developers at</p>
            <div className="flex justify-center items-center space-x-8 opacity-60">
              <span className="font-semibold">FinTech Startups</span>
              <span>•</span>
              <span className="font-semibold">Trading Firms</span>
              <span>•</span>
              <span className="font-semibold">Investment Apps</span>
            </div>
          </div>
        </div>
      </div>

      {/* Background Decorative Elements */}
      <div className="absolute top-20 left-10 w-72 h-72 bg-purple-500/10 rounded-full blur-3xl animate-pulse"></div>
      <div className="absolute top-40 right-10 w-96 h-96 bg-pink-500/10 rounded-full blur-3xl animate-pulse" style={{animationDelay: '1s'}}></div>
      <div className="absolute bottom-20 left-1/2 transform -translate-x-1/2 w-80 h-80 bg-blue-500/5 rounded-full blur-3xl"></div>
    </section>
  );
};

export default Hero;