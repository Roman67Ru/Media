using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Media
{
    public class TVProgram 
    {
        [Key]
        public int idProgram { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string actors { get; set; }
        public string year { get; set; }

    }
}
