import Api from '@/api';
import { Badge } from '@/components/ui/badge'
import { Card, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { TrendingUpIcon } from 'lucide-react'
import { useEffect, useState } from 'react'

export default function WorkflowDashboardPage() {
  const [completedInstances, setCompleteInstances] = useState<number>(0);
  const [openInstances, setOpenInstances] = useState<number>(0);
  const [avgOnboard, setAvgOnboard] = useState<number>(0);

  async function fetchCompleteInstances() {
    await Api.analytics.fetchNumberOfCompletedInstances()
    .then((response: AxiosResponse<HTTPresponse<number, string>>) => {
      setCompleteInstances(response.data.data);
    })    
  }

  async function fetchOpenInstances() {
    await Api.analytics.fetchNumberOfOpenInstnces()
    .then((response: AxiosResponse<HTTPresponse<number, string>>) => {
      setOpenInstances(response.data.data);
    })   
  }

  async function fetchAverageTimeToOnboard() {
    await Api.analytics.fetchAvgTimeToOnboard()
    .then((response: AxiosResponse<HTTPresponse<number, string>>) => {
      setAvgOnboard(response.data.data);
    })  
  }

  useEffect(() => {
    void fetchCompleteInstances();
    void fetchOpenInstances();
    void fetchAverageTimeToOnboard();
  }, [])

  return (
    <div className='flex flex-col gap-2'>
      <div className='flex flex-col gap-2'>
        <div className='flex flex-row w-full justify-between'>
          <h1 className='text-lg flex-10 font-bold'>Onboarding Workflows</h1>
        </div>        
        <div className='*:data-[slot=card]:shadow-xs grid grid-cols-3 gap-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card'>
          <Card className="@container/card col-3">
            <CardHeader className="relative">
              <CardDescription>Time to onboard</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums">
                {avgOnboard} days
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
          <Card className="@container/card col-3">
            <CardHeader className="relative">
              <CardDescription>Employees Onboarding</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums">
                { openInstances }
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
          <Card className="@container/card col-3">
            <CardHeader className="relative">
              <CardDescription>Employees Onboarded</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums">
                {completedInstances}
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
        </div>
      </div>
      <div className='flex flex-col gap-2'>
        <h1 className='text-lg font-bold'>Tasks</h1>        
        <div className='*:data-[slot=card]:shadow-xs grid grid-cols-4 gap-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card'>
          <Card className="@container/card">
            <CardHeader className="relative">
              <CardDescription>Tasks Completed</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums">
                15
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
          <Card className="@container/card">
            <CardHeader className="relative">
              <CardDescription>Tasks Created</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums">
                4
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
          <Card className="@container/card">
            <CardHeader className="relative">
              <CardDescription>Overdue Tasks</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums text-destructive">
                3
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
          <Card className="@container/card">
            <CardHeader className="relative">
              <CardDescription>Open Task Issues</CardDescription>
              <CardTitle className="@[250px]/card:text-3xl text-2xl font-semibold tabular-nums text-destructive">
                4
              </CardTitle>
              <div className="absolute right-4 top-0">
                <Badge variant="outline" className="flex gap-1 rounded-lg text-xs">
                  <TrendingUpIcon className="size-3" />
                  +12.5%
                </Badge>
              </div>
            </CardHeader>
          </Card>
        </div>
      </div>
    </div>
  )
}
