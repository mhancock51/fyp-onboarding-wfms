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



