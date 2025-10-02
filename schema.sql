-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: onboarding-wfms-db-4
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `AccountId` varchar(255) NOT NULL,
  `DisplayName` varchar(255) NOT NULL,
  `EmailAddress` varchar(255) NOT NULL,
  `HashedPassword` varchar(255) NOT NULL,
  `IsSupervisor` tinyint(1) NOT NULL,
  `DepartmentId` varchar(255) NOT NULL,
  `OrganisationId` varchar(255) NOT NULL,
  `AccountStatus` varchar(255) NOT NULL,
  PRIMARY KEY (`AccountId`),
  KEY `account_ibfk_1` (`OrganisationId`),
  KEY `account_ibfk_2` (`DepartmentId`),
  CONSTRAINT `account_ibfk_1` FOREIGN KEY (`OrganisationId`) REFERENCES `organisation` (`OrganisationId`),
  CONSTRAINT `account_ibfk_2` FOREIGN KEY (`DepartmentId`) REFERENCES `department` (`DepartmentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `checklisttaskinstance`
--

DROP TABLE IF EXISTS `checklisttaskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `checklisttaskinstance` (
  `Id` varchar(255) NOT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `ItemCompletionStatuses` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_checklist_instance_task_instance_id` (`TaskInstanceId`),
  CONSTRAINT `fk_checklist_instance_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `checklisttasktemplate`
--

DROP TABLE IF EXISTS `checklisttasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `checklisttasktemplate` (
  `Id` varchar(255) NOT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `Items` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_task_template_id` (`TaskTemplateId`),
  CONSTRAINT `fk_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `comment`
--

DROP TABLE IF EXISTS `comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comment` (
  `Id` varchar(255) NOT NULL,
  `CommenterId` varchar(255) DEFAULT NULL,
  `Text` text,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `CreationTimestamp` datetime DEFAULT NULL,
  `ParentCommentId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_comment_commenter_id` (`CommenterId`),
  KEY `fk_comment__task_template_id` (`TaskTemplateId`),
  KEY `fk_comment_parent_comment_id` (`ParentCommentId`),
  CONSTRAINT `fk_comment__task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`),
  CONSTRAINT `fk_comment_commenter_id` FOREIGN KEY (`CommenterId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_comment_parent_comment_id` FOREIGN KEY (`ParentCommentId`) REFERENCES `comment` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `department`
--

DROP TABLE IF EXISTS `department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `department` (
  `DepartmentId` varchar(255) NOT NULL,
  `DisplayName` varchar(255) NOT NULL,
  PRIMARY KEY (`DepartmentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `document`
--

DROP TABLE IF EXISTS `document`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `document` (
  `Id` varchar(255) NOT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `CreatorId` varchar(255) DEFAULT NULL,
  `WorkflowInstanceId` varchar(255) DEFAULT NULL,
  `DocumentData` longblob,
  `FileExtension` varchar(50) DEFAULT NULL,
  `UploadTimestamp` datetime DEFAULT NULL,
  `FileName` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_document_creator_id` (`CreatorId`),
  KEY `fk_document_workflow_instance_idx` (`WorkflowInstanceId`),
  KEY `fk_document_task_instance_id` (`TaskInstanceId`),
  CONSTRAINT `fk_document_creator_id` FOREIGN KEY (`CreatorId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_document_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE,
  CONSTRAINT `fk_document_workflow_instance` FOREIGN KEY (`WorkflowInstanceId`) REFERENCES `workflowinstance` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `documentaccesslink`
--

DROP TABLE IF EXISTS `documentaccesslink`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentaccesslink` (
  `Id` varchar(255) NOT NULL,
  `AccountId` varchar(255) DEFAULT NULL,
  `DocumentId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_document_access_document_id` (`DocumentId`),
  CONSTRAINT `fk_document_access_document_id` FOREIGN KEY (`DocumentId`) REFERENCES `document` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feedbacktaskinstance`
--

DROP TABLE IF EXISTS `feedbacktaskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feedbacktaskinstance` (
  `id` varchar(255) NOT NULL,
  `taskInstanceId` varchar(255) DEFAULT NULL,
  `responses` json DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `taskInstanceId` (`taskInstanceId`),
  CONSTRAINT `feedbacktaskinstance_ibfk_1` FOREIGN KEY (`taskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feedbacktasktemplate`
--

DROP TABLE IF EXISTS `feedbacktasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feedbacktasktemplate` (
  `id` varchar(255) NOT NULL,
  `taskTemplateId` varchar(255) DEFAULT NULL,
  `questions` json DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `taskTemplateId` (`taskTemplateId`),
  CONSTRAINT `feedbacktasktemplate_ibfk_1` FOREIGN KEY (`taskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fileuploadtaskinstance`
--

DROP TABLE IF EXISTS `fileuploadtaskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fileuploadtaskinstance` (
  `Id` varchar(255) NOT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `DocumentId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_file_upload_task_instance_id` (`TaskInstanceId`),
  KEY `fk_file_upload__document_id` (`DocumentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fileuploadtasktemplate`
--

DROP TABLE IF EXISTS `fileuploadtasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fileuploadtasktemplate` (
  `Id` varchar(255) NOT NULL,
  `TaskTemplateId` varchar(255) NOT NULL,
  `SupportedDocumentType` varchar(255) NOT NULL,
  `DocumentName` varchar(255) NOT NULL,
  `AccessAccountIds` json NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_fileuploadtasktemplate_tasktemplate` (`TaskTemplateId`),
  CONSTRAINT `fk_fileuploadtasktemplate_tasktemplate` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nodetaskdependency`
--

DROP TABLE IF EXISTS `nodetaskdependency`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nodetaskdependency` (
  `Id` varchar(255) NOT NULL,
  `NodeId` varchar(255) DEFAULT NULL,
  `DependencyNodeId` varchar(255) DEFAULT NULL,
  `WorkflowTemplateId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_nodetaskdependency_node_id` (`NodeId`),
  KEY `fk_nodetaskdependency_dependency_node_id` (`DependencyNodeId`),
  KEY `fk_nodetaskdependency_workflow_template_id` (`WorkflowTemplateId`),
  CONSTRAINT `fk_nodetaskdependency_dependency_node_id` FOREIGN KEY (`DependencyNodeId`) REFERENCES `workflowtemplatenode` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_nodetaskdependency_node_id` FOREIGN KEY (`NodeId`) REFERENCES `workflowtemplatenode` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_nodetaskdependency_workflow_template_id` FOREIGN KEY (`WorkflowTemplateId`) REFERENCES `workflowtemplate` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `notification`
--

DROP TABLE IF EXISTS `notification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notification` (
  `Id` varchar(255) NOT NULL,
  `RecipientId` varchar(255) DEFAULT NULL,
  `Status` varchar(255) DEFAULT NULL,
  `Description` text,
  `Tags` json DEFAULT NULL,
  `Timestamp` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_recipient_id` (`RecipientId`),
  CONSTRAINT `fk_recipient_id` FOREIGN KEY (`RecipientId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `onboardingemployeedetails`
--

DROP TABLE IF EXISTS `onboardingemployeedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `onboardingemployeedetails` (
  `Id` varchar(255) NOT NULL,
  `WorkflowInstanceId` varchar(255) DEFAULT NULL,
  `DisplayName` varchar(255) DEFAULT NULL,
  `EmailAddress` varchar(255) DEFAULT NULL,
  `DepartmentId` varchar(255) DEFAULT NULL,
  `OnboarderAccountId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_onboarding_employee_details_department_id` (`DepartmentId`),
  KEY `fk_onboarding_employee_details_workflow_instance_id` (`WorkflowInstanceId`),
  KEY `fk_onboarder_account_id` (`OnboarderAccountId`),
  CONSTRAINT `fk_onboarder_account_id` FOREIGN KEY (`OnboarderAccountId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_onboarding_employee_details_department_id` FOREIGN KEY (`DepartmentId`) REFERENCES `department` (`DepartmentId`),
  CONSTRAINT `fk_onboarding_employee_details_workflow_instance_id` FOREIGN KEY (`WorkflowInstanceId`) REFERENCES `workflowinstance` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organisation`
--

DROP TABLE IF EXISTS `organisation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `organisation` (
  `OrganisationId` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  PRIMARY KEY (`OrganisationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `projectasktemplate`
--

DROP TABLE IF EXISTS `projectasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectasktemplate` (
  `Id` varchar(255) NOT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `Brief` text,
  `Objectives` json DEFAULT NULL,
  `Skills` json DEFAULT NULL,
  `SupportLinks` json DEFAULT NULL,
  `Deliverable` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_project_task_task_template_id` (`TaskTemplateId`),
  CONSTRAINT `fk_project_task_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `projecttaskinstance`
--

DROP TABLE IF EXISTS `projecttaskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projecttaskinstance` (
  `Id` varchar(255) NOT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `ObjectiveStates` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_task_instance_id` (`TaskInstanceId`),
  CONSTRAINT `fk_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `readdocumenttaskinstance`
--

DROP TABLE IF EXISTS `readdocumenttaskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `readdocumenttaskinstance` (
  `Id` varchar(255) NOT NULL,
  `TaskInstanceId` varchar(255) DEFAULT NULL,
  `CheckboxChecked` tinyint(1) DEFAULT NULL,
  `LinkClicked` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_readdoc_instance_task_instance_id` (`TaskInstanceId`),
  CONSTRAINT `fk_readdoc_instance_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `readdocumenttasktemplate`
--

DROP TABLE IF EXISTS `readdocumenttasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `readdocumenttasktemplate` (
  `Id` varchar(255) NOT NULL,
  `TaskTemplateId` varchar(255) NOT NULL,
  `DocumentName` varchar(255) NOT NULL,
  `DocumentUrl` varchar(2083) NOT NULL,
  `CheckBoxLabel` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_readdocumenttasktemplate_tasktemplate` (`TaskTemplateId`),
  CONSTRAINT `fk_readdocumenttasktemplate_tasktemplate` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reportedissue`
--

DROP TABLE IF EXISTS `reportedissue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
  KEY `fk_reported_issue_task_instance_id` (`TaskInstanceId`),
  KEY `fk_reported_issue_task_template_id` (`TaskTemplateId`),
  CONSTRAINT `fk_reported_issue_issue_creator_id` FOREIGN KEY (`IssueCreatorId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_reported_issue_task_instance_id` FOREIGN KEY (`TaskInstanceId`) REFERENCES `taskinstance` (`TaskInstanceId`) ON DELETE CASCADE,
  CONSTRAINT `fk_reported_issue_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taskinstance`
--

DROP TABLE IF EXISTS `taskinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `taskinstance` (
  `TaskInstanceId` varchar(255) NOT NULL,
  `AssigneeAccountId` varchar(255) DEFAULT NULL,
  `AssignerAccountId` varchar(255) DEFAULT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `CreationTimestamp` datetime DEFAULT NULL,
  `Status` varchar(255) DEFAULT NULL,
  `WorkflowInstanceId` varchar(255) DEFAULT NULL,
  `WorkflowNodeId` varchar(255) DEFAULT NULL,
  `DueDate` datetime DEFAULT NULL,
  `WorkflowInstanceNodeId` varchar(255) DEFAULT NULL,
  `CompletionTimestamp` datetime DEFAULT NULL,
  PRIMARY KEY (`TaskInstanceId`),
  KEY `fk_task_instance_assignee_account_id` (`AssigneeAccountId`),
  KEY `fk_task_instance_assigner_account_id` (`AssignerAccountId`),
  KEY `fk_task_instance_workflow_instance_id` (`WorkflowInstanceId`),
  KEY `fk_task_instance_workflow_node_id` (`WorkflowNodeId`),
  KEY `fk_task_instance_task_template_id` (`TaskTemplateId`),
  KEY `fk_task_instance_workflow_instance_node_id` (`WorkflowInstanceNodeId`),
  CONSTRAINT `fk_task_instance_assignee_account_id` FOREIGN KEY (`AssigneeAccountId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_task_instance_assigner_account_id` FOREIGN KEY (`AssignerAccountId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_task_instance_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE,
  CONSTRAINT `fk_task_instance_workflow_instance_id` FOREIGN KEY (`WorkflowInstanceId`) REFERENCES `workflowinstance` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_task_instance_workflow_instance_node_id` FOREIGN KEY (`WorkflowInstanceNodeId`) REFERENCES `workflowinstancenode` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_task_instance_workflow_node_id` FOREIGN KEY (`WorkflowNodeId`) REFERENCES `workflowtemplatenode` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tasktemplate`
--

DROP TABLE IF EXISTS `tasktemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tasktemplate` (
  `TaskTemplateId` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Description` text,
  `CreatorAccountId` varchar(255) NOT NULL,
  `DateCreated` datetime NOT NULL,
  `TaskTypeId` varchar(255) NOT NULL,
  `Status` varchar(255) NOT NULL,
  `LastModifiedTimestamp` datetime DEFAULT NULL,
  PRIMARY KEY (`TaskTemplateId`),
  UNIQUE KEY `Name_UNIQUE` (`Name`),
  KEY `fk_tasktemplate_creator` (`CreatorAccountId`),
  KEY `fk_tasktemplate_tasktype` (`TaskTypeId`),
  CONSTRAINT `fk_tasktemplate_creator` FOREIGN KEY (`CreatorAccountId`) REFERENCES `account` (`AccountId`),
  CONSTRAINT `fk_tasktemplate_tasktype` FOREIGN KEY (`TaskTypeId`) REFERENCES `tasktype` (`TaskTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tasktype`
--

DROP TABLE IF EXISTS `tasktype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tasktype` (
  `TaskTypeId` varchar(255) NOT NULL,
  `TaskName` varchar(255) NOT NULL,
  PRIMARY KEY (`TaskTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowinstance`
--

DROP TABLE IF EXISTS `workflowinstance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workflowinstance` (
  `Id` varchar(255) NOT NULL,
  `WorkflowTemplateId` varchar(255) NOT NULL,
  `SupervisorAccountId` varchar(255) DEFAULT NULL,
  `CreationTimestamp` datetime DEFAULT NULL,
  `MainflowStartTimestamp` datetime DEFAULT NULL,
  `CompletionTimestamp` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_workflow_instance_workflow_template_id` (`WorkflowTemplateId`),
  KEY `fk_workflow_instance_supervisor_account_id` (`SupervisorAccountId`),
  CONSTRAINT `fk_workflow_instance_supervisor_account_id` FOREIGN KEY (`SupervisorAccountId`) REFERENCES `account` (`AccountId`),
  CONSTRAINT `fk_workflow_instance_workflow_template_id` FOREIGN KEY (`WorkflowTemplateId`) REFERENCES `workflowtemplate` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowinstanceauditlog`
--

DROP TABLE IF EXISTS `workflowinstanceauditlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workflowinstanceauditlog` (
  `Id` varchar(255) NOT NULL,
  `WorkflowInstanceId` varchar(255) DEFAULT NULL,
  `Log` text,
  `Timestamp` datetime DEFAULT NULL,
  `AccountId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_workflow_audit_logworkflow_instance_id` (`WorkflowInstanceId`),
  KEY `fk_workflow_audit_log_account_id` (`AccountId`),
  CONSTRAINT `fk_workflow_audit_log_account_id` FOREIGN KEY (`AccountId`) REFERENCES `account` (`AccountId`) ON DELETE CASCADE,
  CONSTRAINT `fk_workflow_audit_logworkflow_instance_id` FOREIGN KEY (`WorkflowInstanceId`) REFERENCES `workflowinstance` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowinstancenode`
--

DROP TABLE IF EXISTS `workflowinstancenode`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workflowinstancenode` (
  `Id` varchar(255) NOT NULL,
  `WorkflowInstanceId` varchar(255) DEFAULT NULL,
  `WorkflowTemplateId` varchar(255) DEFAULT NULL,
  `Status` varchar(255) DEFAULT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `WorkflowTemplateNodeId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_workflow_instance_id` (`WorkflowInstanceId`),
  KEY `fk_workflow_template_id` (`WorkflowTemplateId`),
  KEY `fk_node_instance_task_template_id` (`TaskTemplateId`),
  KEY `fk_node_instance_workflow_template_node_id` (`WorkflowTemplateNodeId`),
  CONSTRAINT `fk_node_instance_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE,
  CONSTRAINT `fk_node_instance_workflow_template_node_id` FOREIGN KEY (`WorkflowTemplateNodeId`) REFERENCES `workflowtemplatenode` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_workflow_instance_id` FOREIGN KEY (`WorkflowInstanceId`) REFERENCES `workflowinstance` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_workflow_template_id` FOREIGN KEY (`WorkflowTemplateId`) REFERENCES `workflowtemplate` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowtemplate`
--

DROP TABLE IF EXISTS `workflowtemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workflowtemplate` (
  `Id` varchar(255) NOT NULL,
  `IsOnboardingWF` tinyint(1) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Description` text,
  `Status` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowtemplatenode`
--

DROP TABLE IF EXISTS `workflowtemplatenode`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workflowtemplatenode` (
  `Id` varchar(255) NOT NULL,
  `WorkflowTemplateId` varchar(255) DEFAULT NULL,
  `TaskTemplateId` varchar(255) DEFAULT NULL,
  `AssigneeId` varchar(255) DEFAULT NULL,
  `Order` int DEFAULT NULL,
  `WorkflowSection` varchar(255) DEFAULT NULL,
  `DaysUntilDue` int DEFAULT NULL,
  `AccountsToNotify` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_workflow_template_node_task_template_id` (`TaskTemplateId`),
  KEY `fk_workflow_template_node_workflow_template_id` (`WorkflowTemplateId`),
  CONSTRAINT `fk_workflow_template_node_task_template_id` FOREIGN KEY (`TaskTemplateId`) REFERENCES `tasktemplate` (`TaskTemplateId`) ON DELETE CASCADE,
  CONSTRAINT `fk_workflow_template_node_workflow_template_id` FOREIGN KEY (`WorkflowTemplateId`) REFERENCES `workflowtemplate` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-12 21:33:57


INSERT INTO `organisation` (`OrganisationId`, `Name`) VALUES ('organisation', 'My Org');

INSERT INTO `tasktype` (`TaskTypeId`, `TaskName`) VALUES ('checklist', 'Checklist');
INSERT INTO `tasktype` (`TaskTypeId`, `TaskName`) VALUES ('upload-document', 'Upload Document');
INSERT INTO `tasktype` (`TaskTypeId`, `TaskName`) VALUES ('read-document', 'Read Document');
INSERT INTO `tasktype` (`TaskTypeId`, `TaskName`) VALUES ('project-task', 'Project Task');
INSERT INTO `tasktype` (`TaskTypeId`, `TaskName`) VALUES ('feedback', 'Feedback Task');