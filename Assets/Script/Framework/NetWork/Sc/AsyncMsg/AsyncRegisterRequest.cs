//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : AsyncRegisterRequest
//
// Created by : Baoxue at 2015/11/25 11:54:52
//
//
//========================================================================

using NetWork;
using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AsyncRegisterRequest : AbstractAsyncHttpRequest<RegisterRequest, RegisterResponse>
    {
    public AsyncRegisterRequest(RegisterRequest req)
            : base(req)
        {

        }
    protected override void AfterRequest(RegisterResponse resp)
        {
            Debuger.Log(resp);
            LoginLogic.Instance.OnRegisterResponse(resp);
        }

        protected override bool IsBlock()
        {
            return true;
        }
    }

