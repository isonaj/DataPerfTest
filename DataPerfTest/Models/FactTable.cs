using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public enum Compliance {  Compliant, NonCompliant, ExpiringSoon }

    public class FactTable
    {
        public int PersonId { get; set; }
        public int LearningItemId { get; set; }

        public DateTime? CompletedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Compliance Compliance { get; set; }
    }
}
