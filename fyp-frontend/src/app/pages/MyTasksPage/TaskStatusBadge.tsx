import { Badge } from '@/components/ui/badge'
import clsx from 'clsx';

export default function TaskStatusBadge(props: {status: string, className?: string;}) {
  function statusToColor(status: string) {
    switch(status.toLowerCase()) {
      case "open":
        return "bg-green-500";
      case "complete":
        return "bg-primary";
      default:
        return ""
    }
  }

  return (
    <Badge className={clsx(`rounded-full text-white ${statusToColor(props.status)} text-center p-2 min-w-[90px]`, props.className)}>
      {props.status.toUpperCase()}
    </Badge>
  )
}
