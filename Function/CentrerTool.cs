using Newtonsoft.Json;
using BaiTap_phan3.Models;
using System.Text;

namespace BaiTap_phan3.Function
{
    public class CenterTool
    {
        #region Fields
        public static List<NhanVien> NhanViens = new List<NhanVien>();
        public static IHttpContextAccessor? ContextAccessor;
        private static bool _isLoadedData = false;

        #endregion

        public static void LoadNhanVienToSession(IHttpContextAccessor contextAccessor)
        {
            if (_isLoadedData)
                return;

            ContextAccessor = contextAccessor;
            NhanVien nhanVien;
            string nhanVienJson;

            for (int i = 1; i < 7; i++)
            {
                nhanVien = new NhanVien()
                {
                    HoVaTen = "Nguyễn văn " + i,
                    SoDienThoai = "098765" + i,
                    DiaChi = $"Ngõ {i} Thanh Xuân, Hà nội",
                    NgayThangNamSinh = new DateTime(1999, 10, 1).AddDays(i),
                    ChucVu = "Nhân Viên",
                    MaNhanVien = GenerateMaNv()
                };

                nhanVienJson = JsonConvert.SerializeObject(nhanVien);
                ContextAccessor.HttpContext?.Session.SetString(nhanVien.MaNhanVien, nhanVienJson);
            }
            _isLoadedData = true;

        }

        public static Dictionary<string, NhanVien> GetDanhSachNhanVien()
        {

            Dictionary<string, NhanVien> nhanVienDictionary = new Dictionary<string, NhanVien>();
            string? nhanVienString;

            foreach (string key in ContextAccessor.HttpContext.Session.Keys)
            {
                if (key.Contains("NV-"))
                {

                    nhanVienString = ContextAccessor.HttpContext.Session.GetString(key);
                    if (!string.IsNullOrEmpty(nhanVienString))
                    {

                        nhanVienDictionary.Add(key, JsonConvert.DeserializeObject<NhanVien>(nhanVienString));
                    }
                }

            }
            return nhanVienDictionary;

        }

        public static string  GenerateMaNv()
        {
            //MaNhanVien = $"NV-{string.Concat(Enumerable.Repeat('0', 4 - (++_count).ToString().Count()))}{_count}";
            Dictionary<string, NhanVien>? nhanVienDic = GetDanhSachNhanVien();
            if (nhanVienDic.Count == 0)
            {
                return "NV-0001";
            }
            else
            {
                string maxMaNhanVien = nhanVienDic.MaxBy(c => c.Value.MaNhanVien).Value.MaNhanVien;
                string maNhanVien = createMaNv(int.Parse(maxMaNhanVien.Substring(3)));
                return maNhanVien;
            }
        }

        public static string createMaNv(int count)
        {
            count++;
            StringBuilder stringBuilder = new StringBuilder("NV-");
            for (int i = 0; i < 4 - count.ToString().Count(); i++)
            {
                stringBuilder.Append("0");
            }
            stringBuilder.Append(count.ToString());
            string maNhanVien = stringBuilder.ToString();

            return maNhanVien;
        }
    }
}