//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : AsyncEnterGame
//
// Created by : Baoxue at 2015/11/27 12:47:17
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
    public class AsyncEnterGame : AbstractAsyncHttpRequest<EnterGameRequest, EnterGameResponse>
    {
        public AsyncEnterGame(EnterGameRequest req)
            : base(req)
        { 
        }
        protected override void AfterRequest(EnterGameResponse resp)
        {
            LoginLogic.Instance.OnEnterGame(resp);
        }

        protected override bool IsBlock()
        {
            return true;
        }
    }
}