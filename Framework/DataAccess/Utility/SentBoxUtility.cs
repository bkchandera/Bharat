using System;
using System.Collections.Generic;
using Framework.DataAccess.Utility.Models;
using Framework.DataAccess.Dapper;
using Framework.Models;
using Framework.Library.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using Framework.CustomDataType;

namespace Framework.DataAccess.Utility
{
    public class SentBoxUtility
    {
        private Sentbox _SentboxModel;
        DBRepository Repo = new DBRepository();
       
        public SentBoxUtility()
        {

        }
        public void SaveSentBox(BaseModel SaveModel)
        {
            if (SaveModel.is_sentbox)
            {
                QueryParam Query = new QueryParam
                {
                    Fields = "destinations,flag_entry,table_name,module_type,to_child,type,priority",
                    Where = new List<ConditionParameter>()
                {
                    new ConditionParameter{ PropertyName="table_name",PropertyValue=SaveModel.GetType().GetTableName()},
                    new ConditionParameter{ PropertyName="flag_entry",PropertyValue=1},
                    new ConditionParameter{ PropertyName="type",PropertyValue=1},
                    new ConditionParameter{ PropertyName="to_child",PropertyValue=1},
                }
                };
                Addressbook _Addressbook = Repo.Find<Addressbook>(Query);
                bool flag = false;
                if (_Addressbook != null && _Addressbook.module_type != null && _Addressbook.module_type.Trim().Length > 0)
                {
                    List<string> ModuleType = _Addressbook.module_type.Split(',').ToList<string>();
                    flag = ModuleType.Contains(SaveModel.master_module_type);
                }
                else if (SaveModel.master_module_type == "")
                {
                    flag = true;
                }

                if (_Addressbook != null && flag)
                {

                    Query = new QueryParam
                    {
                        Fields = "device_id,module_code",
                        Table = "application_installation",
                        Join = new List<JoinParameter>()
                    {
                        new JoinParameter{table="application_installation_detail",condition="application_installation.application_installation_code=application_installation_detail.application_installation_code"}
                    }
                    };
                    if (_Addressbook.destinations == 0)
                    {
                        SaveSentbox(Query, SaveModel, _Addressbook.priority);
                    }
                    else if (_Addressbook.destinations == 1)
                    {

                        string bmc_code = GetBmcCode(SaveModel);
                        if (bmc_code != null && bmc_code != "")
                        {

                            Query.Where = new List<ConditionParameter>()
                        {
                            new ConditionParameter{ PropertyName="module_code",PropertyValue=bmc_code},
                            new ConditionParameter{ PropertyName="module_name",PropertyValue="bmc"},
                        };
                            SaveSentbox(Query, SaveModel, _Addressbook.priority);
                        }
                    }
                }
            }
            
            
        }

        private string GetBmcCode(BaseModel SaveModel)
        {           
            string bmc_code = "";            
            if (SaveModel.HasProperty("bmc_code"))
            {
                bmc_code = SaveModel.GetPropertyValue("bmc_code");
            }
            else if (SaveModel.HasProperty("module_name"))
            {
                string module_name = SaveModel.GetPropertyValue("module_name");
                if (module_name=="bmc")
                    bmc_code= SaveModel.GetPropertyValue("module_code");
                else
                {
                    bmc_code = GetBmcFromMpp("module_code", SaveModel);
                }
            }
            else if (SaveModel.HasProperty("mpp_code"))
            {
                bmc_code = GetBmcFromMpp("mpp_code", SaveModel);
            }
            return bmc_code;

        }
        private string GetBmcFromMpp(string PropertyName,BaseModel SaveModel)
        {
            QueryParam TmpQuery = new QueryParam
            {
                Fields = "bmc_code",
                Table = "business_hierarchy",
                Where = new List<ConditionParameter>()
                {
                      new ConditionParameter{ PropertyName="mpp_code",PropertyValue=SaveModel.GetPropertyValue(PropertyName)}
                }
            };
            return Repo.Find<string>(TmpQuery);
        }

        private void SaveSentbox(QueryParam Query,BaseModel SaveModel,int sequence)
        {
            var settings = new JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                ContractResolver = new IgnoreExtraPropertiesResolver()
            };
            IEnumerable<ApplicationInstallation> DeviceList = Repo.FindAll<ApplicationInstallation>(Query);
            foreach (ApplicationInstallation _device in DeviceList.NotEmpty())
            {
                // _SentboxModel = SentboxModel;
                _SentboxModel = new Sentbox
                {
                    uuid = Guid.NewGuid().ToString(),
                    table_names = SaveModel.GetType().GetTableName(),
                    json_text = JsonConvert.SerializeObject(SaveModel, settings),
                    source_org_id = "01",
                    dest_org_id = _device.module_code,
                    operation = SaveModel.operation_type,
                    device_id = _device.device_id,
                    sequence_no=sequence
                };
                Repo.AddWithoutSentbox(_SentboxModel);
            }
        }
    }


   
}
