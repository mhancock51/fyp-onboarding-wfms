import { Badge } from '@/components/ui/badge'
import { Card, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Spinner } from '@/components/ui/spinner';
import { TrendingDownIcon, TrendingUpIcon, TriangleAlert } from 'lucide-react'
import { JSX } from 'react';

interface Props {
  label: string;
  data: string | number | JSX.Element | undefined;
  loading: boolean;
  trend?: number;
  trendUp?: boolean;
  fontSize?: string;
  fontBold?: string;
  colSpan?: string;
  rowSpan?: string;
  errored?: boolean;
}

export default function DataCard(props: Props) {
  return (
    <Card className={`@container/card min-h-[150px] ${props.colSpan ? props.colSpan : ""} ${props.rowSpan ? props.rowSpan : ""}`}>
      <CardHeader className="relative">
        <CardDescription>{props.label}</CardDescription>
        <CardTitle className={`${props.fontSize ? props.fontSize : "text-3xl"} ${props.fontBold ? props.fontBold : "font-semibold"} tabular-nums`}>
          {
            !props.loading && props.errored && 
            <div className='w-full flex flex-row justify-start gap-2 items-center text-red-700 text-xl'>
              <TriangleAlert/>
              Failed to load
            </div>
          }
          {
            props.loading &&
            <div className='w-full flex flex-row justify-start'>
              <Spinner/>
            </div>
          }
          {
            !props.loading && !props.errored && 
            props.data
          }          
        </CardTitle>
        {
          props.trend !== undefined && props.trendUp !== undefined &&
          <div className="absolute right-4 top-0">
            <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
              {
                props.trendUp ? (
                  <TrendingUpIcon className="size-3" />
                ) : (
                  <TrendingDownIcon className='size-3'/>
                )
              }
              {
                props.trendUp ? (
                  "+"
                ) : (
                  "-"
                )
              }
              {
                `${props.trend}%`
              }
            </Badge>
          </div>
        }
      </CardHeader>
    </Card>
  )
}
