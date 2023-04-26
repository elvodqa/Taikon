using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Taikon.Graphics;

public class Primitives
{
    public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius)
    {
        Texture2D texture = new Texture2D(graphicsDevice, radius, radius);
        Color[] colorData = new Color[radius*radius];
    
        float diam = radius / 2f;
        float diamsq = diam * diam;
    
        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                int index = x * radius + y;
                Vector2 pos = new Vector2(x - diam, y - diam);
                if (pos.LengthSquared() <= diamsq)
                {
                    colorData[index] = Color.White;
                }
                else
                {
                    colorData[index] = Color.Transparent;
                }
            }
        }
    
        texture.SetData(colorData);
        return texture;
    }

    public static Texture2D CreateCircleWithOutline(GraphicsDevice graphicsDevice, int radius, int borderWidth)
    {
        int size = radius + borderWidth * 2;
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] colorData = new Color[size * size];

        float outerDiam = size / 2f;
        float innerDiam = outerDiam - borderWidth;
        float outerDiamSq = outerDiam * outerDiam;
        float innerDiamSq = innerDiam * innerDiam;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int index = x * size + y;
                Vector2 pos = new Vector2(x - outerDiam, y - outerDiam);
                float lengthSq = pos.LengthSquared();
                if (lengthSq <= outerDiamSq && lengthSq >= innerDiamSq)
                {
                    colorData[index] = Color.White;
                }
                else
                {
                    colorData[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);
        return texture;
    }

}