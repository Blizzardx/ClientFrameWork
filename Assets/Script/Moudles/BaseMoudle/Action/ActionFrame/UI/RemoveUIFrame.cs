﻿using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RemoveUIFrame : AbstractActionFrame {

	private RemoveUIFrameConfig m_FrameConfig;
	
	public RemoveUIFrame(ActionPlayer action, ActionFrameData data)
		: base(action, data)
	{
		m_FrameConfig = m_FrameData.RemoveUIFrame;
	}
	
	public override bool IsTrigger(float fRealTime)
	{
		if (null == m_FrameData)
		{
			return false;
		}
		
		if (fRealTime >= m_FrameData.Time)
		{
			return true;
		}
		
		return false;
	}
	
	public override bool IsFinish(float fRealTime)
	{
		return true;
	}
	
	public override void Play()
	{
		
	}
	
	protected override void Execute()
	{
		OnTrigger();
	}
	
	public override void Pause(float fTime)
	{
		
	}
	
	public override void Stop()
	{
		OnTrigger();
		
	}
	public override void Destory()
	{
		
	}
	private void OnTrigger()
	{
	    WindowManager.Instance.CloseWindow(m_FrameConfig.WindowId);
	}
}