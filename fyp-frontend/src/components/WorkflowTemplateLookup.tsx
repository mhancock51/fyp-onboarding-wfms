import Api from '@/api';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from './ui/select';

interface Props {
  value: WorkflowTemplateDTO | null;
  setValue: React.Dispatch<React.SetStateAction<WorkflowTemplateDTO | null>>;
}

export default function WorkflowTemplateLookup(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);
  const [loaded, setLoaded] = useState<boolean>(false);
  const [templates, setTemplates] = useState<WorkflowTemplateDTO[]>([]);

  async function fetchAllWorkflowTemplates() {
    setLoading(true);
    Api.fetchAllWorkflowTemplates()
    .then((response: AxiosResponse<HTTPresponse<WorkflowTemplateDTO[], string>>) => {
      setTemplates(response.data.data);
    })
    .catch((error) => {
      toast.error("Failed to load workflow templates");
    })
    .finally(() => {
      setLoading(false);
      setLoaded(true);
    })
  }

  useEffect(() => {
    void fetchAllWorkflowTemplates();
  }, [])

  return (
    <Select required value={props.value?.id ?? ""} onValueChange={(value: string) => {props.setValue(templates.find(i => i.id === value) ?? null)}}>
      <SelectTrigger className='flex-8 w-[180px]'>
        <SelectValue placeholder="Select a workflow template" />
      </SelectTrigger>
      <SelectContent>
        {
          !loading &&
          <SelectGroup>
            <SelectLabel>Workflow Templates</SelectLabel>
            {
              templates.map((template, index) => (
                <SelectItem key={index} value={template.id}>{template.name} {template.isOnboardingWF ? `(onboarding)` : ""} ({template.numberOfTasks} tasks)</SelectItem>
              ))
            }          
          </SelectGroup>
        }
        {
          loading &&
          <SelectGroup>
            <SelectLabel>Loading workflow templates...</SelectLabel>
          </SelectGroup>
        }
      </SelectContent>
    </Select> 
  )
}
