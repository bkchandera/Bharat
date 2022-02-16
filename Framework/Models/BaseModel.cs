using Dapper.Contrib.Extensions;
using System;
using Framework.Library.Attribute;

namespace Framework.Models
{
   
    public class BaseModel : IBaseModel
    {
        public DateTime? created_at { get; set; } 
        public string created_by { get; set; } 
        public DateTime? updated_at { get; set; }        
        public string updated_by { get; set; }
        public string flg_sentbox_entry { get; set; } = "N";
        public int originating_type { get; set; } = 0;
        public string originating_org_code { get; set; }
        [HistoryColumn]
        public string history_created_by { get; set; }
        [HistoryColumn]
        public DateTime history_created_at { get; set; } 
        [HistoryColumn]
        //delete_special for delete entry in sentbox 
        //insert_special for insert entry in sentbox
        public string operation_type { get; set; } = "INSERT";
        //insert,update,delete
        [Computed]
        public string model_operation { get; set; } = "insert";
        [Computed]
        //value will be ,parent or master column name
        public string parent_child { get; set; } = "none";
        [Computed]
        public string custom_edit { get; set; } = "0";
        [Computed]
        public string master_module_type { get; set; } = "";
        [Computed]
        public bool is_sentbox { get; set; } = true;
        

    }
}
