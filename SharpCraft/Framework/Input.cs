using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace SharpCraft.Framework;

public static class Input
{
    public static Vector2 MousePos => _mousePos;
    public static Vector2 MouseDelta => _mouseDelta;
    public static float ScrollDelta => _scrollDelta;
    
    private static Vector2 _mousePos;
    private static Vector2 _mouseDelta;
    private static Vector2 _lastMousePos;
    private static float _scrollDelta;
    
    private static readonly Dictionary<MouseButton, bool> MouseButtons = [];
    private static readonly Dictionary<MouseButton, bool> LastMouseButtons = [];

    private static readonly Dictionary<Key, bool> Keys = [];
    private static readonly Dictionary<Key, bool> LastKeys = [];
    
    private static IInputContext _ctx = null!;

    public static bool IsMouseButtonDown(MouseButton button)
    {
        return MouseButtons.TryGetValue(button, out var pressed) && pressed;
    }

    public static bool IsMouseButtonUp(MouseButton button)
    {
        return !IsMouseButtonDown(button);
    }

    public static bool IsMouseButtonPressed(MouseButton button)
    {
        return !(LastMouseButtons.TryGetValue(button, out var lastPressed) && lastPressed) &&
               MouseButtons.TryGetValue(button, out var pressed) && pressed;
    }

    public static bool IsMouseButtonReleased(MouseButton button)
    {
        return !IsMouseButtonPressed(button);
    }

    public static bool IsKeyDown(Key key)
    {
        return Keys.TryGetValue(key, out var pressed) && pressed;
    }

    public static bool IsKeyUp(Key key)
    {
        return !IsKeyDown(key);
    }

    public static bool IsKeyPressed(Key key)
    {
        return !(LastKeys.TryGetValue(key, out var lastPressed) && lastPressed) &&
               Keys.TryGetValue(key, out var pressed) && pressed;
    }

    public static bool IsKeyReleased(Key key)
    {
        return !IsKeyPressed(key);
    }

    public static void SetCursorMode(CursorMode mode)
    {
        foreach (var mouse in _ctx.Mice)
        {
            mouse.Cursor.CursorMode = mode;
        }
    }
    
    internal static void CreateContext(IWindow window)
    {
        _ctx = window.CreateInput();

        foreach (var mouse in _ctx.Mice)
        {
            SetupMouse(mouse);
        }

        foreach (var keyboard in _ctx.Keyboards)
        {
            SetupKeyboard(keyboard);
        }

        _ctx.ConnectionChanged += (device, connected) =>
        {
            if (!connected) return;

            switch (device)
            {
                case IMouse mouse:
                    SetupMouse(mouse);
                    break;
                
                case IKeyboard keyboard:
                    SetupKeyboard(keyboard);
                    break;
            }
        };
    }

    internal static void Update()
    {
        foreach (var (button, state) in MouseButtons)
        {
            LastMouseButtons[button] = state;
        }
        
        foreach (var (key, state) in Keys)
        {
            LastKeys[key] = state;
        }

        _mouseDelta = _mousePos - _lastMousePos;
        _lastMousePos = _mousePos;
    }

    private static void SetupKeyboard(IKeyboard keyboard)
    {
        keyboard.KeyDown += KeyDown;
        keyboard.KeyUp += OnKeyUp;
    }

    private static void OnKeyUp(IKeyboard keyboard, Key key, int scancode)
    {
        Keys[key] = false;
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int scancode)
    {
        Keys[key] = true;
    }

    private static void SetupMouse(IMouse mouse)
    {
        mouse.MouseMove += OnMouseMove;
        mouse.MouseDown += OnMouseDown;
        mouse.MouseUp += OnMouseUp;
        mouse.Scroll += OnScroll;
    }

    private static void OnScroll(IMouse mouse, ScrollWheel wheel)
    {
        _scrollDelta = Math.Max(wheel.X, wheel.Y);
    }

    private static void OnMouseUp(IMouse mouse, MouseButton button)
    {
        MouseButtons[button] = false;
    }

    private static void OnMouseDown(IMouse mouse, MouseButton button)
    {
        MouseButtons[button] = true;
    }

    private static void OnMouseMove(IMouse mouse, Vector2 pos)
    {
        _mousePos = pos;
    }
}