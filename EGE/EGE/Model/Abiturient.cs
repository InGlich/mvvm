using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class Abiturient
    {
        [Key]
        public int IDAbitur { get; set; }
        public string LastName { get; set; }
        [ForeignKey("Shool")]
        public int IDSchool { get; set; }
        public string Adres { get; set; }
        public DateTime DateBirth { get; set; }
        public string Phone { get; set; }
        public decimal CredBal { get; set; }
        [ForeignKey("Specialnosti")] // Явное указание связи
        public int IDSpecial { get; set; }

        // Навигационные свойства для связей
        public virtual School Shool { get; set; }
        public virtual Specialnosti Specialnosti { get; set; }
        public virtual ICollection<ResultExem> ResultExems { get; set; }

    }
}
