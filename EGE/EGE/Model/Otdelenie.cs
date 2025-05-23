using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class Otdelenie
    {
        [Key]
        public int IDOtdel { get; set; }

        public string NameOtdel { get; set; }

        public virtual ICollection<Specialnosti> Specialnostis { get; set; }
    }
}
