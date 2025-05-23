using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class School
    {
        [Key]
        public int IDSchool { get; set; }
        public string NameOrganiz { get; set; }
        public string Adres { get; set; }

        // Навигационное свойство
        public virtual ICollection<Abiturient> Abiturients { get; set; }
    }
}
