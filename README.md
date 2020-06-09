# AR Industrial Demo
This is a multi-platform demo that shows functionality between HoloLens 2 and mobile AR (ARKit / ARCore).

It uses Unity's [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/manual/index.html) to enable tracking and rendering on the supported platforms. It uses [Universal Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.1/manual/index.html) to hit performant framerates on all platforms. 

## HoloLens 2 support
The input for HoloLens 2 uses the native WindowsMR API's to recognize the pinch gesture and native XR API's to track the hands positions and rotations. 

### Gestures
Listen for the air tap
``` 
m_MRGestures.onTappedChanged += OnTapped;
```
Check if the hand device is valid and set the particle effect to the device location and play it, call CallActivate based on the hand position
```
void OnTapped(WindowsMRTappedGestureEvent eventArgs)
{
    if (m_LeftHandDevice.isValid)
    {
         m_TapParticleSystem.transform.position = m_LeftHandPos * m_ScaleMod;
         m_TapParticleSystem.Play();
         CallActivate(m_LeftHandPos * m_ScaleMod);
    }
}
```

### Hand Tracking

Get a list of devices based on characteristics 
```
m_RightHandDevices = new List<InputDevice>();
m_LeftHandDevices = new List<InputDevice>();

InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, m_RightHandDevices);
InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, m_LeftHandDevices);
```
Assign the devices and enable the hand visual *we are assuming there to only be one device for left and right hand*
```
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
```
Subscribe to device connected and disconnected events     
```
InputDevices.deviceConnected += DeviceConnected;
InputDevices.deviceDisconnected += DeviceDisconnected;
```

On connected event, check the device characters has the `.Right` or `.Left` and assign the device
```
void DeviceConnected(InputDevice device)
{
     if((device.characteristics & InputDeviceCharacteristics.Right) != 0)
     {
         m_RightHandDevice = device;
         m_RightHandVisual.gameObject.SetActive(true);
     }
}
```

## Mobile AR Support
Uses native `Input` class for determining when the user touches the device

Use a reference to the camera to do a raycast based on the users touch position and call CallActivate()
```
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
```
