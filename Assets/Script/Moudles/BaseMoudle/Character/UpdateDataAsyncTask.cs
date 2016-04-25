using Communication;
using Framework.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Moudles.BaseMoudle.Character
{
    public class UpdateDataAsyncTask : IAsyncTask
    {
        public bool Running { get; set; }

        public List<TBase> UpdateList { get; set; }


        public AsyncState BeforeAsyncTask()
        {
            Running = true;
            return AsyncState.DoAsync;
        }

        public AsyncState DoAsyncTask()
        {
            if (UpdateList == null || UpdateList.Count == 0)
            {
                return AsyncState.AfterAsync;
            }
            foreach(TBase tbase in UpdateList)
            {
                try
                {
                    byte[] bytes = ThriftSerialize.Serialize(tbase);
                    if (bytes == null)
                    {
                        continue;
                    }
                    //string sql = string.Format("replace into {0}(id, data) values((@id) ,(@data))");
                    //SqliteCommand cmd = SqliteDbAccess.Instance.CreateSqliteCommand();
                    //cmd.CommandText = sql;
                    //cmd.Parameters.Add(new SqliteParameter("@id", tbase.GetType().FullName));
                    //cmd.Parameters.Add(new SqliteParameter("@data", bytes));
                    int n = 0;// cmd.ExecuteNonQuery();
                    if (n == 0)
                    {
                        Debuger.LogError(string.Format("{0} update fail.", tbase.GetType().FullName));
                    }
                }
                catch (Exception e)
                {
                    Debuger.LogError(e.Message);
                }
            }
            return AsyncState.AfterAsync;
        }

        public AsyncState AfterAsyncTask()
        {
            Running = false;
            if (UpdateList != null)
            {
                UpdateList.Clear();
            }
            return AsyncState.Done;
        }
    }
}
