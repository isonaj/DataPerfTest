using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? ManagerId { get; set; }

        public Person Manager { get; set; }
        public IList<LearningPlan> LearningPlans { get; set; }
        public IList<LearningHistory> LearningHistories { get; set; }
        public IList<FactTable> Facts { get; set; }
    }
}
