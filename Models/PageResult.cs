using System;
using System.Security.Principal;
using OfficeOpenXml.Export.ToDataTable;

namespace BaiTap_phan3.Models{

    public class PageResult<T>{
        
        public int TotalPage {get; set;} = 0;
        public IEnumerable<T>? Data {get; set;} = null;
        public Pagination Pagination {get; set;}
    }
}