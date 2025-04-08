import { Inbox } from 'lucide-react'
import React from 'react'

export default function NoResults(props: {text: string}) {
  return (
    <div className='flex flex-col gap-2 justify-center mx-auto w-auto items-center p-4'>
      <Inbox size={40}/>
      <h1>{props.text}</h1>
    </div>
  )
}
