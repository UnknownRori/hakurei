using SDL3;

namespace Hakurei.Engine;


public class Input
{
    private static bool[] currentKeys = new bool[512];
    private static bool[] previousKeys = new bool[512];

    // TODO : Way to add custom mapping easily
    public enum Key
    {
        Unknown = 0,

        Z,
        X,

        Shift,
        Space,
        Escape,
        Enter,

        Left,
        Right,
        Up,
        Down,
    }

    private static readonly Dictionary<Key, SDL.Scancode> keyMap = new()
    {
        { Key.X, SDL.Scancode.X },
        { Key.Z, SDL.Scancode.Z },
        { Key.Shift, SDL.Scancode.LShift },
        { Key.Space, SDL.Scancode.Space },
        { Key.Escape, SDL.Scancode.Escape },

        { Key.Left, SDL.Scancode.Left },
        { Key.Right, SDL.Scancode.Right },
        { Key.Up, SDL.Scancode.Up },
        { Key.Down, SDL.Scancode.Down },
    };

    public static void Update()
    {
        Array.Copy(currentKeys, previousKeys, currentKeys.Length);

        ReadOnlySpan<bool> keyboard = SDL.GetKeyboardState(out _);

        for (int i = 0; i < keyboard.Length; i++)
        {
            currentKeys[i] = keyboard[i];
        }
    }

    public static bool IsKeyDown(Key key)
    {
        return currentKeys[(int)keyMap[key]];
    }

    public static bool IsKeyPressed(Key key)
    {
        return currentKeys[(int)keyMap[key]]
            && !previousKeys[(int)keyMap[key]];
    }

    public static bool IsKeyReleased(Key key)
    {
        return !currentKeys[(int)keyMap[key]]
            && previousKeys[(int)keyMap[key]];
    }
}
