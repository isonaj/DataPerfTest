using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public class LearningPlan
    {
        public int PersonId { get; set; }
        public int LearningItemId { get; set; }

        public DateTime RequiredDate { get; set; }
    }
}
