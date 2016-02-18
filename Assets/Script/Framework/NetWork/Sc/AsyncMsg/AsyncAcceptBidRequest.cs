using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncAcceptBidRequest : AbstractAsyncHttpRequest<AcceptBidRequest, AcceptBidResponse>
    {
        public AsyncAcceptBidRequest(AcceptBidRequest req)
            : base(req)
        {
            
        }
        protected override void AfterRequest(AcceptBidResponse resp)
        {
            MessageTreeLogic.Instance.OnSellAcceptResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}