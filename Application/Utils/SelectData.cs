using Core.Interfaces.Services;
using Core.Models.DataPage;
using Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class SelectData
    {
        public ServiceResult<Dictionary<string, DataPageRequest>> Page {  get; set; }
        public string Language { get; set; }

        public SelectData(ServiceResult<Dictionary<string, DataPageRequest>> _page, string _language)
        {
            Page = _page;
            Language = _language;
        }

        public string From(string DataName)
        {
            string myData;
            if (Language == "ru")
            {
                myData = Page.Data != null ? Page.Data[DataName].InfRu : "Таких данных в системе не найдено";
            }
            else
            {
                myData = Page.Data != null ? Page.Data[DataName].InfEng : "Data not exists";
            }

            return myData;
        }
    }
}
