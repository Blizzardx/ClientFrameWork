using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncCloseSaleRequest : AbstractAsyncHttpRequest<CloseSaleRequest, CloseSaleResponse>
    {
        public AsyncCloseSaleRequest(CloseSaleRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(CloseSaleResponse resp)
        {
            MessageTreeLogic.Instance.OnCloseSaleResponse(resp);
        }
        protected override bool IsBlock()
        {
            return true;
        }
    }
}