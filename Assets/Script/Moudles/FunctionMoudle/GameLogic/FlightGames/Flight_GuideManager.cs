using UnityEngine;
using System.Collections;

public enum GuideStep
{
	None =0,
	FlyUpStep1 =1,
	FreeFlightUpStep2 =2,
	FreeFlightUpStep3 =3,
	CombatFlightStep4 =4,
	CombatFlightStep5 =5,
	CombatFlightStep6 =6,
	FreeFlightDownStep7 =7,
	FlyDownStep8 =8,
	LandingStep9 =9,
	End =10,
}

public class Flight_GuideManager : SingletonTemplateMon<Flight_GuideManager> {

	public Flight_UIGuide uiGuide;
	public Flight_AudioManager audioManager;
	public GuideStep guideStep;

	public static GuideStep staticGuideStep;

	void Awake()
	{
		_instance = this;
		Initialization();
	}

	public void Initialization()
	{
		if(Flight_StageController.isGuide)
		{
			staticGuideStep = GuideStep.End;
		}else{
			staticGuideStep = GuideStep.None;
		}
		guideStep = GuideStep.None;
		if(uiGuide == null)
			uiGuide = transform.parent.GetComponentInChildren<Flight_UIGuide>();
		if(audioManager == null)
			audioManager = transform.parent.GetComponentInChildren<Flight_AudioManager>();
	}

	public void CloseGuide()
	{
		if(uiGuide != null)
			uiGuide.CloseAll();
	}

	public void ChangeGuideStep(GuideStep guideStep)
	{
		if(staticGuideStep.Equals(GuideStep.End)) return;
		this.guideStep = guideStep;
		staticGuideStep = guideStep;
		switch(guideStep)
		{
			case GuideStep.FlyUpStep1:
				if(uiGuide != null)
					uiGuide.SetUpGuide();
				break;

			case GuideStep.FreeFlightUpStep2:
				if(uiGuide != null)
					uiGuide.SetLeftGuide();
				break;

			case GuideStep.FreeFlightUpStep3:
				if(uiGuide != null)
					uiGuide.SetRightGuide();
				break;

			case GuideStep.CombatFlightStep4:
				if(uiGuide != null)
				{
					uiGuide.SetLeftGuide();
					uiGuide.ShowLeftMask();
				}
				break;

			case GuideStep.CombatFlightStep5:
				if(uiGuide != null)
				{
				    uiGuide.CancelInvoke();
					uiGuide.SetRightGuide();
					uiGuide.ShowRightMask();
				}
				break;

			case GuideStep.CombatFlightStep6:
				if(uiGuide != null)
				{
					uiGuide.CloseAll();
					uiGuide.ShowCenterMask();
				}
				break;

			case GuideStep.FreeFlightDownStep7:
				if(uiGuide != null)
					uiGuide.SetLeftAndRightGuide();
				break;

			case GuideStep.FlyDownStep8:
				if(uiGuide != null)
					uiGuide.SetDownGuide();
				break;

			case GuideStep.LandingStep9:
				if(uiGuide != null)
					uiGuide.SetDownGuide();
                PlayerManager.Instance.GetCharCounterData().SetFlag(4,true);
				break;

			default:
				Debug.Log("ErrorStep");
				break;
		}
	}
}
