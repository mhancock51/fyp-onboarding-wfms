import './App.css'
import { BrowserRouter as Router, Routes, Route, useNavigate } from "react-router-dom";
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

function App() {
  const user = useSelector((state: RootState) => state.app.user);    
  const dispatch = useDispatch();

  async function relogin(email: string, password: string) {
    await Api.fetchLogin(email, password)
    .then((response) => {
      if (response.status === 200) {
        // successful login
        console.log("Successfully relogged in");
        const authUser: AuthenticatedUser = response.data.data as AuthenticatedUser;
        dispatch(SET_USER(authUser));        
        toast(`Welcome back ${authUser.displayName}! (successfully relogged in)`);
      }
    })
    .catch((error) => {
      // failed to relog
      dispatch(SET_USER(null));
      toast("Failed to re login, redirector to login page", { duration: 1000, onAutoClose: () => { Utils.safelyRedirectToLoginPage();}})
    })
  }

  /** Test validity of token by sending it to the API server and checking the response */
  function testValidityOfToken() {
    Api.testTokenValidity()
    .then((response) => {
      console.log("Token checked, still valid!");      
    })
    .catch((error) => {
      console.log("Invalid token, redirecting to login page");
      // check if login details have been saved
      var loginDetails = Utils.loadLoginDetailsFromLocalStorage();    
      if (loginDetails === null) {
        Utils.safelyRedirectToLoginPage();       
      }
      else {
        // login with details
        void relogin(loginDetails.email, loginDetails.password);
      }

    })
  }

  useEffect(() => {
    if (!Utils.isCurrentLocationLoginPage()) {
      void testValidityOfToken();
    }    
  }, []);

  return (
    <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
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
