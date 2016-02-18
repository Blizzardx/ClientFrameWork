using UnityEngine;
using System.Collections;

public class ArithmeticStage : StageBase {

	public ArithmeticStage(GameStateType type): base(type)
	{

	}

	public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
		UIArithmeticGameManager.Instance.Initialize();
        EventReporter.Instance.EnterSceneReport("Arithmetic game scene ");
    }
	
	public override void EndStage()
    {
        UICamera uiCamera = WindowManager.Instance.GetUICamera().GetComponent<UICamera>();
		if(uiCamera != null)
		{
			uiCamera.allowMultiTouch = true;
		}
		if(UIArithmeticGameManager.Instance.parentTrans != null)
			GameObject.Destroy(UIArithmeticGameManager.Instance.parentTrans.gameObject);
		if(UIArithmeticGameManager.Instance.sceneObject != null)
			GameObject.Destroy(UIArithmeticGameManager.Instance.sceneObject);

        EventReporter.Instance.ExitSceneReport("Arithmetic game Scene ");
	}
}
