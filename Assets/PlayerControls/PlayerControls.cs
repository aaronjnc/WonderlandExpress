//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/PlayerControls/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""ClickEvents"",
            ""id"": ""56637187-0079-4e79-a787-161c6344075f"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""661b4999-f2b8-48d2-99db-c2d8c24f563e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ZoomOut"",
                    ""type"": ""Button"",
                    ""id"": ""0539a495-9e53-4ca7-8f00-4bc36fe6d9c7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0d47953a-e46b-4a9a-b07e-a451b0850aed"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""577c0c80-1e19-4b71-8dee-ecfd758edfb6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""df164fd6-de07-475a-b4b7-311a41f46209"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a762f2f7-1e6f-4be8-93b5-9ecf1fcd96c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6a731a5c-52ba-44c2-8683-ef5fb7de98d4"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ClickEvents
        m_ClickEvents = asset.FindActionMap("ClickEvents", throwIfNotFound: true);
        m_ClickEvents_Click = m_ClickEvents.FindAction("Click", throwIfNotFound: true);
        m_ClickEvents_ZoomOut = m_ClickEvents.FindAction("ZoomOut", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Pause = m_Menu.FindAction("Pause", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // ClickEvents
    private readonly InputActionMap m_ClickEvents;
    private IClickEventsActions m_ClickEventsActionsCallbackInterface;
    private readonly InputAction m_ClickEvents_Click;
    private readonly InputAction m_ClickEvents_ZoomOut;
    public struct ClickEventsActions
    {
        private @PlayerControls m_Wrapper;
        public ClickEventsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Click => m_Wrapper.m_ClickEvents_Click;
        public InputAction @ZoomOut => m_Wrapper.m_ClickEvents_ZoomOut;
        public InputActionMap Get() { return m_Wrapper.m_ClickEvents; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ClickEventsActions set) { return set.Get(); }
        public void SetCallbacks(IClickEventsActions instance)
        {
            if (m_Wrapper.m_ClickEventsActionsCallbackInterface != null)
            {
                @Click.started -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnClick;
                @ZoomOut.started -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnZoomOut;
                @ZoomOut.performed -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnZoomOut;
                @ZoomOut.canceled -= m_Wrapper.m_ClickEventsActionsCallbackInterface.OnZoomOut;
            }
            m_Wrapper.m_ClickEventsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @ZoomOut.started += instance.OnZoomOut;
                @ZoomOut.performed += instance.OnZoomOut;
                @ZoomOut.canceled += instance.OnZoomOut;
            }
        }
    }
    public ClickEventsActions @ClickEvents => new ClickEventsActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Pause;
    public struct MenuActions
    {
        private @PlayerControls m_Wrapper;
        public MenuActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_Menu_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IClickEventsActions
    {
        void OnClick(InputAction.CallbackContext context);
        void OnZoomOut(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnPause(InputAction.CallbackContext context);
    }
}
