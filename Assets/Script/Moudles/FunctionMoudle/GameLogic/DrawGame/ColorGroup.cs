using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UGUI
{
    public class ColorGroup : UIToggleGroup
    {
        public DrawSomething draw;

        void Start()
        {
            //painting = FindObjectOfType<Painting>();
        }
        public override void OnClick(UIToggle toggle)
        {
            base.OnClick(toggle);
            var t = (UIToggleColor)toggle;
            draw.color = t.color;
        }
    }
}

