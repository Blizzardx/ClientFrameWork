using System;
using Framework.Common;

namespace Framework.Task
{
    public class TaskElement : ITask
    {
        private int     m_iTaskType;
        private object  m_TaskParam;
        private object  m_TaskCustomParam;
        private Exception m_ErrorInfo;
        private Action<int, object, object, Exception> m_OnCompletedCallBack;

        public TaskElement(int taskType, Action<int, object, object, Exception> callBack,object param, object customParam = null)
        {
            m_iTaskType = taskType;
            m_OnCompletedCallBack = callBack;
            m_TaskParam = param;
            m_TaskCustomParam = customParam;
        }
        public int GetTaskType()
        {
            return m_iTaskType;
        }
        public object GetTaskParam()
        {
            return m_TaskParam;
        }
        public object GetResult()
        {
            return m_TaskCustomParam;
        }
        public Action<int, object, object, Exception> GetCompletedCallBack()
        {
            return m_OnCompletedCallBack;
        }
    }
}
