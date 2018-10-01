# GlobalHook

Very simple mouse, keyboard global hooking library written in C#

## HowToUse

### Hooking

Add `KeyboardHook.HookStart()` or `MouseHook.HookStart()` at starts.
**All events must return boolean value**. If returned value is false, that event go out.

### Simulation

Just call method

## Examples

Locking left windows key

```csharp
KeyboardHook.KeyDown += (int vkCode) => (Keys)vkCode != Keys.LWin;
```

Watch all mouse down

```csharp
MouseHook.MouseDown += (MouseEventType type, int x, int y) => {
    Console.WriteLine($"{type} Down at: {x}, {y}");
    return true;
};
```

Press `Escape`

```csharp
KeyboardSimulation.MakeKeyEvent((int)Keys.Escape, KeyboardEventType.KEYCLICK);
```

Scroll mouse down

```csharp
MouseSimulation.MouseScroll(MouseScrollType.DOWN);
```
