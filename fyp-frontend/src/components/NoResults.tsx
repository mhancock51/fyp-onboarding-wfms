import { Inbox } from 'lucide-react'
import React from 'react'

export default function NoResults(props: {text: string}) {
  return (
    <div className='bg-sidebar flex flex-col gap-2 justify-center mx-auto w-100 items-center p-4'>
      <Inbox size={40}/>
      <h1>{props.text}</h1>
    </div>
  )
}
