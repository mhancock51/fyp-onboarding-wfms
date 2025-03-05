import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { ReadDocumentTaskTemplate } from '@/models/ReadDocumentTaskTemplate';
import { CheckedState } from '@radix-ui/react-checkbox';
import { Link } from 'lucide-react';
import React, { useEffect, useState } from 'react'

interface Props {
  taskInstanceId: string;
  readDocumentInstance: any;
  readDocumentTemplate: ReadDocumentTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}

export default function ReadDocumentTask(props: Props) {
  const [read, setRead] = useState<boolean>(false);
  const [linkClicked, setLinkClicked] = useState<boolean>(false);

  useEffect(() => {
    props.setCanCompleteTask(read);
  }, [read]);

  function redirectToDocumentLink() {
    window.open(props.readDocumentTemplate.documentUrl, '_blank');
    setLinkClicked(true);
  }

  return (
    <div className='flex flex-col gap-2 p-2'>
      <Button onClick={redirectToDocumentLink}>Read Document <Link/></Button>
      <div className='flex flex-row gap-2 mx-auto'>
        <Checkbox disabled={!linkClicked} checked={read} onCheckedChange={(checked: CheckedState) => { setRead(checked as boolean);}}/>
        <Label>{props.readDocumentTemplate.checkBoxLabel}</Label>
      </div>  
    </div>  
  )
}
