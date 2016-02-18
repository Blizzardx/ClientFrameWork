using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncBidRequest : AbstractAsyncHttpRequest<BidRequest, BidResponse>
    {
        public AsyncBidRequest(BidRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(BidResponse resp)
        {
            MessageTreeLogic.Instance.OnBidResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}