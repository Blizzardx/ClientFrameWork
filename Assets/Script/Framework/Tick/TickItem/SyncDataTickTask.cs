using Cache;
using Moudles.BaseMoudle.Character;
using NetWork;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;


public class SyncDataTickTask : AbstractTickTask
{
    static private bool m_bIsActive;
    private AsyncSyncCharDataRequest syncRequest;

    protected override bool FirstRunExecute()
    {
        return false;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_SYNC_DATA;
    }
    protected override void Beat()
    {
        if (!m_bIsActive ||(syncRequest != null && syncRequest.Running))
        {
            return;
        }
        CacheKeyInfo keyInfo = CacheKeyContants.CHAR_DATA_SNAPSHOT_KEY.BuildCacheInfo(PlayerManager.Instance.GetCharBaseData().CharId);

        CharacterDataSnapshot data = CacheManager.GetInsance().Get(keyInfo) as CharacterDataSnapshot;
        if (data == null)
        {
            return;
        }
        SyncCharDataRequest request = new SyncCharDataRequest();
        foreach (TBase tbase in data.DataList)
        {
            if (tbase is CharBaseInfo)
            {
                request.CharBaseInfo = tbase as CharBaseInfo;
                continue;
            }
            if (tbase is CharCounterInfo)
            {
                request.CharCounterInfo = tbase as CharCounterInfo;
                continue;
            }
            if (tbase is CharBagInfo)
            {
                request.CharBagInfo = tbase as CharBagInfo;
                continue;
            }
            if (tbase is CharMissionInfo)
            {
                request.CharMissionInfo = tbase as CharMissionInfo;
                continue;
            }
        }
        syncRequest = new AsyncSyncCharDataRequest(request);
        syncRequest.SyncVersion = data.Version;
        syncRequest.TryRequest();
    }
    static public void SetSyncStatus(bool status)
    {
        m_bIsActive = status;
    }
}

