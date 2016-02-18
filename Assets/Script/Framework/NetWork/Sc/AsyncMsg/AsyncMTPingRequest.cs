using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncMTPingRequest : AbstractAsyncHttpRequest<MTPingRequest, MTPingResponse>
    {
        public AsyncMTPingRequest(MTPingRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(MTPingResponse resp)
        {
        }
        protected override bool IsBlock()
        {
            return false;
        }
    }
}