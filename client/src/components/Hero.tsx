import { ArrowRight } from "lucide-react";

const Hero = () => {
  return (
    <section
      id="hero"
      className="bg-white pt-24 pb-32"
    >
      <div className="container mx-auto px-6">
        <div className="max-w-3xl mx-auto text-center">
          <div className="inline-block px-4 py-1 bg-gray-100 text-gray-800 rounded text-sm font-medium mb-8">
            Schwab API Data Simplified
          </div>
          <h1 className="text-4xl md:text-5xl font-bold leading-tight mb-6 text-gray-900">
            Transform Unstructured Schwab Data into Organized Files
          </h1>
          <p className="text-gray-600 text-lg mb-12 max-w-2xl mx-auto">
            Stop struggling with messy API data. Our platform converts complex
            Schwab API responses into clean, structured files ready for
            analysis.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <button className="bg-gray-900 hover:bg-gray-800 text-white px-8 py-3 rounded font-medium transition-colors flex items-center justify-center">
              Start Free Trial <ArrowRight className="ml-2" size={18} />
            </button>
            <button className="bg-white hover:bg-gray-50 text-gray-700 px-8 py-3 rounded font-medium transition-colors border border-gray-300 flex items-center justify-center">
              View Demo
            </button>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Hero;
