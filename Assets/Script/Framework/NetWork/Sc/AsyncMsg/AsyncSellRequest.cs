using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncSellRequest : AbstractAsyncHttpRequest<SellRequest, SellResponse>
    {
        public AsyncSellRequest(SellRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(SellResponse resp)
        {
            MessageTreeLogic.Instance.OnSellResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}