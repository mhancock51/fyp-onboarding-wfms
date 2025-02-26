import { Button } from '@/components/ui/button'
import { useNavigate } from 'react-router'

export default function WorkflowsPage() {
  const navigate = useNavigate();

  return (
    <div>
      <h1>Workflows</h1>
      <Button variant={"default"} className='cursor-pointer' onClick={() => { navigate("/workflows-create")}}>Create Workflow</Button>
    </div>
  )
}
