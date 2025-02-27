import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/theme-provider';
import WorkflowsPage from './app/pages/WorkflowsPage';
import CreateWorkflowPage from './app/pages/CreateWorkflowPage/CreateWorkflowPage';

import '@xyflow/react/dist/style.css';
import LoginPage from './app/pages/LoginPage/LoginPage';

function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <Router>
        <Routes>
          <Route element={<Layout/>}>
            <Route path="/" element={<div><h1>Test</h1></div>} />
            <Route path="/tasks" element={<div><h1>Tasks</h1></div>} />
            <Route path="/workflows" element={<WorkflowsPage/>} />
            <Route path="/workflows-create" element={<CreateWorkflowPage/>} />            
            <Route path="/settings" element={<div><h1>Settings</h1></div>} />
          </Route>
          <Route path="/login" element={<LoginPage/>}/>
        </Routes>
      </Router>    
    </ThemeProvider>    
  )
}

export default App
