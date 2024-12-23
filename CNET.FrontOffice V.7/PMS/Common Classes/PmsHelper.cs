using System;
using System.Collections.Generic;
using CNET_V7_Domain.Domain.PmsSchema;
using DevExpress.XtraScheduler;

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class PmsHelper
    {
        /// <summary>
        /// This method will generate outputs a CNET's list of WeekDays for the given weekDays, component and reference
        /// </summary>
        public static void GenerateWeekDays(WeekDays weekDays, int component, int reference, out List<WeekDayDTO> cnetWeekDays)
        {
            cnetWeekDays = new List<WeekDayDTO>();
            // 1
            if (weekDays.HasFlag(WeekDays.Monday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Id = 0,
                    Day = 2,
                    Pointer = component,
                    Reference = reference
                });
            }
            // 2
            if (weekDays.HasFlag(WeekDays.Tuesday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Id = 0,
                    Day = 4,
                    Pointer = component,
                    Reference = reference
                });
            }
            // 3
            if (weekDays.HasFlag(WeekDays.Wednesday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Day = 8,
                    Pointer = component,
                    Reference = reference
                });
            }
            // 4
            if (weekDays.HasFlag(WeekDays.Thursday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Day = 16,
                    Pointer = component,
                    Reference = reference
                });
            }
            // 5
            if (weekDays.HasFlag(WeekDays.Friday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Day = 32,
                    Pointer = component,
                    Reference = reference
                });
            }// 6
            if (weekDays.HasFlag(WeekDays.Saturday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Day = 64,
                    Pointer = component,
                    Reference = reference
                });
            }
            // 7
            if (weekDays.HasFlag(WeekDays.Sunday))
            {
                cnetWeekDays.Add(new WeekDayDTO()
                {
                    Day = 1,
                    Pointer = component,
                    Reference = reference
                });
            }
        }
        /// <summary>
        /// This method takes a CNET's list of WeekDays as an argument, and returns a enum value of typeof WeekDays
        /// </summary>
        public static WeekDays PaintWeekDaysCheckList(List<WeekDayDTO> cnetWeekDays)
        {
            WeekDays weekDays = new WeekDays();

            foreach (WeekDayDTO day in cnetWeekDays)
            {
                if (day.Day == 2)
                {
                    weekDays |= WeekDays.Monday;
                }
                else if (day.Day == 4)
                {
                    weekDays |= WeekDays.Tuesday;
                }
                else if (day.Day == 8)
                {
                    weekDays |= WeekDays.Wednesday;
                }
                else if (day.Day == 16)
                {
                    weekDays |= WeekDays.Thursday;
                }
                else if (day.Day == 32)
                {
                    weekDays |= WeekDays.Friday;
                }
                else if (day.Day == 64)
                {
                    weekDays |= WeekDays.Saturday;
                }
                else if (day.Day == 1)
                {
                    weekDays |= WeekDays.Sunday;
                }
            }

            return weekDays;
        }
        public static List<string> GetWeekDaysName(List<string> cnetWeekDays)
        {
            List<string> weekDays = new List<string>();

            foreach (string day in cnetWeekDays)
            {
                if (day == "2")
                {
                    weekDays.Add("Monday");
                }
                else if (day == "4")
                {
                    weekDays.Add("Tuesday");
                }
                else if (day == "8")
                {
                    weekDays.Add("Wednesday");
                }
                else if (day == "16")
                {
                    weekDays.Add("Thursday");
                }
                else if (day == "32")
                {
                    weekDays.Add("Friday");
                }
                else if (day == "64")
                {
                    weekDays.Add("Saturday");
                }
                else if (day == "1")
                {
                    weekDays.Add("Sunday");
                }
            }

            return weekDays;
        }
    }
}
