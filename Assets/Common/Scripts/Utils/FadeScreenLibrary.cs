using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreenLibrary {

    Texture2D fadeImage;
    float fadeSpeed = 1.5f;
    float fastSpeed = 1.5f;
    float normalSpeed = 0.5f;

    float alpha = 1;
    int drawDepth = -1000;

    bool isFadingIn = false;
    bool isFadingOut = false;
    bool fadeDoing = false;
    bool fadeOutRequested = false;
    bool fadeOutCompleted = false;

    Rect fadeScreenRect = new Rect(0, 0, 100, 100);

    string keyStr = string.Empty;

	public void StartFadeIn (bool fastChangeMode)
    {
        if (fastChangeMode)
            fadeSpeed = fastSpeed;
        else
            fadeSpeed = normalSpeed;

        fadeScreenRect.width = Screen.width;
        fadeScreenRect.height = Screen.height;

        alpha = 1;
        isFadingIn = true;
        isFadingOut = false;
        fadeDoing = true;

        fadeOutRequested = false;
        fadeOutCompleted = false;
	}
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pix[i] = col;
                i++;
            }
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


    public void RequestFadeOut(string key_str, bool fastChangeMode)
    {
        if (fastChangeMode)
            fadeSpeed = fastSpeed;
        else
            fadeSpeed = normalSpeed;


        fadeScreenRect.width = Screen.width;
        fadeScreenRect.height = Screen.height;


        keyStr = key_str;

        fadeOutRequested = true;
        fadeOutCompleted = false;
        alpha = 0;
        isFadingIn = false;
        isFadingOut = true;
        fadeDoing = true;
    }


    public string CheckFadeOutCompleted()
    {
        if (fadeOutRequested && isFadingOut)
        {
            if (fadeDoing == false)
            {
                string res = keyStr;

                ResetFade();

                return res;
            }
        }

        return string.Empty;
    }

    public void ResetFade()
    {
        fadeDoing = false;
        fadeOutRequested = false;
        fadeOutCompleted = false;

        keyStr = string.Empty;
    }


    public void DoFadeGUI()
    {
        if (fadeImage == null)
            fadeImage = MakeTex(Screen.width, Screen.height, Color.black);

        if (isFadingIn)
        {
            if (fadeDoing)
            {
                alpha -= fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);

                if (alpha <= 0.05f)
                {
                    alpha = 0;
                    fadeDoing = false;
                }

                Color thisAlpha = GUI.color;
                thisAlpha.a = alpha;

                GUI.color = thisAlpha;
                GUI.depth = drawDepth;

                GUI.DrawTexture(fadeScreenRect, fadeImage);

                thisAlpha.a = 0;
                GUI.color = thisAlpha;
            }
        }
        else if (isFadingOut)
        {
            if (fadeDoing)
            {
                alpha += fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);

                if (alpha >= 0.95f)
                {
                    alpha = 1;
                    fadeDoing = false;
                }
            }

            Color thisAlpha = GUI.color;
            thisAlpha.a = alpha;

            GUI.color = thisAlpha;
            GUI.depth = drawDepth;

            GUI.DrawTexture(fadeScreenRect, fadeImage);

            thisAlpha.a = 0;
            GUI.color = thisAlpha;
        }
        
    }
       

}
