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
import { useEffect, useState } from 'react';
import Api from './api';
import Utils from './util';
import { Button } from './components/ui/button';
import RegisterPage from './app/pages/RegisterPage/RegisterPage';
import MyTasksPage from './app/pages/MyTasksPage/MyTasksPage';
import { Spinner } from './components/ui/spinner';

function App() {  

  const user = useSelector((state: RootState) => state.app.user);     

  const [loading, setLoading] = useState<boolean>(false);

  /** Test validity of token by sending it to the API server and checking the response */
  async function testValidityOfToken() {
    setLoading(true);
    await Api.testTokenValidity()
    .then(async(response) => {      
      setLoading(false);
      console.log("Token checked, still valid!");      
    })
    .catch(async(error) => {
      console.log("Invalid token, redirecting to login page");
      // check if login details have been saved
      await Utils.relogin();
      setLoading(false);
    })
  }  

  useEffect(() => {
    if (!Utils.isCurrentLocationLoginPage() && !Utils.isCurrentLocationRegisterPage()) {
      void testValidityOfToken();
    }        
  }, []);

  return (
    <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">    
      {
        loading ? (
          <div className='flex flex-col justify-center my-auto' style={{minHeight: "100vh"}}>
            <div className='flex flex-row gap-2 justify-center'>
              <h1>Authenticating...</h1>
              <Spinner/>
            </div>
          </div>
        ) : (
          <Router>
            <Routes>
              <Route element={user !== null ? <Layout/> : <Navigate to={"/login"} />}>
                <Route path="/" element={<div><h1>Test</h1><Button onClick={testValidityOfToken}>Test API</Button></div>} />
                <Route path="/tasks" element={<MyTasksPage/>} />
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
        )
      }  
      <Toaster /> 
    </ThemeProvider>    
  )
}

export default App
