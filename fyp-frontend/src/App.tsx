import './App.css'
import { BrowserRouter as Router, Routes, Route, useNavigate } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/theme-provider';
import WorkflowsPage from './app/pages/WorkflowsPage';
import CreateWorkflowPage from './app/pages/CreateWorkflowPage/CreateWorkflowPage';

import '@xyflow/react/dist/style.css';
import LoginPage from './app/pages/LoginPage/LoginPage';
import { Toaster } from 'sonner';
import { useSelector } from 'react-redux';
import { RootState } from './store';
import { useEffect } from 'react';
import Api from './api';

function App() {
  const user = useSelector((state: RootState) => state.app.user);

  function safelyRedirectToLoginPage() {
    if (!isCurrentLocationLoginPage()) {
      document.location.href = "/login";     
    }
  }

  function isCurrentLocationLoginPage() {
    return document.location.href.split("/").at(-1) === "login";
  }

  /** Test validity of token by sending it to the API server and checking the response */
  function testValidityOfToken() {
    Api.testTokenValidity()
    .then((response) => {
      console.log("Token checked, still valid!");      
    })
    .catch((error) => {
      console.log("Invalid token, redirecting to login page");
      safelyRedirectToLoginPage();       
    })
  }

  useEffect(() => {
    if (user === null) {      
      safelyRedirectToLoginPage()
    }
  }, [user]);

  useEffect(() => {
    void testValidityOfToken();
  }, []);

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
      <Toaster /> 
    </ThemeProvider>    
  )
}

export default App
