import { useEffect, useState } from 'react';
import Select, { ActionMeta, MultiValue, SingleValue } from 'react-select';
import Api from '@/api';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { toast } from 'sonner';

interface Props {
  templates: WorkflowTemplateDTO[];
  setTemplates: (templates: WorkflowTemplateDTO[]) => void;  
  filter?: (t: WorkflowTemplateDTO) => boolean;
  isMulti: boolean;
}

export default function WorkflowTemplateLookup(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);
  const [allTemplates, setAllTemplates] = useState<WorkflowTemplateDTO[]>([]);

  const filteredTemplates =
    props.filter !== undefined
      ? allTemplates.filter(props.filter)
      : allTemplates;

  function getTemplateDisplayString(template: WorkflowTemplateDTO): string {
    return `${template.name} ${template.isOnboardingWF ? '(onboarding)' : ''} (${template.numberOfTasks} tasks)`;
  }

  function getSelectOptions(): { label: string; value: string }[] {
    return filteredTemplates.map(function (template) {
      return {
        label: getTemplateDisplayString(template),
        value: template.id,
      };
    });
  }

  function getSelectValue(): any {
    const options = getSelectOptions();

    if (props.isMulti) {
      return options.filter(function (option) {
        return props.templates.some(function (t) {
          return t.id === option.value;
        });
      });
    } else {
      return options.find(function (option) {
        return option.value === props.templates[0]?.id;
      }) || null;
    }
  }

  function handleValueChange(
    newValue: MultiValue<{ label: string; value: string }> | SingleValue<{ label: string; value: string }>,
    actionMeta: ActionMeta<{ label: string; value: string }>
  ): void {
    if (props.isMulti) {
      const selectedIds = (newValue as MultiValue<{ label: string; value: string }>).map(function (v) {
        return v.value;
      });
      const selectedTemplates = filteredTemplates.filter(function (t) {
        return selectedIds.includes(t.id);
      });
      props.setTemplates(selectedTemplates);
    } else {
      const selectedId = (newValue as SingleValue<{ label: string; value: string }>)?.value;
      const selectedTemplate = filteredTemplates.find(function (t) {
        return t.id === selectedId;
      });
      props.setTemplates(selectedTemplate ? [selectedTemplate] : []);
    }
  }

  async function fetchWorkflowTemplates(): Promise<void> {
    setLoading(true);
    Api.workflowTemplates
      .fetchAllWorkflowTemplates()
      .then(function (response: AxiosResponse<HTTPresponse<WorkflowTemplateDTO[], string>>) {
        const data = response.data.data as WorkflowTemplateDTO[];
        setAllTemplates(
          data.filter(function (t) {
            return t.status.toLowerCase() !== 'archived';
          })
        );
      })
      .catch(function () {
        toast.error('Failed to load workflow templates');
      })
      .finally(function () {
        setLoading(false);
      });
  }

  useEffect(function () {
    void fetchWorkflowTemplates();
  }, []);

  return (
    <Select
      className='col-span-3'
      classNamePrefix='react-select'
      isMulti={props.isMulti}
      options={getSelectOptions()}
      value={getSelectValue()}
      isLoading={loading}
      onChange={handleValueChange}
      placeholder="Select a workflow template"
    />
  );
}
