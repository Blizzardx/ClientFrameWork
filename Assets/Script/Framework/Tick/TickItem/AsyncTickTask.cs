using Framework.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AsyncTickTask : AbstractTickTask
{
    protected override bool FirstRunExecute()
    {
        return false;
    }

    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_ASYNC;
    }

    protected override void Beat()
    {
        IInternalMessage message = SystemMessageQueue.Instance.Poll();
        if (message != null)
        {
            switch (message.GetMessageId())
            {
                case AsyncTaskMessage.ASYNC_MESSAGE_ID:
                    {
                        AsyncTaskMessage asyncTaskMessage = message as AsyncTaskMessage;
                        AsyncState state = asyncTaskMessage.State;
                        IAsyncTask asyncTask = asyncTaskMessage.AsyncTask;
                        if (state == AsyncState.DoAsync)
                        {
                            throw new ApplicationException(string.Format("asyncTask:{0} [DoAsyncTask] infinite loop.", asyncTask.GetType().FullName));
                        }
                        AsyncManager.Instance.ExecuteAsyncTask(state, asyncTask);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}

