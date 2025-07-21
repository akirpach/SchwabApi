import React from "react";
import { ChevronDown } from "lucide-react";

export default function FAQ() {
  const faqs = [
    {
      question: "Is my Schwab data secure with your platform?",
      answer:
        "Yes, we take security very seriously. We use bank-level encryption and never store your Schwab credentials. All data is encrypted both in transit and at rest.",
    },
    {
      question: "What file formats do you support for export?",
      answer:
        "We support CSV, Excel (XLSX), JSON, and PDF formats. We're always adding more based on customer feedback.",
    },
    {
      question: "Do I need technical knowledge to use SchwabiOrg?",
      answer:
        "Not at all! Our platform is designed to be user-friendly for financial professionals of all technical levels. The interface is intuitive and requires no coding knowledge.",
    },
    {
      question: "Can I integrate SchwabiOrg with other tools?",
      answer:
        "Yes, our Professional and Enterprise plans include API access that allows you to integrate with other financial tools and platforms.",
    },
  ];

  return (
    <section className="py-20 bg-white">
      <div className="container mx-auto px-6">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-4xl font-bold mb-6">
            Frequently Asked Questions
          </h2>
          <p className="text-gray-600">
            Find answers to common questions about our platform
          </p>
        </div>

        <div className="max-w-3xl mx-auto space-y-6">
          {faqs.map((faq, i) => (
            <div
              key={i}
              className="border border-gray-200 rounded-lg overflow-hidden"
            >
              <details className="group">
                <summary className="flex justify-between items-center p-6 cursor-pointer list-none">
                  <h3 className="font-medium text-lg">{faq.question}</h3>
                  <ChevronDown
                    size={20}
                    className="text-gray-500 group-open:rotate-180 transition-transform"
                  />
                </summary>
                <div className="px-6 pb-6 text-gray-600">
                  <p>{faq.answer}</p>
                </div>
              </details>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
