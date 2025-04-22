CREATE TABLE department (
    DepartmentId VARCHAR(255) PRIMARY KEY,
    DisplayName VARCHAR(255) NOT NULL
);
CREATE TABLE account (
    AccountId VARCHAR(255) PRIMARY KEY,
    DisplayName VARCHAR(255) NOT NULL,
    EmailAddress VARCHAR(255) NOT NULL,
    HashedPassword VARCHAR(255) NOT NULL,    
    IsSupervisor TINYINT(1) NOT NULL,
    DepartmentId VARCHAR(255),
    OrganisationId VARCHAR(255),
    AccountStatus VARCHAR(255)
);

CREATE TABLE organisation (
    OrganisationId VARCHAR(255) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL
);

CREATE TABLE organisationAdminLink (
    Id VARCHAR(255) PRIMARY KEY,
    OrganisationId VARCHAR(255),
    AccountId VARCHAR(255)
);

CREATE TABLE tasktype (
    TaskTypeId VARCHAR(255) NOT NULL,
    TaskName VARCHAR(255) NOT NULL,
    PRIMARY KEY (TaskTypeId)
);

CREATE TABLE tasktemplate (
    TaskTemplateId VARCHAR(255) NOT NULL,
    Name VARCHAR(255) UNIQUE NOT NULL ,
    Description TEXT,
    CreatorAccountId VARCHAR(255) NOT NULL,
    DateCreated DATETIME NOT NULL,
    TaskTypeId VARCHAR(255) NOT NULL,
    PRIMARY KEY (TaskTemplateId),
    Status VARCHAR(255) NOT NULL,
    LastModifiedTimestamp DATETIME
);

