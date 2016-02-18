using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace NetWork
{
    public class AsyncLoginRequest : AbstractAsyncHttpRequest<LoginRequest, LoginResponse>
    {
        public AsyncLoginRequest(LoginRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(LoginResponse resp)
        {
            Debuger.Log(resp);
            if (resp.FailSystemInfo == null)
            {
                HttpManager.Instance.Sk = responseMessage.Header.Sk;
            }
            LoginLogic.Instance.OnLoginResponse(resp);
            return;
        }

        protected override bool IsBlock()
        {
            return true;
        }
    }
}
