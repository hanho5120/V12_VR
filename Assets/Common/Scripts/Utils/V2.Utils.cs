using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    public class Utils : MonoBehaviour
    {
        public static Color ToFloatColor(float r, float g, float b)
        {
            return ToFloatColor(r, g, b, 255);
        }

        public static Color ToFloatColor(float r, float g, float b, float a)
        {
            float n_r = r;
            if (r < 0)
                n_r = 0;
            else if (r > 255)
                n_r = 255;

            float n_g = g;
            if (g < 0)
                n_g = 0;
            else if (g > 255)
                n_g = 255;

            float n_b = b;
            if (b < 0)
                n_b = 0;
            else if (b > 255)
                n_b = 255;

            float n_a = a;
            if (a < 0)
                n_a = 0;
            else if (a > 255)
                n_a = 255;

            return new Color(n_r / 255.0f, n_g / 255.0f, n_b / 255.0f, n_a / 255.0f);
        }


        public static Texture2D MakeTex(int width, int height, Color col, bool selected_flag)
        {
            Color white_col = new Color(1, 1, 1);
            Color[] pix = new Color[width * height];

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                //if (y < (height - 1) && y > 0)
                if (y > 6)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pix[i] = col;
                        i++;
                    }
                }
                else
                {
                    if (selected_flag)
                    {
                        //흰색 선
                        for (int x = 0; x < width; x++)
                        {
                            pix[i] = white_col;
                            i++;
                        }
                    }
                    else
                    {
                        for (int x = 0; x < width; x++)
                        {
                            pix[i] = col;
                            i++;
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

    }
}