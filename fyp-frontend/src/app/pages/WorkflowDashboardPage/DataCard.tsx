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

const DataCard = ({label, data, loading, trend, trendUp, fontSize, fontBold, colSpan, rowSpan, errored }: Props) => {
  return (
    <Card className={`@container/card min-h-[150px] ${colSpan ? colSpan : ""} ${rowSpan ? rowSpan : ""}`}>
      <CardHeader className="relative">
        <CardDescription>{label}</CardDescription>
        <CardTitle className={`${fontSize ? fontSize : "text-3xl"} ${fontBold ? fontBold : "font-semibold"} tabular-nums`}>
          {
            !loading && errored && 
            <div className='w-full flex flex-row justify-start gap-2 items-center text-red-700 text-xl'>
              <TriangleAlert/>
              Failed to load
            </div>
          }
          {
            loading &&
            <div className='w-full flex flex-row justify-start'>
              <Spinner/>
            </div>
          }
          {
            !loading && !errored && 
            data
          }          
        </CardTitle>
        {
          trend !== undefined && trendUp !== undefined &&
          <div className="absolute right-4 top-0">
            <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
              {
                trendUp ? (
                  <TrendingUpIcon className="size-3" />
                ) : (
                  <TrendingDownIcon className='size-3'/>
                )
              }
              {
                trendUp ? (
                  "+"
                ) : (
                  "-"
                )
              }
              {
                `${trend}%`
              }
            </Badge>
          </div>
        }
      </CardHeader>
    </Card>
  )
}

export default DataCard
