
export default function HowItWorks() {
  const steps = [
    {
      step: "01",
      title: "Connect Your Schwab API",
      description:
        "Securely connect your Schwab API credentials to our platform",
    },
    {
      step: "02",
      title: "Select Data & Format",
      description:
        "Choose which data you need and your preferred output format",
    },
    {
      step: "03",
      title: "Download & Use",
      description:
        "Download your organized data files or use them directly in our dashboard",
    },
  ];

  return (
    <section id="how-it-works" className="py-20 bg-white">
      <div className="container mx-auto px-6">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-4xl font-bold mb-6 text-gray-900">
            How SchwabSaaS Works
          </h2>
          <p className="text-gray-600">
            Three simple steps to transform your Schwab API data into organized,
            usable files
          </p>
        </div>

        <div className="grid md:grid-cols-3 gap-8 max-w-4xl mx-auto">
          {steps.map((item, i) => (
            <div key={i} className="relative">
              <div className="bg-gray-50 p-8 rounded border border-gray-200 hover:border-gray-300 transition-colors z-10 relative h-full">
                <div className="text-4xl font-bold text-gray-200 mb-4">
                  {item.step}
                </div>
                <h3 className="text-lg font-semibold mb-3 text-gray-900">{item.title}</h3>
                <p className="text-gray-600">{item.description}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
