import React from "react";
import {
  BarChart3,
  Database,
  FileText,
  CheckCircle,
  ArrowRight,
} from "lucide-react";

export default function Features() {
  const features = [
    {
      icon: <Database className="text-blue-600" size={24} />,
      title: "Automated Data Conversion",
      description:
        "Convert raw Schwab API data into clean CSV, JSON, or Excel files with a single click",
    },
    {
      icon: <BarChart3 className="text-blue-600" size={24} />,
      title: "Smart Data Visualization",
      description:
        "Instantly generate charts and graphs from your converted data",
    },
    {
      icon: <FileText className="text-blue-600" size={24} />,
      title: "Custom Data Templates",
      description:
        "Create and save templates for your most common data conversion needs",
    },
    {
      icon: <CheckCircle className="text-blue-600" size={24} />,
      title: "Data Validation",
      description:
        "Automatically check for inconsistencies and errors in your Schwab data",
    },
    {
      icon: <Database className="text-blue-600" size={24} />,
      title: "Historical Data Storage",
      description: "Store and access your converted data securely in the cloud",
    },
    {
      icon: <ArrowRight className="text-blue-600" size={24} />,
      title: "API Integration",
      description:
        "Connect with other financial tools and platforms through our API",
    },
  ];

  return (
    <section id="features" className="py-20 bg-white">
      <div className="container mx-auto px-6">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-4xl font-bold mb-6">
            Powerful Features for Schwab API Data
          </h2>
          <p className="text-gray-600">
            Our platform transforms complex Schwab API responses into
            well-structured, organized files that are ready for analysis and
            reporting.
          </p>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, i) => (
            <div
              key={i}
              className="bg-white p-6 rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-shadow group"
            >
              <div className="w-12 h-12 bg-blue-50 rounded-lg flex items-center justify-center mb-4 group-hover:bg-blue-100 transition-colors">
                {feature.icon}
              </div>
              <h3 className="text-xl font-semibold mb-3">{feature.title}</h3>
              <p className="text-gray-600">{feature.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
