using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.DataPage
{
    public class DataPageUpdateRequest
    {
        public int Id {  get; set; }
        public string Data { get; set; } = string.Empty;
        public string EngData { get; set; } = string.Empty;
    }
}
