export interface OnboardedEmployeesTimelineItem {
  monthIndex: number;
  month: string;
  onboarders: number;
}

export interface OnboardedEmployeesTimelineDTO {
  timeline: OnboardedEmployeesTimelineItem[];
}