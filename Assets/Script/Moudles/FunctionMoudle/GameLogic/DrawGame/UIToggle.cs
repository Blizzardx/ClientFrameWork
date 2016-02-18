using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UGUI
{
    public class UIToggle : MonoBehaviour
    {

        Toggle toggle;
        UIToggleGroup group;

        // Use this for initialization
        void Awake()
        {
            toggle = GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnValueChanged);
            }
            group = GetComponentInParent<UIToggleGroup>();
        }

        void OnValueChanged(bool value)
        {
            if (value)
            {
                if (group)
                {
                    group.OnClick(this);
                }
            }
        }
    }
}

