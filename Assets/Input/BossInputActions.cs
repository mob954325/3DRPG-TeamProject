//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/BossInputActions.inputactions
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

public partial class @BossInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BossInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BossInputActions"",
    ""maps"": [
        {
            ""name"": ""Boss"",
            ""id"": ""53cdfbc6-7ddb-437e-9a30-850d67b8c4ea"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""5454377b-ed50-4475-83e0-9263f4264bbf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Clow"",
                    ""type"": ""Value"",
                    ""id"": ""66486817-f62e-4f09-92c0-b336ef0e7d13"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""58d69420-c306-4104-9176-3b513dfe46aa"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""254bbb26-8aa0-47e0-8166-6df4fbc9b119"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KW"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""70534a66-5db5-4801-b79c-08ddf64014bd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KW"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""952f2a15-7318-4b99-a8b1-ed0f43bf629c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KW"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""dfda04e7-ae5d-4e67-a9f6-d99abd1c96ff"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KW"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""717ca445-5b64-49bb-b883-d4b1c57cac36"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KW"",
                    ""action"": ""Clow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KW"",
            ""bindingGroup"": ""KW"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Boss
        m_Boss = asset.FindActionMap("Boss", throwIfNotFound: true);
        m_Boss_Move = m_Boss.FindAction("Move", throwIfNotFound: true);
        m_Boss_Clow = m_Boss.FindAction("Clow", throwIfNotFound: true);
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

    // Boss
    private readonly InputActionMap m_Boss;
    private List<IBossActions> m_BossActionsCallbackInterfaces = new List<IBossActions>();
    private readonly InputAction m_Boss_Move;
    private readonly InputAction m_Boss_Clow;
    public struct BossActions
    {
        private @BossInputActions m_Wrapper;
        public BossActions(@BossInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Boss_Move;
        public InputAction @Clow => m_Wrapper.m_Boss_Clow;
        public InputActionMap Get() { return m_Wrapper.m_Boss; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BossActions set) { return set.Get(); }
        public void AddCallbacks(IBossActions instance)
        {
            if (instance == null || m_Wrapper.m_BossActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BossActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Clow.started += instance.OnClow;
            @Clow.performed += instance.OnClow;
            @Clow.canceled += instance.OnClow;
        }

        private void UnregisterCallbacks(IBossActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Clow.started -= instance.OnClow;
            @Clow.performed -= instance.OnClow;
            @Clow.canceled -= instance.OnClow;
        }

        public void RemoveCallbacks(IBossActions instance)
        {
            if (m_Wrapper.m_BossActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IBossActions instance)
        {
            foreach (var item in m_Wrapper.m_BossActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BossActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public BossActions @Boss => new BossActions(this);
    private int m_KWSchemeIndex = -1;
    public InputControlScheme KWScheme
    {
        get
        {
            if (m_KWSchemeIndex == -1) m_KWSchemeIndex = asset.FindControlSchemeIndex("KW");
            return asset.controlSchemes[m_KWSchemeIndex];
        }
    }
    public interface IBossActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnClow(InputAction.CallbackContext context);
    }
}