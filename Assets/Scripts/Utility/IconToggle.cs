using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconToggle : MonoBehaviour
{
    public Image m_iconImage;
    public Sprite m_trueIcon;
    public Sprite m_falseIcon;
    public bool m_defaultState = true;

    // Start is called before the first frame update
    void Start()
    {
        m_iconImage.sprite = (m_defaultState) ? m_trueIcon : m_falseIcon;
    }

    public void ToggleIcon(bool state)
    {
        m_iconImage.sprite = (state) ? m_trueIcon : m_falseIcon;
    }
}
