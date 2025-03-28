import axios, { AxiosResponse } from 'axios';
import { store } from './store';
import Utils from './util';
import WorkflowTemplateDTO from './models/DTOs/WorkflowTemplateDTO';
import { CreateWorkflowTemplatePayload } from './models/payloads/CreateWorkflowTemplatePayload';
import { ChecklistTaskInstance } from './models/tasks/ChecklistTaskInstance';
import FileUploadTaskInstance from './models/tasks/FileUploadTaskInstance';
import ReadDocumentTaskInstance from './models/tasks/ReadDocumentTaskInstance';
import ProjectTaskInstance from './models/tasks/ProjectTaskInstance';
const ROUTE_URL = import.meta.env.VITE_BACKEND_SERVICE_ROUTE_URL;

const AuthInstance = axios.create();
AuthInstance.interceptors.request.use((config: any) => {
  const token = store.getState().app.user?.jwtToken;
  config.headers.Authorization = `Bearer ${token}`;
  return config
});
AuthInstance.interceptors.response.use(
  (config: AxiosResponse) => {    
    return config;
  }, 
  async(error: any) => {
    const statusCode = error.response.status;
    // if statusCode is 401 try to relogin
    if (statusCode === 401) {
      // attempt to relog
      await Utils.relogin();
    }
    
    // Do something with request error
    return Promise.reject(error);
  }
);

const Api = {
  account: {
    makeSupervisor: async(accountId: string) => {
      return AuthInstance.post(`${ROUTE_URL}/account/make-supervisor`, null, { params: {accountId: accountId}});
    }
  },
  fetchLogin: async(emailAddress: string, password: string) => {
    return axios.post(`${ROUTE_URL}/auth/login?emailAddress=${emailAddress}&password=${password}`);
  },
  testTokenValidity: async() => {
    return AuthInstance.get(`${ROUTE_URL}/auth/test`);
  },
  fetchDepartments: async() => {
    return AuthInstance.get(`${ROUTE_URL}/department/all`);    
  },
  inviteUser: async(displayName: string, email: string, departmentId: string) => {    
    return AuthInstance.post(`${ROUTE_URL}/account/invite`, null, {
      params: {
        displayName:  displayName,
        emailAddress: email,        
        departmentId: departmentId
      }
    });
  },
  createDepartment: async(dptName: string) => {
    return AuthInstance.post(`${ROUTE_URL}/department/create`, null, {
      params: {
        name: dptName
      }
    });
  },
  getInvitedAccount: async(emailAddress: string) => {
    return AuthInstance.get(`${ROUTE_URL}/account/get-invited-account`, {
      params: {
        emailAddress: emailAddress
      }
    });
  },
  registerAccount: async(emailAddress: string, password: string, confirmationPassword: string) => {
    return AuthInstance.post(`${ROUTE_URL}/account/register`, null, {
      params: {
        emailAddress: emailAddress,
        password: password,
        confirmationPassword: confirmationPassword
      }
    });
  },
  fetchAssignedTaskInstance: async() => {
    return AuthInstance.get(`${ROUTE_URL}/task/instances/get-assigned?accountId=477430cf-bb2b-4936-bf40-ee6779a95e25`); 
  },
  updateTaskState: async(taskState: ChecklistTaskInstance | FileUploadTaskInstance | ReadDocumentTaskInstance | ProjectTaskInstance, taskTypeId: string, taskInstanceId: string) => {
    return AuthInstance.post(`${ROUTE_URL}/task/instances/update-task-state`, {
      updateTaskState: taskState,
      taskTypeId: taskTypeId,
      taskInstanceId: taskInstanceId
    });
  },
  completeTask: async(taskInstanceId: string) => {
    return AuthInstance.post(`${ROUTE_URL}/task/instances/complete`, null, {
      params: {
        taskInstanceId : taskInstanceId
      }
    });
  },
  fetchTaskTypes: async() => {
    return AuthInstance.get(`${ROUTE_URL}/task/templates/task-types`);
  },
  createTaskTemplate: async(name: string, description: string, taskTypeId: string, taskData: any) => {
    return AuthInstance.post(`${ROUTE_URL}/task/templates/create`, {
      Name: name,
      Description: description,
      TaskTypeId: taskTypeId,
      TaskTypeData: taskData
    })
  },
  fetchDocumentData: async(documentId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/document/data?documentId=${documentId}`);
  },
  fetchAllTaskTemplates: async() => {
    return AuthInstance.get(`${ROUTE_URL}/task/templates/all`);
  },
  fetchAccountsDirectory: async() => {
    return AuthInstance.get(`${ROUTE_URL}/account/directory`);
  },
  createWorkflowTemplate: async(payload: CreateWorkflowTemplatePayload) => {
    return AuthInstance.post(`${ROUTE_URL}/workflow-template/create`, payload);
  },
  fetchWorkflowTemplate: async(workflowTemplateId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/workflow-template?workflowTemplateId=${workflowTemplateId}`);
  },
  fetchTaskTemplateComments: async(taskTemplateId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/comment/tasktemplate?taskTemplateId=${taskTemplateId}`);
  },
  postTaskTemplateComment: async(taskTemplateId: string, comment: string, parentCommentId?: string) => {
    return AuthInstance.post(`${ROUTE_URL}/comment/tasktemplate`, {
      taskTemplateId: taskTemplateId,
      text: comment,
      parentCommentId: parentCommentId
    });
  },
  fetchWorkflowInstances: async() => {
    return AuthInstance.get(`${ROUTE_URL}/workflow/instance/get`);
  },
  fetchWorkflowsDocuments: async(workflowInstanceId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/document/workflow-instance/data?workflowInstanceId=${workflowInstanceId}`);
  },
  fetchAllWorkflowTemplates: async() => {
    return AuthInstance.get(`${ROUTE_URL}/workflow-template/all`);
  },
  createWorkflowInstance: async(workflowTeamplateId: string, supervisorAccountId: string, onboardingEmployeeDetails: { displayName: string, emailAddress: string, departmentId: string} | null) => {
    return AuthInstance.post(`${ROUTE_URL}/workflow/instance/create`, {
      workflowTeamplateId,
      supervisorAccountId,
      onboardingEmployeeDetails
    })
  }
}

export default Api;