using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncBuyRequest : AbstractAsyncHttpRequest<BuyRequest, BuyResponse>
    {
        public AsyncBuyRequest(BuyRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(BuyResponse resp)
        {
            MessageTreeLogic.Instance.OnBuyResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}
