using Core.Models.DataPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results
{
    public class MyViews
    {
        public DataPageNameRequest Data {  get; set; }
        public static MyViews TakeData (string page, string name)
        {
            DataPageNameRequest request = new DataPageNameRequest();
            request.Page = page;
            request.Name = name;
            return new MyViews { Data = request };
        }
    }
}
