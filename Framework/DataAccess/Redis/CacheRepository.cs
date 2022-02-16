using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Framework.Library.Helper;

namespace Framework.DataAccess.Redis
{
    public class CacheRepository
    {
      //  private IRedisClient _cache;
        //private readonly IRedisCacheClient _cache;
        private static IDatabase _cache;
        public CacheRepository()
        {
            try
            {

                // RedisManagerPool Redis = new RedisManagerPool("localhost:6379");
                // RedisManagerPool Redis = new RedisManagerPool("dishant#1276@182.74.63.142:6379");
                // _cache = Redis.GetClient();
                //var connection = ConnectionMultiplexer.Connect("182.74.63.142:6379,password=dishant#1276");

                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FrameworkPath("DataAccess\\Config\\Setting.json")));
                string Environment = json.SelectToken("Environment").ToString();
                if (Environment.IsEmpty())
                {
                    Environment = "Development";
                }
                string _connection_string = json.SelectToken(Environment).Value<string>("redis_string");

                var connection = ConnectionMultiplexer.Connect(_connection_string);
                _cache = connection.GetDatabase(json.SelectToken(Environment).Value<int>("redis_db"));

            }
            catch 
            {

            }

        }

        public void SaveData<T>(T SaveModel,string Token) where T : class
        {
          //  _cache.AddItemToSortedSet(Token, JsonConvert.SerializeObject(SaveModel));
            //_cache.Store(SaveModel);
           // _cache.Add(Token, SaveModel);
            _cache.StringSet(Token, JsonConvert.SerializeObject(SaveModel));

            
        }

        public T GetById<T>(dynamic Id)
        {
            // return _cache.GetById<T>(Id);
            return JsonConvert.DeserializeObject<T>(_cache.StringGet(Id));
        }

        public T GetData<T>(string Token)
        {
            // return _cache.Get<T>(Token);
            
            try
            {
                return JsonConvert.DeserializeObject<T>(_cache.StringGet(Token));
            }
            catch
            {
                return default(T);
            }
            

        }
        public bool DeleteData(string Token)

        {
            //   return _cache.Remove(Token);
            return _cache.KeyDelete(Token);
        }
        public bool UpdateData<T>(string Token, T SaveModel)
        {
            //return _cache.Replace(Token, SaveModel); //Replace<T>(Token,SaveModel);
            return _cache.StringSet(Token, JsonConvert.SerializeObject(SaveModel));
           
        }
    }
}
