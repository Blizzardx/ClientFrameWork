using Moudles.BaseMoudle.Character;
using Moudles.BaseMoudle.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Thrift.Protocol;
using Framework.Async;
using Cache;


public class FlushDataTickTask : AbstractTickTask
{
    protected override bool FirstRunExecute()
    {
        return false;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_FLUSH_DATA;
    }
    protected override void Beat()
    {
        List<TBase> updateList = null;
        while (true)
        {
            AbstractBusinessObject obj = UpdateManager.Instance.Poll();
            if (obj == null)
            {
                break;
            }
            ICharDataConverter converter = ConverterManager.Instance.FindConverter(obj.GetType());
            if (converter == null)
            {
                Debuger.LogError(string.Format("not found converter. type:{0}", obj.GetType()));
                continue;
            }
            TBase info = converter.Convert(obj);
            obj.ResetModify();
            if (info == null)
            {
                Debuger.LogError(string.Format("convert result is null. type:{0}", obj.GetType()));
                continue;
            }
            if (updateList == null)
            {
                updateList = new List<TBase>();
            }
            updateList.Add(info);
        }
        if (updateList != null)
        {
            CacheKeyInfo keyInfo = CacheKeyContants.CHAR_DATA_SNAPSHOT_KEY.BuildCacheInfo(PlayerManager.Instance.GetCharBaseData().CharId);

            CharacterDataSnapshot data = CacheManager.GetInsance().Get(keyInfo) as CharacterDataSnapshot;

            if (data == null)
            {
                data = new CharacterDataSnapshot();
                data.Version = 0L;
                data.DataList = updateList;
            }
            else
            {
                data.Version += 1;
                List<TBase> delList = new List<TBase>();
                for (int i = 0; i < updateList.Count; i++)
                {
                    TBase newTbase = updateList[i];
                    for (int j = 0; j < data.DataList.Count; j++)
                    {
                        if (newTbase.GetType() == data.DataList[j].GetType())
                        {
                            data.DataList[j] = newTbase;
                            delList.Add(newTbase);
                            break;
                        }
                    }
                }
                foreach (TBase delTbase in delList)
                {
                    updateList.Remove(delTbase);
                }
                if (updateList.Count > 0)
                {
                    data.DataList.AddRange(updateList);
                }
            }
            CacheManager.GetInsance().Set(keyInfo, data);
        }
    }
}

