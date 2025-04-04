import AccountDirectory from '@/models/AccountDirectory'
import React from 'react'
import { Badge } from './ui/badge';
import { HoverCard, HoverCardContent, HoverCardTrigger } from './ui/hover-card';
import { Separator } from './ui/separator';
import { Label } from './ui/label';
import { ClipboardCopy, Copy, ShieldUser } from 'lucide-react';
import { toast } from 'sonner';

interface Props {
  accountDirectory: AccountDirectory | undefined;
}

export default function AccountDirectoryBadge(props: Props) {
  function copyEmailToClipboard() {
    if (props.accountDirectory?.emailAddress) {
      navigator.clipboard.writeText(props.accountDirectory?.emailAddress);
      toast.success("Copied email address to clipboard");
    }
  }

  return  (
    <HoverCard>
      <HoverCardTrigger>
        <Badge className='p-2 w-full rounded-full cursor-pointer'>
          {props.accountDirectory?.displayName}
        </Badge>
      </HoverCardTrigger>
      <HoverCardContent>
        <div className='p-2 flex flex-col gap-2 min-w-[200px]'>
          <div className='flex flex-row gap-2 items-center'>
            <h1 className='text-start'>{props.accountDirectory?.displayName}</h1>
            {props.accountDirectory?.isSupervisor ? <ShieldUser className='text-blue-500' size={20}/> : ""}
          </div>
          <Separator/>
          <div className='flex flex-row gap-2 items-center justify-start'>
            <Label className='font-normal text-sm'>{props.accountDirectory?.emailAddress}</Label>
            <Copy size={16} className='text-gray-500 cursor-pointer' onClick={copyEmailToClipboard}/>
          </div>
          <Label className='font-normal text-sm'>{props.accountDirectory?.departmentName}</Label>
        </div>
      </HoverCardContent>
    </HoverCard>
  )
}
