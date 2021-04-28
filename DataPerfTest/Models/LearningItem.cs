using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public class LearningItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<LearningPlan> LearningPlans { get; set; }
        public IEnumerable<FactTable> Facts { get; set; }
        public IEnumerable<LearningHistory> LearningHistories { get; set; }


    }
}
