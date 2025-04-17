import AccountDirectory from '@/models/AccountDirectory'
import React from 'react'
import { Badge } from './ui/badge';
import { HoverCard, HoverCardContent, HoverCardTrigger } from './ui/hover-card';
import { Separator } from './ui/separator';
import { Label } from './ui/label';
import { ClipboardCopy, Copy, ShieldUser } from 'lucide-react';
import { toast } from 'sonner';
import clsx from 'clsx';

interface Props {
  accountDirectory: AccountDirectory | undefined;
  className?: string;
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
        {
          props.accountDirectory !== undefined &&
          <Badge className={clsx('p-2 w-full rounded-full cursor-pointer min-w-[125px]', props.className)}>
            {props.accountDirectory?.displayName}
          </Badge>
        }
        {
          props.accountDirectory === undefined && 
          <div className='w-full flex flex-row justify-center text-[12px] items-center min-w-[125px]'>Not found</div>
        }
      </HoverCardTrigger>
      {
        props.accountDirectory !== undefined &&
        <HoverCardContent className='p-2'>
          <div className='flex flex-col gap-1 min-w-[250px]'>
            <div className='flex flex-row gap-2 items-center'>
              <h1 className='text-start text-base'>{props.accountDirectory?.displayName}</h1>
              {props.accountDirectory?.isSupervisor ? <ShieldUser className='text-blue-500' size={20}/> : ""}
            </div>
            <Separator/>
            {
              props.accountDirectory?.emailAddress !== undefined &&
              <div className='flex flex-row gap-2 items-center justify-start'>
                <Label className='font-normal text-sm'>{props.accountDirectory?.emailAddress}</Label>
                <Copy size={16} className='text-gray-500 cursor-pointer' onClick={copyEmailToClipboard}/>
              </div>
            }
            <Label className='font-normal text-sm'>{props.accountDirectory?.departmentName}</Label>
          </div>
        </HoverCardContent>
      }
    </HoverCard>
  )
}
