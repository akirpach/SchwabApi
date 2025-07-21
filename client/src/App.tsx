import { useState } from "react";
import Navbar from "./components/Navbar";

function App() {
  return (
    <div className="min-h-screen bg-gradient-to-br neutral-50">
      <Navbar />
      <div className="flex items-center justify-center h-screen">
        <h1 className="text-4xl font-bold text-black">
          SchwabSaaS -
          <span className="bg-gradient-to-r from-purple-400 to-pink-600 bg-clip-text text-transparent">
            Tailwind is Working!
          </span>
        </h1>
      </div>
    </div>
  );
}

export default App;
