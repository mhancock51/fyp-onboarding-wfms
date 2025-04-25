using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class OnboardedEmployeesTimelineItem
    {
        public OnboardedEmployeesTimelineItem(int monthIndex, string month, int onboarders)
        {
            MonthIndex = monthIndex;
            Month = month;
            Onboarders = onboarders;
        }

        public int MonthIndex { get; set; }
        public string Month { get; set; }
        public int Onboarders {  get; set; }
    }

    public class OnboardedEmployeesTimelineDTO
    {


        public List<OnboardedEmployeesTimelineItem> Timeline { get; set; }

        public OnboardedEmployeesTimelineDTO(List<OnboardedEmployeesTimelineItem> timeline)
        {
            Timeline = timeline;
        }
    }
}
