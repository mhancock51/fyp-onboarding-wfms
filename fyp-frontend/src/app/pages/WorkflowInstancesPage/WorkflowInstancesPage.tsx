import WorkflowInstancesTable from '@/components/Tables/WorkflowInstancesTable'
import { Separator } from '@/components/ui/separator'
import React from 'react'

export default function WorkflowInstancesPage() {
  return (
    <div className='m-4 flex flex-col gap-4'>
      <div className='rounded-3xl bg-accent p-8' >
        <h1 className='text-xl text-foreground font-bold m-2'>Workflows Assinged To You</h1>
        <Separator/>
        <WorkflowInstancesTable/>
      </div>
    </div>
  )
}
