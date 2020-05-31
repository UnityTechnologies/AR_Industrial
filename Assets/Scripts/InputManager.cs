using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WindowsMR;
#endif
public class InputManager : MonoBehaviour
{
#if UNITY_WSA
    [SerializeField]
    WindowsMRGestures m_MRGestures;
    
    public WindowsMRGestures mrGestures
    {
        get => m_MRGestures;
        set => m_MRGestures = value;
    }
#endif

    [SerializeField]
    Transform m_SessionOrigin;
    
    public Transform sessionOrigin
    {
        get => m_SessionOrigin;
        set => m_SessionOrigin = value;
    }

    [SerializeField]
    Transform m_LeftHandVisual;
    public Transform leftHandVisual
    {
        get => m_LeftHandVisual;
        set => m_LeftHandVisual = value;
    }

    [SerializeField]
    Transform m_RightHandVisual;
    
    public Transform rightHandVisual
    {
        get => m_RightHandVisual;
        set => m_RightHandVisual = value;
    }

    [SerializeField]
    ParticleSystem m_TapParticleSystem;
    
    public ParticleSystem tapParticleSystem
    {
        get => m_TapParticleSystem;
        set => m_TapParticleSystem = value;
    }

    RaycastHit m_Hit;
    Ray m_CamerRay;
    Vector3 m_LeftHandPos;
    Vector3 m_RightHandPos;
    Quaternion m_LeftHandRot;
    Quaternion m_RightHandRot;
    const float k_SphereCastRadius = 10.0f;
    Collider[] m_HitColliders;
    float m_ScaleMod;
    InputDevice m_LeftHandDevice;
    InputDevice m_RightHandDevice;
    List<InputDevice> m_LeftHandDevices;
    List<InputDevice> m_RightHandDevices;

    [SerializeField]
    TMP_Text m_DebugText;
    
    void OnEnable()
    {
        // find left and right hand
        m_RightHandDevices = new List<InputDevice>();
        m_LeftHandDevices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, m_RightHandDevices);
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, m_LeftHandDevices);

        if(m_RightHandDevices.Count > 0)
        {
            m_RightHandDevice = m_RightHandDevices[0];
            m_RightHandVisual.gameObject.SetActive(true);
        }

        if(m_LeftHandDevices.Count > 0)
        {
            m_LeftHandDevice = m_LeftHandDevices[0];
            m_LeftHandVisual.gameObject.SetActive(true);
        }
        
        // listen for device events
        InputDevices.deviceConnected += DeviceConnected;
        InputDevices.deviceDisconnected += DeviceDisconnected;
        
#if UNITY_WSA
        // listen for air tap
        m_MRGestures.onTappedChanged += OnTapped;
#endif
        m_ScaleMod = m_SessionOrigin.localScale.x;
    }

    void OnDisable()
    {
#if ENABLE_WINMD_SUPPORT
        m_MRGestures.onTappedChanged -= OnTapped;
#endif
        InputDevices.deviceConnected -= DeviceConnected;
        InputDevices.deviceDisconnected -= DeviceDisconnected;
    }

    void Update()
    {
        // hand input for HoloLens 2
        if (m_LeftHandDevice.isValid)
        {
            m_LeftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out m_LeftHandPos);
            m_LeftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out m_LeftHandRot);
            // account for origin scaling
            m_LeftHandVisual.position = m_LeftHandPos * m_ScaleMod;
            m_LeftHandVisual.rotation = m_LeftHandRot;
        }

        if (m_RightHandDevice.isValid)
        {
            m_RightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out m_RightHandPos);
            m_RightHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out m_RightHandRot);
            // account for origin scaling
            m_RightHandVisual.position = m_RightHandPos * m_ScaleMod;
            m_RightHandVisual.rotation = m_RightHandRot;
        }
        
        // touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                m_CamerRay = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(m_CamerRay, out m_Hit))
                {
                    m_TapParticleSystem.transform.position = m_Hit.point;
                    m_TapParticleSystem.Play();
                    CallActivate(m_Hit.point);
                }
            }
        }
    }

    void DeviceConnected(InputDevice device)
    {
        if((device.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            m_RightHandDevice = device;
            m_RightHandVisual.gameObject.SetActive(true);
        }

        if((device.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            m_LeftHandDevice = device;
            m_LeftHandVisual.gameObject.SetActive(true);
        }
    }

    void DeviceDisconnected(InputDevice device)
    {
        if(!m_RightHandDevice.isValid)
        {
            m_RightHandDevice = default(InputDevice);
            m_RightHandVisual.gameObject.SetActive(false);
        }

        if (!m_LeftHandDevice.isValid)
        {
            m_LeftHandDevice = default(InputDevice);
            m_LeftHandVisual.gameObject.SetActive(false);
        }
    }
    
#if UNITY_WSA
    void OnTapped(WindowsMRTappedGestureEvent eventArgs)
    {
        if (m_LeftHandDevice.isValid)
        {
            m_TapParticleSystem.transform.position = m_LeftHandPos * m_ScaleMod;
            m_TapParticleSystem.Play();
            CallActivate(m_LeftHandPos * m_ScaleMod);
        }

        if (m_RightHandDevice.isValid)
        {
            m_TapParticleSystem.transform.position = m_RightHandPos * m_ScaleMod;
            m_TapParticleSystem.Play();
            CallActivate(m_RightHandPos * m_ScaleMod);
        }
    }
#endif

    void CallActivate(Vector3 HandPosition)
    {
        m_HitColliders = Physics.OverlapSphere(HandPosition, k_SphereCastRadius);
        foreach (Collider hitCollider in m_HitColliders)
        {
            if(hitCollider.TryGetComponent(out ObjectActivationHandler objectHandler))
            {
                objectHandler.ActivateArea();
            }
        }
    }
}
