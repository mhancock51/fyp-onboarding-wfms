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
        <Badge className='p-2 w-full rounded-full cursor-pointer min-w-[125px]'>
          {props.accountDirectory?.displayName}
        </Badge>
      </HoverCardTrigger>
      <HoverCardContent className='p-2'>
        <div className='flex flex-col gap-1 min-w-[250px]'>
          <div className='flex flex-row gap-2 items-center'>
            <h1 className='text-start text-base'>{props.accountDirectory?.displayName}</h1>
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
