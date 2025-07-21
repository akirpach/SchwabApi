import React from "react";
import { Database } from "lucide-react";

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-white py-12">
      <div className="container mx-auto px-6">
        <div className="grid md:grid-cols-4 gap-8">
          <div>
            <div className="flex items-center mb-4">
              <div className="text-blue-400 mr-2">
                <Database size={24} />
              </div>
              <span className="text-xl font-bold">SchwabSaaS</span>
            </div>
            <p className="text-gray-400 text-sm mb-6">
              Transforming Schwab API data into organized, actionable insights
            </p>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Product</h4>
            <ul className="space-y-2 text-gray-400 text-sm">
              {["Features", "Pricing", "API", "Documentation"].map((item) => (
                <li key={item}>
                  <a href="#" className="hover:text-white transition-colors">
                    {item}
                  </a>
                </li>
              ))}
            </ul>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Company</h4>
            <ul className="space-y-2 text-gray-400 text-sm">
              {["About", "Careers", "Blog", "Legal"].map((item) => (
                <li key={item}>
                  <a href="#" className="hover:text-white transition-colors">
                    {item}
                  </a>
                </li>
              ))}
            </ul>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Contact</h4>
            <ul className="space-y-2 text-gray-400 text-sm">
              <li>hello@schwabiorg.com</li>
              <li>+1 (555) 123-4567</li>
              <li>
                123 Financial St, Suite 100
                <br />
                San Francisco, CA 94107
              </li>
            </ul>
          </div>
        </div>

        <div className="border-t border-gray-800 mt-12 pt-8 text-center text-sm text-gray-500">
          <p>© 2025 SchwabiSaaS. All rights reserved.</p>
        </div>
      </div>
    </footer>
  );
}
