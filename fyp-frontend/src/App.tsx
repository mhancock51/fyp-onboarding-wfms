import './App.css'
import { BrowserRouter as Router, Routes, Route, useNavigate, Navigate } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/theme-provider';
import CreateWorkflowPage from './app/pages/CreateWorkflowPage/CreateWorkflowPage';

import '@xyflow/react/dist/style.css';
import LoginPage from './app/pages/LoginPage/LoginPage';
import { toast, Toaster } from 'sonner';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, store } from './store';
import { useEffect, useState } from 'react';
import Api from './api';
import Utils from './util';
import RegisterPage from './app/pages/RegisterPage/RegisterPage';
import MyTasksPage from './app/pages/MyTasksPage/MyTasksPage';
import { Spinner } from './components/ui/spinner';
import { SET_ACCOUNTS_DIRECTORY, SET_DEPARTMENTS, SET_TASK_TEMPLATES, SET_TASK_TYPES } from './features/appSlice';
import TaskType from './models/tasks/TaskType';
import AccountDirectory from './models/AccountDirectory';
import WorkflowInstancesPage from './app/pages/WorkflowInstancesPage/WorkflowInstancesPage';
import WorkflowDashboardPage from './app/pages/WorkflowDashboardPage/WorkflowDashboardPage';
import { AxiosResponse } from 'axios';
import HTTPresponse from './models/HTTPresponse';
import Department from './models/Department';

export default function App() {  

  const user = useSelector((state: RootState) => state.app.user);  
  const taskTypes = useSelector((state: RootState) => state.app.taskTypes);
  
  const dispatcher = useDispatch(); 

  const [loading, setLoading] = useState<boolean>(false);
  const [loadedTaskTypes, setLoadedTaskTypes] = useState<boolean>(false);
  const [loadedAccounts, setLoadedAccounts] = useState<boolean>(false);  
  const [loadedTaskTemplates, setLoadedTaskTemplates] = useState<boolean>(false);  
  const [loadedDepartments, setLoadedDepartments] = useState<boolean>(false);  

  const [validatedToken, setValidatedToken] = useState<boolean>(false);

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
    .finally(() => {
      setValidatedToken(true);  
    })
  }  

  // fetch task types
  async function fetchTaskTypes() {
    if (taskTypes.length !== 0) return;

    await Api.fetchTaskTypes()
    .then((response) => {      
      toast("Successfully loaded task types");
      const taskTypes = response.data.data as TaskType[];
      dispatcher(SET_TASK_TYPES(taskTypes));
    })
    .catch((error) => {
      console.error("ERRROR:", error);
      toast.error("Failed to load task types");  
    })
    .finally(() => {
      setLoadedTaskTypes(true);
    })
  }

  async function fetchAccountsDirectory() {    
    await Api.fetchAccountsDirectory()
    .then((response) => {
      var accountsDirectory = response.data.data as AccountDirectory[];                 
      dispatcher(SET_ACCOUNTS_DIRECTORY(accountsDirectory));      
    })
    .catch((error) => {
      toast.error("Failed to fetch accounts directory");
    })
    .finally(() => {
      setLoadedAccounts(true);
    })
  }

  async function fetchTaskTemplates() {
    console.log("fetching task templates");        
    Api.fetchAllTaskTemplates()
    .then((response) => {
      dispatcher(SET_TASK_TEMPLATES(response.data.data));      
    })
    .catch((error) => {      
    })
    .finally(() => {
      setLoadedTaskTemplates(true);      
    })    
  }

  async function fetchDepartments() {
    Api.fetchDepartments()
    .then((response: AxiosResponse<HTTPresponse<Department[], string>>) => {
      dispatcher(SET_DEPARTMENTS(response.data.data as Department[]));
    })
    .catch((error) => {
      toast.error("Failed to load departments");
    })
    .finally(() => {
      setLoadedDepartments(true);
    })
  }

  useEffect(() => {
    if (!Utils.isCurrentLocationLoginPage() && !Utils.isCurrentLocationRegisterPage()) {
      void testValidityOfToken();
    }        
  }, []);

  useEffect(() => {
    if (validatedToken && !Utils.isCurrentLocationLoginPage() && !Utils.isCurrentLocationRegisterPage()) {
      void fetchTaskTypes();
      void fetchAccountsDirectory();
      void fetchTaskTemplates();
      void fetchDepartments();
    }
  }, [validatedToken]);

  return (
    <ThemeProvider attribute="class" defaultTheme="system" enableSystem>    
      <Router>
        <Routes>
          {(validatedToken && loadedAccounts && loadedTaskTypes && loadedTaskTemplates && loadedDepartments) || user === null ? (
            <Route 
              path="/" 
              element={user !== null ? <Layout /> : <Navigate to="/login" />}
            >
              <>
                <Route path="/" element={<MyTasksPage />} />
                <Route path="/workflows" element={<WorkflowInstancesPage />} />
                <Route path="/workflows/dashboard" element={<WorkflowDashboardPage />} />
                <Route path="/workflow-builder" element={<CreateWorkflowPage />} />
                <Route path="/settings" element={<div><h1>Settings</h1></div>} />
              </>
            </Route>
          ) : (
            <Route index element={<AppLoading validatedToken={validatedToken} 
              loadedAccounts={loadedAccounts} loadedTaskTypes={loadedTaskTypes} loadedTaskTemplates={loadedTaskTemplates}/>}
            />
          )}
          <Route path="/login" element={user === null ? <LoginPage /> : <Navigate to="/" />} />
          <Route path="/register" element={user === null ? <RegisterPage /> : <Navigate to="/" />} />
        </Routes>
      </Router> 
      <Toaster />
    </ThemeProvider>
  );
}

export function AppLoading(props: {validatedToken: boolean, loadedAccounts: boolean, loadedTaskTypes: boolean, loadedTaskTemplates: boolean}) {
  return (
    <div className="flex flex-col justify-center my-auto" style={{ minHeight: "100vh" }}>
      <div className="flex flex-row gap-2 justify-center">
        {
          !props.validatedToken &&
          <h1>Authenticating...</h1>
        }
        {
          props.validatedToken && (!props.loadedAccounts || !props.loadedTaskTypes || !props.loadedTaskTemplates) &&
          <h1>Loading...</h1>
        }
        <Spinner />
      </div>
    </div>
  )
}
