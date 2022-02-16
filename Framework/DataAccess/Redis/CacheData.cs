using Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.DataAccess.Redis
{
    public class CacheData
    {
        CacheRepository _cache;
        public CacheData()
        {
            _cache = new CacheRepository();
        }

        public FreeAction GetFreeAction()
        {
            return _cache.GetData<FreeAction>("FreeActions");
        }
    }
}
