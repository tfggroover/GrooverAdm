using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Timetable
    {
        public List<Schedule> Schedules { get; set; }
        public DayOfWeek Day { get; set; }
    }
}
