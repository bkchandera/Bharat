using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.CustomDataType
{
    public class BusinessHirarchyData
    {
        public string plant_code { get; set; }
        public string plant_name { get; set; }
        public string mcc_code { get; set; }
        public string mcc_name { get; set; }
        public string bmc_code { get; set; }
        public string bmc_name { get; set; }
        public string mpp_code { get; set; }
        public string mpp_name { get; set; }
    }
    public class GeoHirarchyData
    {
        public string state_code { get; set; }
        public string state_name { get; set; }
        public string district_code { get; set; }
        public string district_name { get; set; }
        public string tehsil_code { get; set; }
        public string tehshil_name { get; set; }
        public string village_code { get; set; }
        public string village_name { get; set; }
        public string hamlet_code { get; set; }
        public string hamlet_name { get; set; }
    }
    public class FileObject
    {
       public IFormFile FormFile { get; set; }
        public string Name { get; set; }
    }
}
