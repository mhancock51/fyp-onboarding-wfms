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
    OrganisationId VARCHAR(255)
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

INSERT INTO `onboarding-wfms-db`.`account` (`AccountId`, `DisplayName`, `EmailAddress`, `HashedPassword`, `IsOnboarder`, `IsAdmin`, `DepartmentId`) VALUES ('admin_user', 'Admin', 'admin@test.com', 'hash', '0', '1', 'dp_1');

ALTER TABLE organisationAdminLink 
ADD CONSTRAINT fk_orgAdminLink_organisation FOREIGN KEY (OrganisationId) REFERENCES organisation(OrganisationId);

ALTER TABLE organisationAdminLink 
ADD CONSTRAINT fk_orgAdminLink_account FOREIGN KEY (AccountId) REFERENCES account(AccountId);


ALTER TABLE account
ADD FOREIGN KEY (OrganisationId) REFERENCES organisation(OrganisationId);

ALTER TABLE account
ADD FOREIGN KEY (DepartmentId) REFERENCES department(DepartmentId);



