import TaskTemplate from '@/models/tasks/TaskTemplate'
import React from 'react'
import { Label } from './ui/label';
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate';
import { Separator } from './ui/separator';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import { PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT } from '@/constants';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';

export default function TaskTemplateView(props: { taskTemplate: TaskTemplate}) {
  return (
    <div className='flex flex-col gap-3'>
      <div className='grid grid-cols-4'>
        <Label>Task Template Name</Label>
        <Label className='col-span-3 font-normal'>{props.taskTemplate.name}</Label>                        
      </div>
      <div className='grid grid-cols-4'>
        <Label>Description</Label>
        <Label className='font-normal'>{props.taskTemplate.description}</Label>        
      </div>
      <div className='grid grid-cols-4'>
        <Label>Task Type</Label>
        <Label className='col-span-3 font-normal'>{props.taskTemplate.taskType.taskName}</Label>                        
      </div> 
      <Separator/>
      {
        props.taskTemplate.taskType.id === "checklist" &&
        <CheclistTaskTemplateView taskTemplateData={props.taskTemplate.taskTypeData as ChecklistTaskTemplate}/>
      }
      {
        props.taskTemplate.taskType.id === "read-document" &&
        <ReadDocumentTaskTemplateView taskTemplateData={props.taskTemplate.taskTypeData as ReadDocumentTaskTemplate}/>
      }
      {
        props.taskTemplate.taskType.id === "upload-document" &&
        <UploadDocumentTaskTemplateView taskTemplateData={props.taskTemplate.taskTypeData as FileUploadTaskTemplate}/>
      }
      {
        props.taskTemplate.taskType.id === "project-task" &&
        <ProjectTaskTemplateView taskTemplateData={props.taskTemplate.taskTypeData as ProjectTaskTemplate}/>
      }
    </div>
  )
}

function CheclistTaskTemplateView(props: {taskTemplateData: ChecklistTaskTemplate}) {
  return (
    <>
      <div className='grid grid-cols-4 items-start'>
        <Label>Items ({props.taskTemplateData.items.length})</Label>
        <div className='flex flex-col gap-1 col-span-3'>
          {
            props.taskTemplateData.items.map((item, index) => (
              <Label className='font-normal' key={index}>[{index + 1}] {item}</Label>
            ))
          }
        </div>
      </div>
    </>
  )
}

function ReadDocumentTaskTemplateView(props: {taskTemplateData: ReadDocumentTaskTemplate}) {
  return (
    <>
      <div className='grid grid-cols-4 items-start'>
        <Label>Document Name</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.documentName}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Document URL</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.documentUrl}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Checkbox Label</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.checkBoxLabel}</Label>
      </div>
    </>
  )
}

function UploadDocumentTaskTemplateView(props: {taskTemplateData: FileUploadTaskTemplate}) {
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  return (
    <>
      <div className='grid grid-cols-4 items-start'>
        <Label>Document Name</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.documentName}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Allowed Document Types</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.supportedDocumentType.split(";").join(", ")}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Accessible to</Label>
        <div className='flex flex-col gap-1 col-span-3'>
          {
            props.taskTemplateData.accessAccountIds.map((accountId, index) => (
              <Label className='col-span-3 font-normal' key={index}>{accounts.concat([PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT]).find(a => a.id === accountId)?.displayName}</Label>
            ))
          }
        </div>
      </div>
    </>
  )
}

function ProjectTaskTemplateView(props: {taskTemplateData: ProjectTaskTemplate}) {
  return (
    <>
      <div className='grid grid-cols-4 items-start'>
        <Label>Brief</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.brief}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Deliverable</Label>
        <Label className='font-normal col-span-3'>{props.taskTemplateData.deliverable}</Label>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Objectives ({props.taskTemplateData.objectives.length})</Label>
        <div className='flex flex-col gap-1 col-span-3'>
          {
            props.taskTemplateData.objectives.map((objective, index) => (
              <Label className='font-normal' key={index}>[{index + 1}] {objective.objective} {objective.required ? "*" : ""}</Label>
            ))
          }
        </div>
      </div>
      <div className='grid grid-cols-4 items-start'>
        <Label>Support Links ({props.taskTemplateData.supportLinks.length})</Label>
        <div className='flex flex-col gap-2 col-span-3'>
          {
            props.taskTemplateData.supportLinks.map((supportLink, index) => (
              <div className='flex flex-col gap-2' key={index}>
                <div className='flex flex-row w-full items-start'>
                  <Label className='font-normal flex-2'>Link:</Label>
                  <Label className='font-normal flex-10'>{supportLink.link.substring(0, 100)}</Label>
                </div>
                <div className='flex flex-row w-full items-start'>
                  <Label className='font-normal flex-2'>Link Label:</Label>
                  <Label className='font-normal flex-10'>{supportLink.linkLabel}</Label>
                </div>
                <div className='flex flex-row w-full items-start'>
                  <Label className='font-normal flex-2'>Description:</Label>
                  <Label className='font-normal flex-10'>{supportLink.description}</Label>
                </div>
                <Separator/>
              </div>              
            ))
          }
        </div>
      </div>
    </>
  )
}
