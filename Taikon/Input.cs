using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Taikon;

public class Input
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;
    private static int previousScrollValue;
    
    private static MouseState currentMouseState;
    private static MouseState previousMouseState;
    
    public static KeyboardState GetKeyboardState()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        return currentKeyState;
    }
    
    public static MouseState GetMouseState()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        return currentMouseState;
    }
    
    public static bool IsKeyPressed(Keys key, bool oneShot)
    {
        if(!oneShot) return currentKeyState.IsKeyDown(key);
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }
        
    public static bool IsLeftMousePressed(bool oneShot)
    {
        if (!oneShot) return currentMouseState.LeftButton == ButtonState.Pressed;
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
    }
    
    public static bool IsRightMousePressed(bool oneShot)
    {
        if (!oneShot) return currentMouseState.RightButton == ButtonState.Pressed;
        return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;
    }

    public static bool IsScrolled(Orientation o)
    {
        switch (o)
        {
            case Orientation.Down:
                if (currentMouseState.ScrollWheelValue < previousScrollValue)
                {
                    return true;
                }

                break;
            
            case Orientation.Up:
                if (currentMouseState.ScrollWheelValue > previousScrollValue)
                {
                    return true;
                }
                
                break;
        }
        return false;
    }

    public static void FixScrollLater()
    {
        previousScrollValue = currentMouseState.ScrollWheelValue;
    }
    
    public static bool IsKeyReleased(Keys key)
    {
        return currentKeyState.IsKeyUp(key) && previousKeyState.IsKeyDown(key);
    }
    
    public static bool IsLeftMouseReleased()
    {
        return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton != ButtonState.Released;
    }
    
    public static bool IsRightMouseReleased()
    {
        return currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton != ButtonState.Released;
    }
    
    public static bool IsMouseHoveringOver(Rectangle rect, Game1 game)
    {
        // if window is not in focus, return false
        if (!game.IsActive) return false;
        return rect.Contains(currentMouseState.Position);
    }
}

public enum Orientation
{
    Up,
    Down
}