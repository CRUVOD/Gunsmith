using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TweenDefinitionTypes { Tween, AnimationCurve }

public class TweenType
{
    public TweenDefinitionTypes TweenDefinitionType = TweenDefinitionTypes.Tween;
    public Tweens.TweenCurve TweenCurve = Tweens.TweenCurve.EaseInCubic;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));

    public TweenType(Tweens.TweenCurve newCurve)
    {
        TweenCurve = newCurve;
        TweenDefinitionType = TweenDefinitionTypes.Tween;
    }
    public TweenType(AnimationCurve newCurve)
    {
        Curve = newCurve;
        TweenDefinitionType = TweenDefinitionTypes.AnimationCurve;
    }
}
