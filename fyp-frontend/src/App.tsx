import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/theme-provider';


function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <Router>
        <Routes>
          <Route element={<Layout/>}>
            <Route path="/" element={<div><h1>Test</h1></div>} />
            <Route path="/tasks" element={<div><h1>Tasks</h1></div>} />
            <Route path="/workflows" element={<div><h1>Workflows</h1></div>} />
            <Route path="/settings" element={<div><h1>Settings</h1></div>} />
          </Route>
        </Routes>
      </Router>    
    </ThemeProvider>    
  )
}

export default App
