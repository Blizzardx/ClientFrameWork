using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncCloseChatRequest : AbstractAsyncHttpRequest<CloseChatRequest, CloseChatResponse>
    {
        public AsyncCloseChatRequest(CloseChatRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(CloseChatResponse resp)
        {
            MessageTreeLogic.Instance.OnCloseChatResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}