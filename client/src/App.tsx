import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import Hero from "./components/Hero";
import Features from "./components/Features";
import HowItWorks from "./components/Howitworks";
import Pricing from "./components/Pricing";
import Cta from "./components/Cta";
import Faq from "./components/Faq";
import Footer from "./components/Footer";
import SignUp from "./pages/SignUp";
import SignIn from "./pages/SignIn";

// Home page component
const HomePage = () => (
  <>
    <main className="pt-16"></main>
    <Hero />
    <HowItWorks />
    <Features />
    <Pricing />
    <Cta />
    <Faq />
    <Footer />
  </>
);

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50 text-gray-900 font-sans">
        <Navbar />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/sign-up" element={<SignUp />} />
          <Route path="/sign-in" element={<SignIn />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
