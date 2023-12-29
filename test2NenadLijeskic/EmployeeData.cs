using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test2NenadLijeskic
{
    public class EmployeeData
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StarTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string EntryNotes { get; set; }
        public DateTime? DeletedOn { get; set; }
        public double TotalWorkedHours { get { return (EndTimeUtc - StarTimeUtc).TotalHours; } set { } }

    }
}
