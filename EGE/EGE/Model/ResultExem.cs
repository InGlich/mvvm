using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class ResultExem
    {
        [Key]
        public int IDResult { get; set; }
        public decimal BallEGE { get; set; }
        public string Predmet { get; set; }

        [ForeignKey("Abiturient")] 
        [Column("IDAbitur")] 
        public int IDAbitur { get; set; }

        public virtual Abiturient Abiturient { get; set; }
    }
}
