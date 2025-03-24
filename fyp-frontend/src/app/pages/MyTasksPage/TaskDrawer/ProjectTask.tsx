import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsTrigger } from '@/components/ui/tabs';
import { Label } from '@radix-ui/react-dropdown-menu';
import { TabsList } from '@radix-ui/react-tabs';
import React from 'react'

interface Props {
  taskInstanceId: string;
  // checklistInstance: ChecklistTaskInstance;
  // checklistTemplate: ChecklistTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}
export default function ProjectTask(props: Props) {
  return (
    <Tabs defaultValue="brief" className="w-full">
      <TabsList className="grid w-full grid-cols-2">
        <TabsTrigger value="brief">Project Brief</TabsTrigger>
        <TabsTrigger value="support">Support</TabsTrigger>
      </TabsList>
      <TabsContent value="brief">
        <div className='flex flex-col gap-2 p-2'>
          <div className='flex flex-col'>
            <Label className='font-bold'>Project Brief</Label>
            <Label className='font-normal'>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec dictum arcu augue, ac facilisis augue venenatis vel. 
              Suspendisse ex enim, consequat vel dolor sed, mattis semper lacus. Fusce at pulvinar augue. 
              Mauris urna risus, dictum ac consectetur ut, ornare non neque. Morbi placerat odio eget ligula commodo tincidunt.
            </Label>
          </div>
          <div className='flex flex-col'>
            <Label className='font-bold'>Deliverable</Label>
            <Badge className='mx-2 my-1 py-2 px-4 rounded-full'>
              A really cool react web app
            </Badge>
          </div>
          <Separator/>
          <div className='flex flex-col'>
            <Label className='font-bold'>Objectives</Label>
            <div className='flex flex-col gap-1 w-full mx-2 my-1'>
            {
              ["Learn what react.js is", "Learn to make a HTTP get request with axios", "Create a react app"]
              .map((item, index) => (
                <div key={index} className='flex flex-row gap-4 items-center'>
                  <Checkbox/>
                  <Label>{item}</Label>
                </div>
              ))  
            }
            </div>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="support">
        <Label className='font-bold'>Supporting Documents</Label>

      </TabsContent>
    </Tabs>
  )
}
