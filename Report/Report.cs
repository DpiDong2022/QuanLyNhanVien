using System;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace BaiTap_phan3.Report
{
    public enum templates
    {
        NhanVien
    }

    public class Report<T>
    {
        public void ExportToExccel(List<T> danhSach)
        {
            //string filePath = Server.
            //Opening an existing Excel file
            FileInfo fi = new FileInfo(@"Path\To\Your\File.xlsx");
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];

                //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets["SomeWorksheet"];

                //If you don't know if a worksheet exists, you could use LINQ,
                //So it doesn't throw an exception, but return null in case it doesn't find it
                ExcelWorksheet anotherWorksheet =
                    excelPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "SomeWorksheet");

                //Get the content from cells A1 and B1 as string, in two different notations
                string valA1 = firstWorksheet.Cells["A1"].Value.ToString();
                string valB1 = firstWorksheet.Cells[1, 2].Value.ToString();

                //Save your file
                excelPackage.Save();
            }

        }
    }
}