
using System.Security.Cryptography;

namespace BaiTap_phan3.Models
{

    public class ResponseError: ResponseBase
    {
        public string? RequestId { get; set; }
        public string Message { get; set; }
        public string ControllerName{get; set;}
        public string ActionName{get; set;}
        public DateTime Time{get; set;}

        public override string ToString()
        {
            return string.Format("\nRequest Id :{0} \nTime: {1}\nController name: {2}\nAction name: {3}\nMessage: {4}", 
                                    RequestId, Time.ToString("hh:mm:ss"), ControllerName, ActionName, Message);
        }
    }
    
    public class ResponseBase 
    {
        public string Status { get; set; } = "Ok";
        public int Code { get; set; } = 200;
        public object Data { get; set; }=null;
    }
}