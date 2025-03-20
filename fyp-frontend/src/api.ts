import axios, { AxiosResponse } from 'axios';
import { store } from './store';
import Utils from './util';
import WorkflowTemplateDTO from './models/DTOs/WorkflowTemplateDTO';
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
  fetchLogin: async(emailAddress: string, password: string) => {
    return axios.post(`${ROUTE_URL}/auth/login?emailAddress=${emailAddress}&password=${password}`);
  },
  testTokenValidity: async() => {
    return AuthInstance.get(`${ROUTE_URL}/auth/test`);
  },
  fetchDepartments: async() => {
    return AuthInstance.get(`${ROUTE_URL}/department/all`);    
  },
  inviteUser: async(displayName: string, email: string, isOnboarder: boolean, departmentId: string) => {    
    return AuthInstance.post(`${ROUTE_URL}/account/invite`, null, {
      params: {
        displayName:  displayName,
        emailAddress: email,
        isOnboarder:  isOnboarder,
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
  updateChecklistTaskState: async(taskInstanceId: string, itemStatuses: boolean[]) => {
    return AuthInstance.post(`${ROUTE_URL}/task/instances/update-task-state/checklist`, {
      taskInstanceId: taskInstanceId,
      itemCompletionStatuses: itemStatuses
    })
  },
  updateReadDocTaskState: async(taskInstanceId: string, checkboxChecked: boolean, linkClicked: boolean) => {
    return AuthInstance.post(`${ROUTE_URL}/task/instances/update-task-state/read-doc`, {
      taskInstanceId: taskInstanceId,
      checkboxChecked: checkboxChecked,
      linkClicked: linkClicked
    });
  },
  updateUploadDocTaskState: async(taskInstanceId: string, fileData: File) => {
    const formData = new FormData();
    formData.append("file", fileData);
    formData.append("taskInstanceId", taskInstanceId);
    return AuthInstance.post(`${ROUTE_URL}/task/instances/update-task-state/upload-doc`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    })
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
  fetchDocument: async(documentId: string) => {
    window.open(`${ROUTE_URL}/document?documentId=${documentId}`, '_blank');
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
  createWorkflowTemplate: async(payload: WorkflowTemplateDTO) => {
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
  }
}

export default Api;