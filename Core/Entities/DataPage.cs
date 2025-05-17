using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DataPage
    {
        [Key] public int Id { get; set; }

        [Required] public string NamePage { get; set; }

        [Required] public string NameData { get; set; }

        public string InfRu {  get; set; } = string.Empty;

        public string InfEng { get; set; } = string.Empty;
    }
}
