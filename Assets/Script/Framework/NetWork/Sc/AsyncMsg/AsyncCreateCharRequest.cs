//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : AsyncCreateCharRequest
//
// Created by : Baoxue at 2015/11/27 11:58:13
//
//
//========================================================================

using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class AsyncCreateCharRequest : AbstractAsyncHttpRequest<CreateNewCharRequest, CreateNewCharResponse>
    {
        public AsyncCreateCharRequest(CreateNewCharRequest req)
            : base(req)
        {

        }
        protected override void AfterRequest(CreateNewCharResponse resp)
        {
            Debuger.Log(resp);
            LoginLogic.Instance.OnCreateChar(resp);
        }

        protected override bool IsBlock()
        {
            return true;
        }
    }
}
