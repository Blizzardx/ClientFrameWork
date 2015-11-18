using Framework.Async;
using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace NetWork
{
    public abstract class AbstractAsyncHttpRequest<REQ, RESP> : IAsyncTask
        where REQ : TBase
        where RESP : TBase
    {
        private bool running = false;
        private Header header = new Header();
        protected TBase req;
        private ResponseMessage responseMessage;
        public AbstractAsyncHttpRequest(REQ req)
        {
            this.req = req;
            this.header.OrderId = HttpManager.Instance.GetNextOrderId();
            this.header.Sk = HttpManager.Instance.Sk;
        }

        public AsyncState BeforeAsyncTask()
        {
            running = true;
            if (IsBlock())
            {
                //TODO 给界面蒙上loading
                WindowManager.Instance.OpenWindow(WindowID.Loading);

            }

            return AsyncState.DoAsync;
        }

        public AsyncState DoAsyncTask()
        {
            responseMessage = HttpManager.Instance.PostMessage(header, req);
            return AsyncState.AfterAsync;
        }

        public AsyncState AfterAsyncTask()
        {
            running = false;
            if (IsBlock())
            {
                //TODO 去掉loading
                WindowManager.Instance.HideAllWindow();

            }

            if (responseMessage.Ex != null)
            {
                //TODO 异常处理


                return AsyncState.Done;
            }
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //TODO 状态码不正确


                return AsyncState.Done;
            }

            if (responseMessage.EventList != null)
            {
                //统一事件处理


            }

            AfterRequest((RESP)responseMessage.Message);

            return AsyncState.Done;
        }

        /// <summary>
        /// 尝试这次请求
        /// </summary>
        public void TryRequest()
        {
            if (running)
            {
                return;
            }
            AsyncManager.Instance.ExecuteAsyncTask(this);
        }

        protected abstract void AfterRequest(RESP resp);

        /// <summary>
        /// 是否阻塞请求
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsBlock();
    }
}
