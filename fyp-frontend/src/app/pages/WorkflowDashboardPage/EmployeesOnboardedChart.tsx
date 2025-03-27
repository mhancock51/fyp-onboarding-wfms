import React, { useEffect, useState } from 'react'
import DataCard from './DataCard'
import { ChartConfig, ChartContainer, ChartTooltip, ChartTooltipContent } from '@/components/ui/chart'
import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from 'recharts'
import { OnboardedEmployeesTimelineDTO } from '@/models/DTOs/OnboardedEmployeesTimelineDTO';
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';

export default function EmployeesOnboardedChart() {
  const [data, setData] = useState<OnboardedEmployeesTimelineDTO | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [errored, setErrored] = useState<boolean>(false);

  const chartData = [
    { month: "January",   onboarders: 4,    lastYearsOnboarders: 3   },
    { month: "February",  onboarders: 2,    lastYearsOnboarders: 2   },
    { month: "March",     onboarders: 2,    lastYearsOnboarders: 8 },
    { month: "April",     onboarders: 4,    lastYearsOnboarders: 7 },
    { month: "May",       onboarders: 2,    lastYearsOnboarders: 3 },
    { month: "June",      onboarders: 2,    lastYearsOnboarders: 4 },
    { month: "July",      onboarders: 4,    lastYearsOnboarders: 4 },
    { month: "August",    onboarders: 6,    lastYearsOnboarders: 4 },
    { month: "September", onboarders: 7,    lastYearsOnboarders: 4 },
    { month: "October",   onboarders: 1,    lastYearsOnboarders: 4 },
    { month: "November",  onboarders: 3,    lastYearsOnboarders: 4 },
    { month: "December",  onboarders: 2,    lastYearsOnboarders: 4 },
  ]

  async function fetchData() {
    setLoading(true);
    Api.analytics.fetchOnboardedEmployeesTimeline()
    .then((response: AxiosResponse<HTTPresponse<OnboardedEmployeesTimelineDTO, string>>) => {
      setData(response.data.data);
    })
    .catch(() => {
      setErrored(true);
    })
    .finally(() => {
      setLoading(false);
    })
  }

  const chartConfig = {
    onboarders: {
      label: "Onboarders",
      color: "#2563eb",
    },
    lastYearsOnboarders: {
      label: "Last Year's Onboarders",
      color: "#60a5fa",
    },
  } satisfies ChartConfig

  useEffect(() => {
    void fetchData();
  }, []);

  return (
    <DataCard label={'Team members onboarded this year'} colSpan='md:col-span-2 sm:col-span-2' rowSpan='row-span-2'
      loading={loading} errored={errored}
      data={
        <ChartContainer config={chartConfig} className="w-full">
          <BarChart accessibilityLayer data={data}>
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey="month"
              tickLine={false}
              tickMargin={2}
              axisLine={false}
              tickFormatter={(value) => value.slice(0, 3)}
            />
            <YAxis
              dataKey={"onboarders"}
              tickLine={false}
              tickMargin={2}
              axisLine={false}            
            />
            <ChartTooltip content={<ChartTooltipContent />} />
            <Bar dataKey="onboarders" fill="var(--color-onboarders)" radius={4} />
            {/* <Bar dataKey="lastYearsOnboarders" fill="var(--color-lastYearsOnboarders)" radius={4} /> */}
          </BarChart>
        </ChartContainer>
    } />
  )
}
