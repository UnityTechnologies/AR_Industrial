using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectActivationHandler : MonoBehaviour
{
    [SerializeField]
    Toggle m_NumberToggle;

    public Toggle numberToggle
    {
        get => m_NumberToggle;
        set => m_NumberToggle = value;
    }

    bool m_IsActive = false;

    public UnityEvent activationEvent;
    
    public void ActivateArea()
    {
        if (!m_IsActive)
        {
            m_IsActive = true;
            m_NumberToggle.isOn = true;
            if (activationEvent != null)
            {
                activationEvent.Invoke();
            }
        }
    }
}
