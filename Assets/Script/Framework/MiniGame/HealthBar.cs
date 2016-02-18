using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour {

    public Image healthIconPrefab;
    public Sprite loseIcon;
    List<Image> healthIconList = new List<Image>();
    int health;
    
    void Awake()
    {
        healthIconPrefab.gameObject.SetActive(false);
    }

    public void InitHealthBar(int health)
    {
        this.health = health;
        RemoveHealthBar();
        for (int i = 0; i < health; i++)
        {
            Image healthIcon = Instantiate(healthIconPrefab);
            healthIcon.gameObject.SetActive(true);
            healthIcon.rectTransform.SetParent(healthIconPrefab.rectTransform.parent);
            healthIcon.rectTransform.localPosition = Vector3.zero;
            healthIcon.rectTransform.localScale = Vector3.one;
            healthIconList.Add(healthIcon);
        }
    }

    public void RemoveOneHealthIcon()
    {
        if(health <= 0)
        {
            return;
        }
        health--;
        healthIconList[health].sprite = loseIcon;
    }

    public void RemoveHealthBar()
    {
        foreach (var icon in healthIconList)
        {
            Destroy(icon.gameObject);
        }
        healthIconList = new List<Image>();
    }
}
