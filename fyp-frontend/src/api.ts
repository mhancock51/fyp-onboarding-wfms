import axios, { AxiosResponse } from 'axios';
import { store } from './store';
import Utils from './util';
import WorkflowTemplateDTO from './models/DTOs/WorkflowTemplateDTO';
import { CreateWorkflowTemplatePayload } from './models/payloads/CreateWorkflowTemplatePayload';
import { ChecklistTaskInstance } from './models/tasks/ChecklistTaskInstance';
import FileUploadTaskInstance from './models/tasks/FileUploadTaskInstance';
import ReadDocumentTaskInstance from './models/tasks/ReadDocumentTaskInstance';
import ProjectTaskInstance from './models/tasks/ProjectTaskInstance';
import { UploadDocumentPayload } from './models/payloads/UploadDocumentPayload';
import { FileUploadTaskTemplate } from './models/tasks/FileUploadTaskTemplate';
import { ReadDocumentTaskTemplate } from './models/tasks/ReadDocumentTaskTemplate';
import ProjectTaskTemplate from './models/tasks/ProjectTaskTemplate';
import { ChecklistTaskTemplate } from './models/tasks/ChecklistTaskTemplate';
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
  notifications: {
    fetchNotifications: async() => {
      return AuthInstance.get(`${ROUTE_URL}/notifications/all`);
    },
    deleteNotification: async(notificationId: string) => {
      return AuthInstance.delete(`${ROUTE_URL}/notifications/delete?notificationId=${notificationId}`);
    }
  },
  issues: {
    createIssue: async(taskInstanceId: string, description: string, suggestedChanges: string) => {
      return AuthInstance.post(`${ROUTE_URL}/issues/create`, {
        taskInstanceId: taskInstanceId,
        description: description,
        suggestedChanges: suggestedChanges
      });
    },
    fetchAll: async() => {
      return AuthInstance.get(`${ROUTE_URL}/issues/all`);
    },
    fetchUsersIssues: async() => {
      return AuthInstance.get(`${ROUTE_URL}/issues`);
    },
    updateStatus: async(status: string, remark: string, issueId: string) => {
      return AuthInstance.post(`${ROUTE_URL}/issues/update`, {
        status: status,
        remark: remark,
        issueId: issueId
      });
    }
  },
  documents: {
    uploadDocument: async(file: File, documentName: string, accessAccountIds: string[], taskInstanceId?: string) => {
     const fileBase64 = (await Utils.convertFileToBase64(file)).split(",")[1];
     const payload = {
       taskInstanceId: taskInstanceId ?? "",
       fileBase64: fileBase64,
       fileName: file.name,
       documentName: documentName || undefined,
       accessAccountIds: accessAccountIds.length > 0 ? accessAccountIds : undefined,
     };
     return AuthInstance.post(`${ROUTE_URL}/document/upload`, payload);
    }
   },
  account: {
    makeSupervisor: async(accountId: string) => {
      return AuthInstance.post(`${ROUTE_URL}/account/make-supervisor`, null, { params: {accountId: accountId}});
    }
  },
  organisation: {
    fetchOrganisation: async() => {
      return AuthInstance.get(`${ROUTE_URL}/organisation`);
    },
    renameOrganisation: async(newName: string) => {
      return AuthInstance.post(`${ROUTE_URL}/organisation/rename`, null, { params: {newName: newName}});
    }
  },
  audit: {
    fetchWorkflowInstanceAuditLogs: async(workflowInstanceId: string) => {
      return AuthInstance.get(`${ROUTE_URL}/audit/workflow-instance-logs?workflowInstanceId=${workflowInstanceId}`);
    }
  },
  taskTemplates: {
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
    fetchAllTaskTemplates: async(status?: string) => {
      return AuthInstance.get(`${ROUTE_URL}/task/templates/all?status=${status !== undefined ? status : ""}`);
    },
    archiveTemplate: async(taskTemplateId: string) => {
      return AuthInstance.post(`${ROUTE_URL}/task/templates/archive?taskTemplateId=${taskTemplateId}`);
    },
    updateTemplate: async(taskTemplateId: string, updatedDescription: string, updatedTaskTypeData: FileUploadTaskTemplate | ReadDocumentTaskTemplate | ProjectTaskTemplate | ChecklistTaskTemplate | null) => {
      return AuthInstance.put(`${ROUTE_URL}/task/templates/update`, {
        id: taskTemplateId,
        updatedDescription: updatedDescription,
        updateTaskTypeData: updatedTaskTypeData
      });
    },
    fetchTemplateHasActiveInstances: async(taskTemplateId: string) => {
      return AuthInstance.get(`${ROUTE_URL}/task/templates/has-active-instances?taskTemplateId=${taskTemplateId}`);
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
  fetchDocumentData: async(documentId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/document/data?documentId=${documentId}`);
  }, 
  fetchAccountsDirectory: async() => {
    return AuthInstance.get(`${ROUTE_URL}/account/directory`);
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
  fetchWorkflowsDocuments: async(workflowInstanceId: string) => {
    return AuthInstance.get(`${ROUTE_URL}/document/workflow-instance/data?workflowInstanceId=${workflowInstanceId}`);
  },  
  createWorkflowInstance: async(workflowTeamplateId: string, supervisorAccountId: string, onboardingEmployeeDetails: { displayName: string, emailAddress: string, departmentId: string} | null) => {
    return AuthInstance.post(`${ROUTE_URL}/workflow/instance/create`, {
      workflowTeamplateId,
      supervisorAccountId,
      onboardingEmployeeDetails
    })
  },
  analytics: {
    fetchOnboardingAnalytics: async(from?: Date) => {
      return AuthInstance.get(`${ROUTE_URL}/analytics/onboarding`, {
        params: {
          from: from
        }
      })
    },
    fetchOnboardedEmployeesTimeline: async() => {
      return AuthInstance.get(`${ROUTE_URL}/analytics/onboarding/timeline`);
    }
  },
  workflowInstances: {
    fetchAllOpenWorkflowInstance: async() => {
      return AuthInstance.get(`${ROUTE_URL}/workflow/instance/onboarding/open`);
    },
    fetchWorkflowInstance: async(workflowInstanceId: string) => {
      return AuthInstance.get(`${ROUTE_URL}/workflow/instance?workflowInstanceId=${workflowInstanceId}`);
    },
    fetchWorkflowInstances: async() => {
      return AuthInstance.get(`${ROUTE_URL}/workflow/instance/all`);
    },
  },
  workflowTemplates: {
    updateWorkflowTemplate: async(payload: CreateWorkflowTemplatePayload) => {
      return AuthInstance.post(`${ROUTE_URL}/workflow/template/update`, payload);
    },
    createWorkflowTemplate: async(payload: CreateWorkflowTemplatePayload) => {
      return AuthInstance.post(`${ROUTE_URL}/workflow/template/create`, payload);
    },
    fetchAllWorkflowTemplates: async() => {
      return AuthInstance.get(`${ROUTE_URL}/workflow/template/all`);
    },
    fetchWorkflowTemplate: async(workflowTemplateId: string) => {
      return AuthInstance.get(`${ROUTE_URL}/workflow/template?workflowTemplateId=${workflowTemplateId}`);
    },
    archiveWorkflowTemplate: async(workflowTemplateId: string) => {
      return AuthInstance.post(`${ROUTE_URL}/workflow/template/archive`, null, {params: {workflowTemplateId: workflowTemplateId}});
    }
  }
}

export default Api;