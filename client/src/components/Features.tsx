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
      icon: <Database className="text-gray-700" size={20} />,
      title: "Automated Data Conversion",
      description:
        "Convert raw Schwab API data into clean CSV, JSON, or Excel files with a single click",
    },
    {
      icon: <BarChart3 className="text-gray-700" size={20} />,
      title: "Smart Data Visualization",
      description:
        "Instantly generate charts and graphs from your converted data",
    },
    {
      icon: <FileText className="text-gray-700" size={20} />,
      title: "Custom Data Templates",
      description:
        "Create and save templates for your most common data conversion needs",
    },
    {
      icon: <CheckCircle className="text-gray-700" size={20} />,
      title: "Data Validation",
      description:
        "Automatically check for inconsistencies and errors in your Schwab data",
    },
    {
      icon: <Database className="text-gray-700" size={20} />,
      title: "Historical Data Storage",
      description: "Store and access your converted data securely in the cloud",
    },
    {
      icon: <ArrowRight className="text-gray-700" size={20} />,
      title: "API Integration",
      description:
        "Connect with other financial tools and platforms through our API",
    },
  ];

  return (
    <section id="features" className="py-20 bg-gray-50">
      <div className="container mx-auto px-6">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-4xl font-bold mb-6 text-gray-900">
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
              className="bg-white p-8 rounded border border-gray-200 hover:border-gray-300 transition-colors group"
            >
              <div className="w-10 h-10 bg-gray-100 rounded flex items-center justify-center mb-6 group-hover:bg-gray-200 transition-colors">
                {feature.icon}
              </div>
              <h3 className="text-lg font-semibold mb-3 text-gray-900">{feature.title}</h3>
              <p className="text-gray-600">{feature.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