CREATE TABLE fileuploadtasktemplate (
    Id VARCHAR(255) NOT NULL,
    TaskTemplateId VARCHAR(255) NOT NULL,
    SupportedDocumentType VARCHAR(255) NOT NULL,
    DocumentName VARCHAR(255) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE readdocumenttasktemplate (
    Id VARCHAR(255) NOT NULL,
    TaskTemplateId VARCHAR(255) NOT NULL,
    DocumentName VARCHAR(255) NOT NULL,
    DocumentUrl VARCHAR(2083) NOT NULL,
    CheckBoxLabel VARCHAR(255) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE checklisttasktemplate (
    Id VARCHAR(255) PRIMARY KEY,
    TaskTemplateId VARCHAR(255),
    Items JSON
);

CREATE TABLE taskinstance (
    TaskInstanceId VARCHAR(255) PRIMARY KEY,
    AssigneeAccountId VARCHAR(255),
    AssignerAccountId VARCHAR(255),
    TaskTemplateId VARCHAR(255),
    WorkflowInstanceId VARCHAR(255),
    CreationTimestamp DATETIME,
    Status VARCHAR(255),    
    DueDate DATETIME,
    WorkflowInstanceNodeId VARCHAR(255),
    CompletionTimestamp DATETIME,
);

CREATE TABLE checklisttaskinstance (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    ItemCompletionStatuses JSON
);

CREATE TABLE readdocumenttaskinstance (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    CheckboxChecked BOOLEAN,
    LinkClicked BOOLEAN
);

CREATE TABLE projectasktemplate (
    Id VARCHAR(255) PRIMARY KEY,
    TaskTemplateId VARCHAR(255),
    Brief TEXT,
    Deliverable VARCHAR(255),
    Objectives JSON,
    SupportLinks JSON,
    Skills JSON
);

CREATE TABLE projecttaskinstance (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    ObjectiveStates JSON
);




CREATE TABLE document (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    CreatorId VARCHAR(255),
    WorkflowInstanceId VARCHAR(255),
    DocumentData LONGBLOB,
    FileExtension VARCHAR(50),
    UploadTimestamp DATETIME,
    FileName VARCHAR(255),
    AccessAccountIds JSON
);

CREATE TABLE documentaccesslink (
    Id VARCHAR(255) PRIMARY KEY,
    AccountId VARCHAR(255),
    DocumentId VARCHAR(255)
);

CREATE TABLE fileuploadtaskinstance (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    DocumentId VARCHAR(255)
);

/* Everything workflow template related */
CREATE TABLE workflowtemplate (
    Id VARCHAR(255) PRIMARY KEY,
    IsOnboardingWF BOOLEAN,
    Name VARCHAR(255),
    Description TEXT,
    Status VARCHAR(255)
);

CREATE TABLE workflowtemplatenode (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowTemplateId VARCHAR(255),
    TaskTemplateId VARCHAR(255),
    AssigneeId VARCHAR(255),
    AccountsToNotify JSON,
    `Order` INT,
    WorkflowSection VARCHAR(255)
);

CREATE TABLE nodetaskdependency (
    Id VARCHAR(255) PRIMARY KEY,
    NodeId VARCHAR(255),
    DependencyNodeId VARCHAR(255),
    WorkflowTemplateId VARCHAR(255)
);

/* Workflow instance */
CREATE TABLE workflowinstance (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowTemplateId VARCHAR(255) NOT NULL,
    OnboarderAccountId VARCHAR(255),
    SupervisorAccountId VARCHAR(255),
    CreationTimestamp DATETIME,    
    MainflowStartTimestamp DATETIME,
    CompletionTimestamp DATETIME
);

CREATE TABLE workflowinstancenode (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowInstanceId VARCHAR(255),
    WorkflowTemplateId VARCHAR(255),
    WorkflowTemplateNodeId VARCHAR(255),
    TaskTemplateId VARCHAR(255),
    Status VARCHAR(255)
);

CREATE TABLE onboardingemployeedetails (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowInstanceId VARCHAR(255),
    DisplayName VARCHAR(255),
    EmailAddress VARCHAR(255),
    DepartmentId VARCHAR(255),
    OnboarderAccountId VARCHAR(255)
);

CREATE TABLE comment (
    Id VARCHAR(255) PRIMARY KEY,
    CommenterId VARCHAR(255),
    Text TEXT,
    TaskTemplateId VARCHAR(255),
    CreationTimestamp DATETIME,
    ParentCommentId VARCHAR(255)
);


CREATE TABLE workflowinstanceauditlog (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowInstanceId VARCHAR(255),
    Log TEXT,
    Timestamp DATETIME,
    AccountId VARCHAR(255)
);

CREATE TABLE notification (
    Id VARCHAR(255) PRIMARY KEY,
    RecipientId VARCHAR(255),
    Status VARCHAR(255),
    Description TEXT,
    Tags JSON,
    Timestamp DATETIME
);

CREATE TABLE `reportedissue` (
  `Id` varchar(255) NOT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `IssueCreatorId` varchar(255) DEFAULT NULL,
  `IssueLoggedTimestamp` datetime DEFAULT NULL,
  `Description` text,
  `SuggestedChanges` text,
  `Status` varchar(255) DEFAULT NULL,
  `Remark` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_reported_issue_issue_creator_id` (`IssueCreatorId`),
  CONSTRAINT `fk_reported_issue_issue_creator_id` FOREIGN KEY (`IssueCreatorId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_reported_issue_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE,
  CONSTRAINT `fk_reported_issue_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
)

CREATE TABLE feedbacktasktemplate (
    id VARCHAR(255) NOT NULL,
    taskTemplateId VARCHAR(255),
    -- 'Questions' is an array of complex types, so store it as JSON
    question JSON,
    PRIMARY KEY (id),
    FOREIGN KEY (taskTemplateId) REFERENCES tasktemplate(TaskTemplateId) ON DELETE CASCADE
);

CREATE TABLE feedbacktaskinstance (
    id VARCHAR(255) NOT NULL,
    taskInstanceId VARCHAR(255),
    -- 'responses' is an int array, so store as JSON
    responses JSON,
    PRIMARY KEY (id),
    FOREIGN KEY (taskInstanceId) REFERENCES taskinstance(TaskInstanceId) ON DELETE CASCADE
);



INSERT INTO `onboarding-wfms-db`.`organisation` (`OrganisationId`, `Name`) VALUES ('organisation', '[EMPTY]');
INSERT INTO `onboarding-wfms-db`.`department` (`DepartmentId`, `DisplayName`) VALUES ('admin','Admin');
INSERT INTO `onboarding-wfms-db`.`department` (`DepartmentId`, `DisplayName`) VALUES ('onboarder','Onboarder');

INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`checklist`, `Checklist`);
INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`upload-document`, `Upload Document`);
INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`read-document`, `Read Document`);
INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES ('project-task', 'Project Task');


ALTER TABLE organisationAdminLink 
ADD CONSTRAINT fk_orgAdminLink_organisation FOREIGN KEY (OrganisationId) REFERENCES organisation(OrganisationId);

ALTER TABLE organisationAdminLink 
ADD CONSTRAINT fk_orgAdminLink_account FOREIGN KEY (AccountId) REFERENCES account(AccountId);


ALTER TABLE account
ADD FOREIGN KEY (OrganisationId) REFERENCES organisation(OrganisationId);

ALTER TABLE account
ADD FOREIGN KEY (DepartmentId) REFERENCES department(DepartmentId);

ALTER TABLE tasktemplate 
ADD CONSTRAINT fk_tasktemplate_creator FOREIGN KEY (CreatorAccountId) 
REFERENCES account (AccountId);

ALTER TABLE tasktemplate 
ADD CONSTRAINT fk_tasktemplate_tasktype FOREIGN KEY (TaskTypeId) 
REFERENCES tasktype (TaskTypeId);

ALTER TABLE fileuploadtasktemplate 
ADD CONSTRAINT fk_fileuploadtasktemplate_tasktemplate FOREIGN KEY (TaskTemplateId) 
REFERENCES tasktemplate (TaskTemplateId)
ON DELETE CASCADE;

ALTER TABLE readdocumenttasktemplate 
ADD CONSTRAINT fk_readdocumenttasktemplate_tasktemplate FOREIGN KEY (TaskTemplateId) 
REFERENCES tasktemplate (TaskTemplateId)
ON DELETE CASCADE;

ALTER TABLE checklisttasktemplate
ADD CONSTRAINT fk_task_template_id
FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId)
ON DELETE CASCADE;

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_assignee_account_id
    FOREIGN KEY (AssigneeAccountId) REFERENCES account(AccountId);

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_assigner_account_id
    FOREIGN KEY (AssignerAccountId) REFERENCES account(AccountId);

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId)
    ON DELETE CASCADE;

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_workflow_instance_id
    FOREIGN KEY (WorkflowInstanceId) REFERENCES workflowinstance(Id);

ALTER TABLE taskinstance    
    ADD CONSTRAINT fk_task_instance_workflow_instance_node_id
    FOREIGN KEY (WorkflowInstanceNodeId) REFERENCES workflowinstancenode(Id);


ALTER TABLE checklisttaskinstance
    ADD CONSTRAINT fk_checklist_instance_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);

ALTER TABLE readdocumenttaskinstance
    ADD CONSTRAINT fk_readdoc_instance_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId)
    ON DELETE CASCADE; 

ALTER TABLE document
    ADD CONSTRAINT fk_document_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId)
    ON DELETE CASCADE;

ALTER TABLE document
    ADD CONSTRAINT fk_document_creator_id
    FOREIGN KEY (CreatorId) REFERENCES account(AccountId)
    ON DELETE CASCADE;

ALTER TABLE fileuploadtaskinstance
    ADD CONSTRAINT fk_file_upload_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);

ALTER TABLE fileuploadtaskinstance
    ADD CONSTRAINT fk_file_upload_document_id
    FOREIGN KEY (DocumentId) REFERENCES document(Id);

/* Workflow Template Node Constraints */
ALTER TABLE workflowtemplatenode
    ADD CONSTRAINT  fk_workflow_template_node_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId)
    ON DELETE CASCADE;

ALTER TABLE workflowtemplatenode
    ADD CONSTRAINT fk_workflow_template_node_workflow_template_id
    FOREIGN KEY (WorkflowTemplateId) REFERENCES workflowtemplate(Id)
    ON DELETE CASCADE;

/* No constraint for assigneId because of resevered placeholder ids i.e. onboarder */

ALTER TABLE nodetaskdependency
    ADD CONSTRAINT fk_nodetaskdependency_node_id
    FOREIGN KEY (NodeId) REFERENCES workflowtemplatenode(Id)
    ON DELETE CASCADE;

ALTER TABLE nodetaskdependency
    ADD CONSTRAINT fk_nodetaskdependency_dependency_node_id
    FOREIGN KEY (DependencyNodeId) REFERENCES workflowtemplatenode(Id)
    ON DELETE CASCADE;

ALTER TABLE nodetaskdependency
    ADD CONSTRAINT fk_nodetaskdependency_workflow_template_id
    FOREIGN KEY (WorkflowTemplateId) REFERENCES workflowtemplate(Id)
    ON DELETE CASCADE;

/* Workflow instance */
ALTER TABLE workflowinstance
    ADD CONSTRAINT fk_workflow_instance_workflow_template_id
    FOREIGN KEY (WorkflowTemplateId) REFERENCES workflowtemplate(Id)
    ON DELETE CASCADE;

ALTER TABLE workflowinstance
    ADD CONSTRAINT fk_workflow_instance_supervisor_account_id
    FOREIGN KEY (SupervisorAccountId) REFERENCES account(AccountId);

ALTER TABLE onboardingemployeedetails
    ADD CONSTRAINT fk_onboarding_employee_details_workflow_instance_id
    FOREIGN KEY (WorkflowInstanceId) REFERENCES workflowinstance(Id)
    ON DELETE CASCADE;

ALTER TABLE onboardingemployeedetails
    ADD CONSTRAINT fk_onboarding_employee_details_department_id
    FOREIGN KEY (DepartmentId) REFERENCES department(DepartmentId);

ALTER TABLE onboardingemployeedetails
    ADD CONSTRAINT fk_onboarder_account_id
    FOREIGN KEY (OnboarderAccountId) REFERENCES account(AccountId);

/* Workflow instance node */
ALTER TABLE workflowinstancenode
    ADD CONSTRAINT fk_node_instance_workflow_instance_id
    FOREIGN KEY (WorkflowInstanceId) REFERENCES workflowinstance(Id)
    ON DELETE CASCADE;

ALTER TABLE workflowinstancenode
    ADD CONSTRAINT fk_node_instance_workflow_template_id
    FOREIGN KEY (WorkflowTemplateId) REFERENCES workflowtemplate(Id)
    ON DELETE CASCADE;

ALTER TABLE workflowinstancenode
    ADD CONSTRAINT fk_node_instance_workflow_template_node_id
    FOREIGN KEY (WorkflowTemplateNodeId) REFERENCES workflowtemplatenode(Id)
    ON DELETE CASCADE;

ALTER TABLE workflowinstancenode
    ADD CONSTRAINT fk_node_instance_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId)
    ON DELETE CASCADE;

/* Comment table constraints */
ALTER TABLE comment
    ADD CONSTRAINT fk_comment_commenter_id
    FOREIGN KEY (CommenterId) REFERENCES account(AccountId)
    ON DELETE CASCADE;

ALTER TABLE comment
    ADD CONSTRAINT fk_comment__task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId);

ALTER TABLE comment
    ADD CONSTRAINT fk_comment_parent_comment_id
    FOREIGN KEY (ParentCommentId) REFERENCES comment(Id);

/* Document table constraints */
ALTER TABLE documentaccesslink
    ADD CONSTRAINT fk_document_access_document_id
    FOREIGN KEY (DocumentId) REFERENCES document(Id)
    ON DELETE CASCADE;

/* Project Task Template */
ALTER TABLE projectasktemplate
    ADD CONSTRAINT fk_project_task_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId)
    ON DELETE CASCADE;

ALTER TABLE projecttaskinstance
    ADD CONSTRAINT fk_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId)
    ON DELETE CASCADE;

/* Workflow instance Audit Log constraints */
ALTER TABLE workflowinstanceauditlog
    ADD CONSTRAINT fk_workflow_audit_logworkflow_instance_id
    FOREIGN KEY (WorkflowInstanceId) REFERENCES workflowinstance(Id)
    ON DELETE CASCADE;

ALTER TABLE workflowinstanceauditlog
    ADD CONSTRAINT fk_workflow_audit_log_account_id
    FOREIGN KEY (AccountId) REFERENCES account(AccountId)
    ON DELETE CASCADE;
    
/* Notification constraints */
ALTER TABLE notification
    ADD CONSTRAINT fk_recipient_id
    FOREIGN KEY (RecipientId) REFERENCES account(AccountId)
    ON DELETE CASCADE;