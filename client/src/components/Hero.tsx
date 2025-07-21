import React from "react";
import { ArrowRight, FileText } from "lucide-react";

const Hero = () => {
  return (
    <section
      id="hero"
      className="relative overflow-hidden pt-16 md:pt-24 pb-20 md:pb-28"
    >
      {/* Decorative background element */}
      <div className="absolute top-0 left-0 w-full h-full overflow-hidden z-0">
        <div className="absolute -top-40 -right-40 w-96 h-96 bg-blue-200 rounded-full opacity-20 blur-3xl"></div>
        <div className="absolute top-60 -left-20 w-72 h-72 bg-indigo-200 rounded-full opacity-20 blur-3xl"></div>
      </div>

      <div className="container mx-auto px-6 relative z-10">
        <div className="flex flex-col md:flex-row items-center">
          <div className="md:w-1/2 mb-12 md:mb-0">
            <div className="inline-block px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium mb-6">
              Schwab API Data Simplified
            </div>
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold leading-tight mb-6">
              Transform{" "}
              <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Unstructured
              </span>{" "}
              Schwab Data into{" "}
              <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Organized
              </span>{" "}
              Files
            </h1>
            <p className="text-gray-600 text-lg md:text-xl mb-8 max-w-xl">
              Stop struggling with messy API data. Our platform converts complex
              Schwab API responses into clean, structured files ready for
              analysis.
            </p>
            <div className="flex flex-col sm:flex-row gap-4">
              <button className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-medium transition-all shadow-lg shadow-blue-600/20 hover:shadow-blue-600/30 flex items-center justify-center">
                Start Free Trial <ArrowRight className="ml-2" size={18} />
              </button>
              <button className="bg-white hover:bg-gray-50 text-gray-700 px-6 py-3 rounded-lg font-medium transition-all border border-gray-200 flex items-center justify-center">
                View Demo
              </button>
            </div>
          </div>

          <div className="md:w-1/2 relative"></div>
        </div>
      </div>
    </section>
  );
};

export default Hero;
