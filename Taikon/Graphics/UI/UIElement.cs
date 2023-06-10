using Microsoft.Xna.Framework;

namespace Taikon.Graphics.UI;

public class UIElement
{
    public Vector2 Position;
    public Vector2 Size;
    public bool IsVisible = true;

    public virtual void Update()
    {
    }
    
    public virtual void Draw()
    {
        if (IsVisible)
        {
            
        }
    }
}