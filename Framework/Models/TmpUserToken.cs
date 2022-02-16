using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Models
{
    public class TmpUserToken
    {
        public string token { get; set; }
        public DateTime exp { get; set; }
        public string username { get; set; }
        public string user_code { get; set; }
        public string id { get; set; }
        public List<string> role { get; set; }
        public List<string> actions { get; set; }
        public string usertype { get; set; }
        public  List<string> company_code { get; set; }
        public  List<string> plant_code { get; set; }
        public  List<string> mcc_code { get; set; }
        public  List<string> bmc_code { get; set; }
        public  List<string> mpp_code { get; set; }
        public bool isAdmin { get; set; } = false;
        public string tag { get; set; }
        public string current_financial_year { get; set; }
    }

    public static class UserData
    {
        public static string token { get; set; }
        public static string user_code { get; set; }
        public static string usertype { get; set; }
        public static List<string> company_code { get; set; }
        public static List<string> plant_code { get; set; }
        public static List<string> mcc_code { get; set; }
        public static List<string> bmc_code { get; set; }
        public static List<string> mpp_code { get; set; }
        public static bool isAdmin { get; set; }
        public static string tag { get; set; }
        public static string current_financial_year { get; set; }
    }
    public class FreeAction
    {
        public List<string> actions { get; set; }
        public DateTime created_at { get; set; }

    }

}
