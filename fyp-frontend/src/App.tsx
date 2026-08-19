import './App.css';
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Layout from './app/layout';
import { ThemeProvider } from './components/ThemeProvider';
import CreateWorkflowPage from './app/pages/CreateWorkflowPage/CreateWorkflowPage';

import '@xyflow/react/dist/style.css';
import LoginPage from './app/pages/LoginPage/LoginPage';
import { toast, Toaster } from 'sonner';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from './store';
import { useEffect, useMemo, useState } from 'react';
import Api from './api';
import Utils from './util';
import RegisterPage from './app/pages/RegisterPage/RegisterPage';
import MyTasksPage from './app/pages/MyTasksPage/MyTasksPage';
import { Spinner } from './components/ui/spinner';
import { SET_ACCOUNTS_DIRECTORY, SET_DEPARTMENTS, SET_ORGANISATION, SET_TASK_TEMPLATES, SET_TASK_TYPES, SET_USER } from './features/appSlice';
import WorkflowInstancesPage from './app/pages/WorkflowInstancesPage/WorkflowInstancesPage';
import WorkflowDashboardPage from './app/pages/WorkflowDashboardPage/WorkflowDashboardPage';
import OrganisationDashboardPage from './app/pages/OrganisationDashboardPage/OrganisationDashboardPage';
import IssuesPage from './app/pages/IssuesPage/IssuesPage';
import SettingsPage from './app/pages/SettingsPage/SettingsPage';
import AuthenticatedUser from './models/AuthenticatedUser';
import Department from './models/Department';
import StripeCheckoutTestPage from './app/pages/StripeCheckoutTestPage/StripeCheckoutTestPage';

export default function App() {
  const user: AuthenticatedUser | null = useSelector((state: RootState) => state.app.user);
  const taskTypes = useSelector((state: RootState) => state.app.taskTypes);
  const dispatcher = useDispatch();

  const [loaded, setLoaded] = useState({
    validatedToken: false,
    accounts: false,
    taskTypes: false,
    taskTemplates: false,
    departments: false,
    organisation: false
  });

  const [loading, setLoading] = useState(false);

  useEffect(function () {
    if (!Utils.isCurrentLocationLoginPage() && !Utils.isCurrentLocationRegisterPage()) {
      void testValidityOfToken();
    }
  }, []);

  useEffect(function () {
    if (!loaded.validatedToken || user === null) {
      return;
    }

    if (Utils.isCurrentLocationLoginPage() || Utils.isCurrentLocationRegisterPage()) {
      return;
    }

    setLoaded(prev => ({
      ...prev,
      accounts: false,
      taskTypes: false,
      taskTemplates: false,
      departments: false,
      organisation: false
    }));

    void fetchAll();
  }, [loaded.validatedToken, user?.id, user?.tenantId]);

  async function testValidityOfToken() {
    setLoading(true);
    try {
      await Api.testTokenValidity();
      console.log("Token is valid.");
    } catch (error) {
      console.log("Token invalid, attempting relogin...");
      await Utils.relogin();
    } finally {
      setLoaded(prev => ({ ...prev, validatedToken: true }));
      setLoading(false);
    }
  }

  async function fetchAll() {
    await Promise.all([
      fetchDepartments(),
      fetchTaskTypes(),
      fetchAccountsDirectory(),
      fetchTaskTemplates(),
      fetchOrganisation()
    ]);
  }

  async function fetchDepartments() {
    try {
      const response = await Api.fetchDepartments();
      const departments = response.data.data as Department[];
      dispatcher(SET_DEPARTMENTS(departments));
      if (user !== null) {
        dispatcher(SET_USER({ ...user, departmentName: departments.find(d => d.id == user.departmentId)?.displayName }));
      }
    } catch (error) {
      toast.error("Failed to load departments");
      console.log(error);
    } finally {
      setLoaded(prev => ({ ...prev, departments: true }));
    }
  }

  async function fetchTaskTypes() {
    if (taskTypes.length !== 0) {
      setLoaded(prev => ({ ...prev, taskTypes: true }));
      return;
    }

    try {
      const response = await Api.taskTemplates.fetchTaskTypes();
      dispatcher(SET_TASK_TYPES(response.data.data));
    } catch (error) {
      toast.error("Failed to load task types");
    } finally {
      setLoaded(prev => ({ ...prev, taskTypes: true }));
    }
  }

  async function fetchAccountsDirectory() {
    try {
      const response = await Api.fetchAccountsDirectory();
      dispatcher(SET_ACCOUNTS_DIRECTORY(response.data.data));
    } catch (error) {
      toast.error("Failed to fetch accounts directory");
    } finally {
      setLoaded(prev => ({ ...prev, accounts: true }));
    }
  }

  async function fetchTaskTemplates() {
    try {
      const response = await Api.taskTemplates.fetchAllTaskTemplates();
      dispatcher(SET_TASK_TEMPLATES(response.data.data));
    } catch (error) {
      toast.error("Failed to load task templates");
    } finally {
      setLoaded(prev => ({ ...prev, taskTemplates: true }));
    }
  }

  async function fetchOrganisation() {
    try {
      const response = await Api.organisation.fetchOrganisation();
      dispatcher(SET_ORGANISATION(response.data.data));
    } catch (error) {
      toast.error("Failed to load organisation");
    } finally {
      setLoaded(prev => ({ ...prev, organisation: true }));
    }
  }

  const allDataLoaded = useMemo(function () {
    return (
      loaded.validatedToken &&
      loaded.accounts &&
      loaded.taskTypes &&
      loaded.taskTemplates &&
      loaded.departments &&
      loaded.organisation
    );
  }, [loaded]);

  const mainRoutes = useMemo(function () {
    return (
      <Route path="/" element={user !== null ? <Layout /> : <Navigate to="/login" />}>
        <Route index element={<MyTasksPage />} />
        <Route path="workflows" element={<WorkflowInstancesPage />} />
        <Route path="settings" element={<SettingsPage/>} />
        <Route path="issues" element={user !== null ? <IssuesPage /> : <Navigate to="/login" />} />
        <Route path="workflows-dashboard" element={user?.isSupervisor ? <WorkflowDashboardPage /> : <Navigate to="/" />} />
        <Route path="organisation" element={user?.isSupervisor ? <OrganisationDashboardPage /> : <Navigate to="/" />} />
        <Route path="workflows/build" element={user?.isSupervisor ? <CreateWorkflowPage /> : <Navigate to="/" />} />
      </Route>
    );
  }, [user]);

  return (
    <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
      <Router>
        <Routes>
          {allDataLoaded || user === null ? (
            mainRoutes
          ) : (
            <Route 
              index
              element={
                <AppLoading
                  validatedToken={loaded.validatedToken}
                  loadedAccounts={loaded.accounts}
                  loadedTaskTypes={loaded.taskTypes}
                  loadedTaskTemplates={loaded.taskTemplates}
                />
              }
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

export function AppLoading(props: {
  validatedToken: boolean;
  loadedAccounts: boolean;
  loadedTaskTypes: boolean;
  loadedTaskTemplates: boolean;
}) {
  return (
    <div className="flex flex-col justify-center my-auto" style={{ minHeight: "100vh" }}>
      <div className="flex flex-row gap-2 justify-center">
        {!props.validatedToken && <h1>Authenticating...</h1>}
        {props.validatedToken && (!props.loadedAccounts || !props.loadedTaskTypes || !props.loadedTaskTemplates) && <h1>Loading...</h1>}
        <Spinner />
      </div>
    </div>
  );
}
