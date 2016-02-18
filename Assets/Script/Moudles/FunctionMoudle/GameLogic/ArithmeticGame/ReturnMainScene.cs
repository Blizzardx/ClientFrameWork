using UnityEngine;
using System.Collections;

public class ReturnMainScene : MonoBehaviour {

	void Start()
	{
		RegisterEvent();
	}

	void RegisterEvent()
	{
		UIEventListener.Get (gameObject).onClick = OnObjectClick;
	}

	void OnObjectClick(GameObject go)
	{
        WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
        //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
    }
}
