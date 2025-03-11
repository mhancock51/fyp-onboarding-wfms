CREATE TABLE department (
    DepartmentId VARCHAR(255) PRIMARY KEY,
    DisplayName VARCHAR(255) NOT NULL
);
CREATE TABLE account (
    AccountId VARCHAR(255) PRIMARY KEY,
    DisplayName VARCHAR(255) NOT NULL,
    EmailAddress VARCHAR(255) NOT NULL,
    HashedPassword VARCHAR(255) NOT NULL,
    IsOnboarder TINYINT(1) NOT NULL,
    IsAdmin TINYINT(1) NOT NULL,
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
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    CreatorAccountId VARCHAR(255) NOT NULL,
    DateCreated DATETIME NOT NULL,
    TaskTypeId VARCHAR(255) NOT NULL,
    PRIMARY KEY (TaskTemplateId)
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
    CreationTimestamp DATETIME,
    Status VARCHAR(255)
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

CREATE TABLE document (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    CreatorId VARCHAR(255),
    WorkflowInstanceId VARCHAR(255),
    DocumentData LONGBLOB,
    FileExtension VARCHAR(50),
    UploadTimestamp DATETIME,
    FileName VARCHAR(255)
);

CREATE TABLE fileuploadtaskinstance (
    Id VARCHAR(255) PRIMARY KEY,
    TaskInstanceId VARCHAR(255),
    DocumentId VARCHAR(255),
    UploadedTimestamp DATETIME
);

/* Everything workflow template related */
CREATE TABLE workflowtemplate (
    Id VARCHAR(255) PRIMARY KEY,
    IsOnboardingWF BOOLEAN,
    Name VARCHAR(255),
    Description TEXT
);

CREATE TABLE workflowtemplatenode (
    Id VARCHAR(255) PRIMARY KEY,
    WorkflowTemplateId VARCHAR(255),
    TaskTemplateId VARCHAR(255),
    AssigneeId VARCHAR(255),
    `Order` INT,
    WorkflowSection VARCHAR(255)
);

CREATE TABLE nodetaskdependency (
    Id VARCHAR(255) PRIMARY KEY,
    NodeId VARCHAR(255),
    DependencyNodeId VARCHAR(255)
);


INSERT INTO `onboarding-wfms-db`.`organisation` (`OrganisationId`, `Name`) VALUES ('organisation', '[EMPTY]');
INSERT INTO `onboarding-wfms-db`.`department` (`DepartmentId`, `DisplayName`) VALUES ('admin','Admin');
INSERT INTO `onboarding-wfms-db`.`department` (`DepartmentId`, `DisplayName`) VALUES ('onboarder','Onboarder');

INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`checklist`, `Checklist`);
INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`upload-document`, `Upload Document`);
INSERT INTO `onboarding-wfms-db`.`tasktype` (`TaskTypeId`, `TaskName`) VALUES (`read-document`, `Read Document`);

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
REFERENCES tasktemplate (TaskTemplateId);

ALTER TABLE readdocumenttasktemplate 
ADD CONSTRAINT fk_readdocumenttasktemplate_tasktemplate FOREIGN KEY (TaskTemplateId) 
REFERENCES tasktemplate (TaskTemplateId);

ALTER TABLE checklisttasktemplate
    ADD CONSTRAINT fk_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId);


ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_assignee_account_id
    FOREIGN KEY (AssigneeAccountId) REFERENCES account(AccountId);

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_assigner_account_id
    FOREIGN KEY (AssignerAccountId) REFERENCES account(AccountId);

ALTER TABLE taskinstance
    ADD CONSTRAINT fk_task_instance_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId);

ALTER TABLE checklisttaskinstance
    ADD CONSTRAINT fk_checklist_instance_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);

ALTER TABLE readdocumenttaskinstance
    ADD CONSTRAINT fk_readdoc_instance_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);    

ALTER TABLE document
    ADD CONSTRAINT fk_document_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);

ALTER TABLE document
    ADD CONSTRAINT fk_document_creator_id
    FOREIGN KEY (CreatorId) REFERENCES account(AccountId);

ALTER TABLE fileuploadtaskinstance
    ADD CONSTRAINT fk_file_upload_task_instance_id
    FOREIGN KEY (TaskInstanceId) REFERENCES taskinstance(TaskInstanceId);

ALTER TABLE fileuploadtaskinstance
    ADD CONSTRAINT fk_file_upload_document_id
    FOREIGN KEY (DocumentId) REFERENCES document(Id);

/* Workflow Template Node Constraints */
ALTER TABLE workflowtemplatenode
    ADD CONSTRAINT fk_workflow_template_node_workflow_template_id
    FOREIGN KEY (WorkflowTemplateId) REFERENCES workflowtemplate(Id);

ALTER TABLE workflowtemplatenode
    ADD CONSTRAINT  fk_workflow_template_node_task_template_id
    FOREIGN KEY (TaskTemplateId) REFERENCES tasktemplate(TaskTemplateId);

ALTER TABLE workflowtemplatenode
    ADD CONSTRAINT fk_workflow_template_node_assignee_id
    FOREIGN KEY (AssigneeId) REFERENCES account(AccountId);

ALTER TABLE nodetaskdependency
    ADD CONSTRAINT fk_task_dependency_node_id
    FOREIGN KEY (NodeId) REFERENCES workflowtemplatenode(Id);

ALTER TABLE nodetaskdependency
    ADD CONSTRAINT fk_task_dependency_dependency_node_id
    FOREIGN KEY (DependencyNodeId) REFERENCES workflowtemplatenode(Id);
