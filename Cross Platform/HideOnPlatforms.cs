using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Revision 1.002 // Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>A simple script to disable a <seealso cref="GameObject"/>, based on the build device.</para>
/// </summary>
public class HideOnPlatforms : MonoBehaviour
{
    [Header("Editor")]
    /// <summary> Hide on all editor versions of Unity. </summary> 
    [SerializeField] private bool hideInAllEditors;
    /// <summary> Hide on the editor version of Unity that runs on Windows. </summary>
    [SerializeField] private bool hideInWindowsEditor;
    /// <summary> Hide on the editor version of Unity that runs on OSX. </summary>
    [SerializeField] private bool hideInMacEditor;
    /// <summary> Hide on the editor version of Unity that runs on Linux. </summary>
    [SerializeField] private bool hideInLinuxEditor;
    [Header("Standalone")]
    /// <summary> Hide on all standalone versions of Unity. </summary>
    [SerializeField] private bool hideOnAllStandalonePlatforms;
    /// <summary> Hide on the Windows version of Unity. </summary>
    [SerializeField] private bool hideOnWindows;
    /// <summary> Hide on the OSX version of Unity. </summary>
    [SerializeField] private bool hideOnMac;
    /// <summary> Hide on the Linux version of Unity. </summary>
    [SerializeField] private bool hideOnLinux;
    [Header("Mobile")]
    /// <summary> Hide on the iOS version of Unity. </summary>
    [SerializeField] private bool hideOnIOS;
    /// <summary> Hide on the Android version of Unity. </summary>
    [SerializeField] private bool hideOnAndroid;
    [Header("Console")]
    /// <summary> Hide on the Wii version of Unity. </summary>
    [SerializeField] private bool hideOnWii;
    /// <summary> Hide on the Playstation 4 version of Unity. </summary>
    [SerializeField] private bool hideOnPS4;
    /// <summary> Hide on the Xbox One version of Unity. </summary>
    [SerializeField] private bool hideOnXboxOne;
    [Header("Other")]
    /// <summary> Hide on the Lumin version of Unity. </summary>
    [SerializeField] private bool hideOnLumin;
    /// <summary> Hide on the Tizen version of Unity. </summary>
    [SerializeField] private bool hideOnTizen;
    /// <summary> Hide on the Apple TV version of Unity. </summary>
    [SerializeField] private bool hideOnAppleTV;
    void Awake()
    {
#if UNITY_EDITOR
        if (hideInAllEditors)
            gameObject.SetActive(false);
#endif
#if UNITY_EDITOR_WIN
        if (hideInWindowsEditor)
            gameObject.SetActive(false);
#endif
#if UNITY_EDITOR_OSX
        if (hideInMacEditor)
            gameObject.SetActive(false);
#endif
#if UNITY_EDITOR_LINUX
        if (hideInLinuxEditor)
            gameObject.SetActive(false);
#endif
#if UNITY_STANDALONE
        if (hideOnAllStandalonePlatforms)
            gameObject.SetActive(false);
#endif
#if UNITY_STANDALONE_OSX
        if (hideOnMac)
            gameObject.SetActive(false);
#endif
#if UNITY_STANDALONE_WIN
        if (hideOnWindows)
            gameObject.SetActive(false);
#endif
#if UNITY_STANDALONE_LINUX
        if (hideOnLinux)
            gameObject.SetActive(false);
#endif
#if UNITY_WII
        if (hideOnWii)
            gameObject.SetActive(false);
#endif
#if UNITY_IOS
        if (hideOnIOS)
            gameObject.SetActive(false);
#endif
#if UNITY_ANDROID
        if (hideOnAndroid)
            gameObject.SetActive(false);
#endif
#if UNITY_PS4
        if (hideOnPS4)
            gameObject.SetActive(false);
#endif
#if UNITY_XBOXONE
        if (hideOnXboxOne)
            gameObject.SetActive(false);
#endif
#if UNITY_LUMIN
        if (hideOnLumin)
            gameObject.SetActive(false);
#endif
#if UNITY_TIZEN
        if (hideOnTizen)
            gameObject.SetActive(false);
#endif
#if UNITY_TVOS
        if (hideOnAppleTV)
            gameObject.SetActive(false);
#endif
    }
}