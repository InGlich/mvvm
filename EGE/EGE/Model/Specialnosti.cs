using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class Specialnosti
    {
        [Key]
        public int IDSpecial { get; set; }
        [ForeignKey("Otdelenie")] // Явное указание связи
        public int IDOtdel { get; set; }
        public string Shifr { get; set; }
        public string NameSpecial { get; set; }
        public int CountMest { get; set; }
        public string FormaObychen { get; set; }
        public bool Bydzhet { get; set; }

        // Навигационные свойства
        public virtual Otdelenie Otdelenie { get; set; }
        public virtual ICollection<Abiturient> Abiturients { get; set; }
    }
}
