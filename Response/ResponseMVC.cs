using System;

namespace BaiTap_phan3.Response
{

    public class ResponseMvc
    {
        public bool Success { get; set; }
        public object data { get; set; }
        public string Message { get; set; }
    }

}