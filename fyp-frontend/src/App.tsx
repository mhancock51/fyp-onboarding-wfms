import './App.css'
import { BrowserRouter as Router, Routes, Route, useNavigate, Navigate } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/theme-provider';
import WorkflowsPage from './app/pages/WorkflowsPage';
import CreateWorkflowPage from './app/pages/CreateWorkflowPage/CreateWorkflowPage';

import '@xyflow/react/dist/style.css';
import LoginPage from './app/pages/LoginPage/LoginPage';
import { toast, Toaster } from 'sonner';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from './store';
import { useEffect } from 'react';
import Api from './api';
import Utils from './util';
import AuthenticatedUser from './models/AuthenticatedUser';
import { SET_USER } from './features/appSlice';
import { Button } from './components/ui/button';
import RegisterPage from './app/pages/RegisterPage/RegisterPage';

function App() {  

  const user = useSelector((state: RootState) => state.app.user);    

  /** Test validity of token by sending it to the API server and checking the response */
  function testValidityOfToken() {
    Api.testTokenValidity()
    .then((response) => {
      console.log("Token checked, still valid!");      
    })
    .catch(async(error) => {
      console.log("Invalid token, redirecting to login page");
      // check if login details have been saved
      await Utils.relogin();

    })
  }

  useEffect(() => {
    if (!Utils.isCurrentLocationLoginPage() && !Utils.isCurrentLocationRegisterPage()) {
      void testValidityOfToken();
    }    
  }, []);

  return (
    <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
      <Router>
        <Routes>
          <Route element={user !== null ? <Layout/> : <Navigate to={"/login"} />}>
            <Route path="/" element={<div><h1>Test</h1><Button onClick={testValidityOfToken}>Test API</Button></div>} />
            <Route path="/tasks" element={<div><h1>Tasks</h1></div>} />
            <Route path="/workflows" element={<WorkflowsPage/>} />
            <Route path="/workflows-create" element={<CreateWorkflowPage/>} />            
            <Route path="/settings" element={<div><h1>Settings</h1></div>} />
            {/* only allow client to access these paths if admin */}
            {/* <Route path="/invite" element={user?.isAdmin ? <InviteUser/> : <Navigate to={"/"} />}/> */}
          </Route>
          <Route path="/login" element={user === null ? <LoginPage/> : <Navigate to={"/"}/>}/>          
          <Route path='/register' element={user === null ? <RegisterPage/> : <Navigate to={"/"}/>}/>
        </Routes>
      </Router>         
      <Toaster /> 
    </ThemeProvider>    
  )
}

export default App
