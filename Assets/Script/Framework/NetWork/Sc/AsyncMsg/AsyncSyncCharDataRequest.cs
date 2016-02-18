using Cache;
using Moudles.BaseMoudle.Character;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncSyncCharDataRequest : AbstractAsyncHttpRequest<SyncCharDataRequest, SyncCharDataResponse>
    {
        public long SyncVersion { get; set; }

        public AsyncSyncCharDataRequest(SyncCharDataRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(SyncCharDataResponse resp)
        {
            CacheKeyInfo keyInfo = CacheKeyContants.CHAR_DATA_SNAPSHOT_KEY.BuildCacheInfo(PlayerManager.Instance.GetCharBaseData().CharId);

            CharacterDataSnapshot data = CacheManager.GetInsance().Get(keyInfo) as CharacterDataSnapshot;
            if (data == null)
            {
                return;
            }
            if (data.Version != SyncVersion)
            {
                return;
            }
            CacheManager.GetInsance().Del(keyInfo);
        }

        protected override bool IsBlock()
        {
            return false;
        }
    }
}
