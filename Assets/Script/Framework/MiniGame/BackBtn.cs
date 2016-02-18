using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
	}
	
	void OnClick()
    {
        WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
        //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
    }
}
