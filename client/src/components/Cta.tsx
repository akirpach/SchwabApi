
export default function CTA() {
  return (
    <section className="py-20 bg-gray-900 text-white">
      <div className="container mx-auto px-6 text-center">
        <h2 className="text-3xl md:text-4xl font-bold mb-6">
          Ready to Transform Your Schwab Data?
        </h2>
        <p className="text-gray-300 max-w-2xl mx-auto mb-8">
          Join hundreds of financial professionals who are saving time and
          gaining insights with SchwabSaaS
        </p>
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <button className="bg-white hover:bg-gray-100 text-gray-900 px-8 py-3 rounded font-medium transition-colors">
            Start Free Trial
          </button>
          <button className="bg-transparent hover:bg-gray-800 text-white px-8 py-3 rounded font-medium transition-colors border border-gray-600">
            Schedule Demo
          </button>
        </div>
      </div>
    </section>
  );
}
