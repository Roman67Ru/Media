using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media
{
    public class VideoCards
    {
        [Key]
        public int idVideo { get; set; }
        public string name { get; set; }
        public string timing { get; set; }
        public string path { get; set; }
        public string format { get; set; }
        public string size { get; set; }
        [ForeignKey("idProgram")]
        public virtual TVProgram TVProgram { get; set; }
        public int idProgram { get; set; }
    }
}
