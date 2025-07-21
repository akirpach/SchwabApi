import React from "react";

export default function CTA() {
  return (
    <section className="py-20 bg-gradient-to-r from-blue-600 to-indigo-700 text-white">
      <div className="container mx-auto px-6 text-center">
        <h2 className="text-3xl md:text-4xl font-bold mb-6">
          Ready to Transform Your Schwab Data?
        </h2>
        <p className="text-blue-100 max-w-2xl mx-auto mb-8">
          Join hundreds of financial professionals who are saving time and
          gaining insights with SchwabiOrg
        </p>
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <button className="bg-white hover:bg-gray-100 text-blue-600 px-6 py-3 rounded-lg font-medium transition-all">
            Start Free Trial
          </button>
          <button className="bg-transparent hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-medium transition-all border border-white/30">
            Schedule Demo
          </button>
        </div>
      </div>
    </section>
  );
}
