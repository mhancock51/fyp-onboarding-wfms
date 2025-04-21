import Api from '@/api';
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { Expand, Star } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import DataCard from './DataCard';
import OnboardingAnalyticsDTO from '@/models/OnboardingAnalyticsDTO';
import OnboardingEmployeesTableCard from './OnboardingEmployeesTableCard';
import { Button } from '@/components/ui/button';
import { ReportedIssuesAnalyticsDTO } from '@/models/DTOs/ReportedIssuesAnalyticsDTO';
import { TaskAnalyticsDTO } from '@/models/DTOs/TaskAnalyticsDTO';
import { toast } from 'sonner';

export default function WorkflowDashboardPage() {
  const [onboardingAnalytics, setOnboardingAnalytics] = useState<{data: OnboardingAnalyticsDTO | null, loading: boolean}>({data: null, loading: false});
  const [issuesAnalytics, setIssuesAnalytics] = useState<{data: ReportedIssuesAnalyticsDTO | null, loading: boolean}>({data: null, loading: false});
  const [taskAnalytics, setTaskAnalytics] = useState<{data: TaskAnalyticsDTO | null, loading: boolean}>({data: null, loading: false});


  async function fetchOnboardingAnalytics() {
    // set loading to true
    setOnboardingAnalytics(prevState => ({...prevState, loading: true}))
    await Api.analytics.fetchOnboardingAnalytics()
    .then((response: AxiosResponse<HTTPresponse<OnboardingAnalyticsDTO, string>>) => {
      // set onboarding data in hook
      setOnboardingAnalytics(prevState => ({...prevState, data: response.data.data}));
    })
    .catch((error) => {
      toast.error("Failed to fetch onboarding analytics");
    })
    .finally(() => {
      // set loading to false
      setOnboardingAnalytics(prevState => ({...prevState, loading: false}));
    })
  }

  async function fetchIssuesAnalytics() {
    setIssuesAnalytics(prevState => ({...prevState, loading: true}));
    await Api.analytics.fetchReportedIssuesAnalytics()
    .then((response: AxiosResponse<HTTPresponse<ReportedIssuesAnalyticsDTO, string>>) => {
      setIssuesAnalytics(prevState => ({...prevState, data: response.data.data}));
    })
    .catch((error) => {      
      toast.error("Failed to fetch issues analytics");
    })
    .finally(() => {  
      setIssuesAnalytics(prevState => ({...prevState, loading: false}));    
    })
  }

  async function fetchTasksAnalytics() {
    setTaskAnalytics(prevState => ({...prevState, loading: true}));
    await Api.analytics.fetchTaskAnalytics()
    .then((response: AxiosResponse<HTTPresponse<TaskAnalyticsDTO, string>>) => {
      setTaskAnalytics(prevState => ({...prevState, data: response.data.data}));
    })
    .catch((error) => {      
    })
    .finally(() => {
      setTaskAnalytics(prevState => ({...prevState, loading: false}));      
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
    void fetchTasksAnalytics();
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
          <DataCard label='Employees onboarding' 
            data={onboardingAnalytics.data?.employeesOnboarding} 
            loading={onboardingAnalytics.loading}
          />
          <DataCard label='Employees onboarded' 
            data={onboardingAnalytics.data?.employeesOnboarded} 
            loading={onboardingAnalytics.loading}
          />
          <DataCard label='Average Time to onboard' 
            data={`${Math.round(onboardingAnalytics.data?.averageTimeToOnboard ?? 0)} days`} 
            loading={onboardingAnalytics.loading}
          />
          <DataCard label='Open Task Issues' 
            data={<span className='text-red-500'>{issuesAnalytics.data?.openTaskIssues}</span>} 
            loading={issuesAnalytics.loading}
          />
          <DataCard label='Over Due Incomplete Tasks' 
            data={<span className='text-red-500'>{taskAnalytics.data?.incompleteOverdueTasks}</span>}
            loading={taskAnalytics.loading}
          />
          <DataCard label='Tasks completed overdue' 
            data={<span className='text-red-500'>{taskAnalytics.data?.tasksCompletedOverdue}</span>}
            loading={taskAnalytics?.loading}
          />
        </div>
      </div>            
    </div>
  )
}
