using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncRefuseBidRequest : AbstractAsyncHttpRequest<RefuseBidRequest, RefuseBidResponse>
    {
        public AsyncRefuseBidRequest(RefuseBidRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(RefuseBidResponse resp)
        {
            MessageTreeLogic.Instance.OnSellRefuseResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}