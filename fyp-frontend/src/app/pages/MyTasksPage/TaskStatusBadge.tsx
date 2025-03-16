import { Badge } from '@/components/ui/badge'
import React from 'react'

export default function TaskStatusBadge(props: {status: string}) {
  function statusToColor(status: string) {
    switch(status.toLowerCase()) {
      case "open":
        return "bg-green-500";
      case "complete":
        return "bg-blue-500";
      default:
        return ""
    }
  }

  return (
    <Badge className={`mx-2 rounded-full text-white ${statusToColor(props.status)} text-center p-2`} style={{minWidth: "90px"}}>
      {props.status.toUpperCase()}
    </Badge>
  )
}
