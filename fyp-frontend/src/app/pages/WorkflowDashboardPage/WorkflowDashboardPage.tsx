import Api from '@/api';
import { Badge } from '@/components/ui/badge'
import { Card, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { Expand, Fullscreen, Star, TrendingUpIcon } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import DataCard from './DataCard';
import OnboardingAnalyticsDTO from '@/models/OnboardingAnalyticsDTO';
import OnboardingEmployeesTableCard from './OnboardingEmployeesTableCard';
import EmployeesOnboardedChart from './EmployeesOnboardedChart';
import { Button } from '@/components/ui/button';
import { ReportedIssuesAnalyticsDTO } from '@/models/DTOs/ReportedIssuesAnalyticsDTO';

export default function WorkflowDashboardPage() {
  const [onboardingAnalytics, setOnboardingAnalytics] = useState<OnboardingAnalyticsDTO | null>(null);
  const [issuesAnalytics, setIssuesAnalytics] = useState<ReportedIssuesAnalyticsDTO | null>(null);

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

  async function fetchIssuesAnalytics() {
    await Api.analytics.fetchReportedIssuesAnalytics()
    .then((response: AxiosResponse<HTTPresponse<ReportedIssuesAnalyticsDTO, string>>) => {
      setIssuesAnalytics(response.data.data);
    })
    .catch((error) => {      
    })
    .finally(() => {      
    })
  }

  const containerRef = useRef<HTMLDivElement>(null);

  const handleFullscreen = () => {
    if (containerRef.current?.requestFullscreen) {
      containerRef.current.requestFullscreen();
    } 
  };


  useEffect(() => {
    void fetchOnboardingAnalytics();
    void fetchIssuesAnalytics();
  }, [])

  return (
    <div className='flex flex-col gap-2 relative bg-background p-2' ref={containerRef}>
      <Button className='absolute top-1 right-1' onClick={handleFullscreen}><Expand/></Button>
      <div className='flex flex-col gap-2'>
        <div className='flex flex-row w-full justify-between'>
          <h1 className='text-lg flex-10 font-bold'>Onboarding Workflows</h1>
        </div>        
        <div className='*:data-[slot=card]:shadow-xs grid grid-cols-5 grid-rows-3 gap-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card'>
          <OnboardingEmployeesTableCard/>
          <DataCard label='Employees onboarding' data={onboardingAnalytics?.employeesOnboarding} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Employees onboarded' data={onboardingAnalytics?.employeesOnboarded} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Average Time to onboard' data={`${Math.round(onboardingAnalytics?.averageTimeToOnboard ?? 0)} days`} loading={loadingOnboardingAnalytics}/>
          <DataCard label='Open Task Issues' data={<span className='text-red-500'>{issuesAnalytics?.openTaskIssues}</span>}
            loading={loadingOnboardingAnalytics}
            errored={onboardingAnalyticsErrored}
          />
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
          <EmployeesOnboardedChart/>       
        </div>
      </div>            
    </div>
  )
}
