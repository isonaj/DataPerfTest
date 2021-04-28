using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public class LearningHistory
    {
        public int PersonId { get; set; }
        public int LearningItemId { get; set;  }

        public DateTime CompletedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
