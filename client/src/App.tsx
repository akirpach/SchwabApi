import { useState } from "react";
import Navbar from "./components/Navbar";
import Hero from "./components/Hero";
import Features from "./components/Features";
import HowItWorks from "./components/Howitworks";
import Pricing from "./components/Pricing";
import Cta from "./components/Cta";
import Faq from "./components/Faq";
import Footer from "./components/Footer";

function App() {
  return (
    <div className="min-h-screen bg-white text-gray-900 font-sans">
      <Navbar />
      <main className="pt-16"></main>
      <Hero />
      <HowItWorks />
      <Features />
      <Pricing />
      <Cta />
      <Faq />
      <Footer />
    </div>
  );
}

export default App;
