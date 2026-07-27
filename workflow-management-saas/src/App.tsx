import { BrowserRouter as Router, Routes, Route } from "react-router-dom"
import { LandingPage } from "./app/pages/LandingPage"
import GetStartedPage from "./app/pages/GetStartedPage"

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/get-started" element={<GetStartedPage />} />
      </Routes>
    </Router>
  )
}

export default App
