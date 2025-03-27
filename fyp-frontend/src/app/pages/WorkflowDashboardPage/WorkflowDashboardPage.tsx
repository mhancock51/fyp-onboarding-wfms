import Api from '@/api';
import { Badge } from '@/components/ui/badge'
import { Card, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { Star, TrendingUpIcon } from 'lucide-react'
import { useEffect, useState } from 'react'
import DataCard from './DataCard';
import OnboardingAnalyticsDTO from '@/models/OnboardingAnalyticsDTO';

export default function WorkflowDashboardPage() {
  const [onboardingAnalytics, setOnboardingAnalytics] = useState<OnboardingAnalyticsDTO | null>(null);
  const [loadingOnboardingAnalytics, setLoadingOnboardingAnalytics] = useState<boolean>(false);
  const [onboardingAnalyticsErrored, setOnboardingAnalyticsErrored] = useState<boolean>(false);


  async function fetchOnboardingAnalytics() {
    setLoadingOnboardingAnalytics(true);
    await Api.analytics.fetchOnboardingAnalytics()
    .then((response: AxiosResponse<HTTPresponse<OnboardingAnalyticsDTO, string>>) => {
      setOnboardingAnalytics(response.data.data);
    })
    .catch((error) => {
      setOnboardingAnalyticsErrored(true);
    })
    .finally(() => {
      setLoadingOnboardingAnalytics(false);
    })
  }

  useEffect(() => {
    void fetchOnboardingAnalytics();
  }, [])

  return (
    <div className='flex flex-col gap-2'>
      <div className='flex flex-col gap-2'>
        <div className='flex flex-row w-full justify-between'>
          <h1 className='text-lg flex-10 font-bold'>Onboarding Workflows</h1>
        </div>        
        <div className='*:data-[slot=card]:shadow-xs grid grid-cols-5 grid-rows-3 gap-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card'>
          <DataCard label='Time to onboard' data={`${onboardingAnalytics?.averageTimeToOnboard} days`} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Employees onboarding' data={onboardingAnalytics?.employeesOnboarding} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Employees onboarded' data={onboardingAnalytics?.employeesOnboarded} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Onboarding Satisifaction rating' data={
            <div className='flex flex-col'>
              {"[PLACEHOLDER]"}
              <div className='flex flex-row gap-1'>
                <Star className='text-yellow-500'/><Star className='text-yellow-500'/><Star className='text-yellow-500'/><Star className='text-gray-300'/><Star className='text-gray-300'/>
              </div>
            </div>
            } 
            loading={loadingOnboardingAnalytics} fontSize='text-[1.25em]'
            errored={onboardingAnalyticsErrored}
          />
          <DataCard label='Most popular workflow' data={"[PLACEHOLDER] Junior Onboarding Workflow"} fontSize='text-[1.25em]'
            loading={loadingOnboardingAnalytics}
            errored={onboardingAnalyticsErrored}
          />       
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
