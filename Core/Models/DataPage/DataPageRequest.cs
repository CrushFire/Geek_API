using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.DataPage
{
    public class DataPageRequest
    {
        public string InfRu {  get; set; } = string.Empty;
        public string InfEng { get; set; } = string.Empty;

    }
}
