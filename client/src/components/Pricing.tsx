import React from "react";

const Pricing = () => {
  const plans = [
    {
      name: "Free",
      price: "$0",
      period: "forever",
      description: "Perfect for getting started",
      features: [
        { text: "100 conversions/month", included: true },
        { text: "2 data templates", included: true },
        { text: "Basic dashboard", included: true },
        { text: "CSV & JSON formats", included: true },
        { text: "Community support", included: true },
        { text: "API access", included: false },
        { text: "Priority support", included: false },
      ],
      cta: "Get Started",
      popular: false,
    },
    {
      name: "Starter",
      price: "$49",
      period: "per month",
      description: "For individuals and small teams",
      features: [
        { text: "1,000 conversions/month", included: true },
        { text: "5 data templates", included: true },
        { text: "Full dashboard access", included: true },
        { text: "All file formats", included: true },
        { text: "Email support", included: true },
        { text: "Basic API access", included: true },
        { text: "Priority support", included: false },
      ],
      cta: "Start Free Trial",
      popular: true,
    },
    {
      name: "Professional",
      price: "$99",
      period: "per month",
      description: "For growing businesses",
      features: [
        { text: "5,000 conversions/month", included: true },
        { text: "Unlimited templates", included: true },
        { text: "Advanced dashboard", included: true },
        { text: "All file formats", included: true },
        { text: "Priority support", included: true },
        { text: "Full API access", included: true },
        { text: "Custom integrations", included: true },
      ],
      cta: "Start Free Trial",
      popular: false,
    },
  ];

  return (
    <section id="pricing" className="py-20 bg-white">
      <div className="container mx-auto px-6">
        <div className="text-center max-w-3xl mx-auto mb-12">
          <h2 className="text-3xl md:text-4xl font-bold mb-6">
            Pay only for what you need
          </h2>
          <p className="text-gray-600">
            Simple, transparent pricing with no hidden fees
          </p>
        </div>

        <div className="flex flex-col md:flex-row gap-6 max-w-5xl mx-auto">
          {plans.map((plan, i) => (
            <div
              key={i}
              className={`flex-1 rounded-xl overflow-hidden transition-all border ${
                plan.popular
                  ? "border-blue-500 shadow-xl shadow-blue-100"
                  : "border-gray-200 shadow-sm"
              }`}
            >
              {plan.popular && (
                <div className="bg-blue-500 text-white text-xs font-medium py-1 text-center">
                  MOST POPULAR
                </div>
              )}

              <div className="p-8">
                <h3 className="text-xl font-bold mb-2">{plan.name}</h3>
                <div className="flex items-baseline mb-4">
                  <span className="text-4xl font-bold">{plan.price}</span>
                  <span className="text-gray-500 ml-1">{plan.period}</span>
                </div>
                <p className="text-gray-600 mb-6">{plan.description}</p>

                <button
                  className={`w-full rounded-lg py-3 font-medium transition-all ${
                    plan.popular
                      ? "bg-blue-500 hover:bg-blue-600 text-white"
                      : "bg-gray-100 hover:bg-gray-200 text-gray-800"
                  }`}
                >
                  {plan.cta}
                </button>

                <div className="mt-8 space-y-4">
                  {plan.features.map((feature, j) => (
                    <div key={j} className="flex items-start">
                      {feature.included ? (
                        <div className="w-5 h-5 rounded-full bg-blue-500 flex-shrink-0 flex items-center justify-center mt-0.5">
                          <svg
                            className="w-3 h-3 text-white"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth="3"
                              d="M5 13l4 4L19 7"
                            ></path>
                          </svg>
                        </div>
                      ) : (
                        <div className="w-5 h-5 rounded-full bg-gray-200 flex-shrink-0 flex items-center justify-center mt-0.5">
                          <svg
                            className="w-3 h-3 text-gray-400"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth="3"
                              d="M6 18L18 6M6 6l12 12"
                            ></path>
                          </svg>
                        </div>
                      )}
                      <span
                        className={`ml-3 text-sm ${
                          feature.included ? "text-gray-700" : "text-gray-400"
                        }`}
                      >
                        {feature.text}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default Pricing;
