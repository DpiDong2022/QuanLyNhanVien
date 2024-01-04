using System;
using System.Security.Principal;
using OfficeOpenXml.Export.ToDataTable;

namespace BaiTap_phan3.Models{

    public class PageResult<T>{

        public int TotalPage {get; set;}
        public IEnumerable<T>? Data {get; set;}
        public PageResult()
        {
            Data = null;
            TotalPage = 0;
        }

        public PageResult(IEnumerable<T> data, int pageSize, int pageNumber)
        {   
            TotalPage = (int)Math.Ceiling((decimal)data.Count()/pageSize);
            TotalPage = TotalPage<1 ? 1:TotalPage;
            Data = data.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }        

    }
}