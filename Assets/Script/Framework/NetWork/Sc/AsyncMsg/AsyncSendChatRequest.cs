using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncSendChatRequest : AbstractAsyncHttpRequest<SendChatRequest, SendChatResponse>
    {
        public AsyncSendChatRequest(SendChatRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(SendChatResponse resp)
        {
            MessageTreeLogic.Instance.OnSendChatResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}