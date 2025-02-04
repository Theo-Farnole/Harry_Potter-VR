﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public enum FadeType { FadeIn, FadeOut }

public static class GraphicExtension
{
    /// <summary>
    /// Fade graphic component.
    /// </summary>
    public static Coroutine Fade(this Graphic g, FadeType fadeType, float timeToFadout)
    {
        return new Timer(g, timeToFadout, (float completion) =>
        {
            var color = g.color;

            switch (fadeType)
            {
                case FadeType.FadeIn:
                    color.a = completion;
                    break;

                case FadeType.FadeOut:
                    color.a = 1 - completion;
                    break;
            }

            g.color = color;
        }).Coroutine;
    }

    public static void SetAlpha(this Graphic g, float alpha)
    {
        Color c = g.color;
        c.a = alpha;
        g.color = c;
    }
}
