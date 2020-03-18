using Spine;
using Spine.Unity;
using UnityEngine;

public class _SpineUtility
{
    public static void PlayAnimation (SkeletonAnimation sa, int trackIndex, string animName, bool isLoop,  float speed = 1f)
    {
        ExposedList<Spine.Animation> anims = sa.skeleton.Data.Animations;

        string strCurrAnimation = string.Empty;
        if (trackIndex == 0)
            strCurrAnimation = sa.AnimationState.GetCurrent (0).Animation.Name;

        for (int i = 0; i < anims.Count; i++)
        {
            if (anims.Items[i].Name.Equals (animName))
            {
                TrackEntry entry = sa.AnimationState.SetAnimation (trackIndex, animName, isLoop);
                entry.TimeScale = speed;
                if (!isLoop && trackIndex == 0 && !strCurrAnimation.Equals (string.Empty))
                    sa.AnimationState.AddAnimation (0, strCurrAnimation, true, 0);
                break;
            }
        }
    }

    public static void PlayAnimation ( SkeletonGraphic sg, int trackIndex, string animName, bool isLoop, float speed = 1f )
    {
        ExposedList<Spine.Animation> anims = sg.Skeleton.Data.Animations;

        string strCurrAnimation = string.Empty;
        if (trackIndex == 0)
            strCurrAnimation = sg.AnimationState.GetCurrent (0).Animation.Name;

        for (int i = 0; i < anims.Count; i++)
        {
            if (anims.Items[i].Name.Equals (animName))
            {
                TrackEntry entry = sg.AnimationState.SetAnimation (trackIndex, animName, isLoop);
                entry.TimeScale = speed;
                if (!isLoop && trackIndex == 0 && !strCurrAnimation.Equals (string.Empty))
                    sg.AnimationState.AddAnimation (0, strCurrAnimation, true, 0);
                break;
            }
        }
    }

    public static void SetSkinColor (SkeletonAnimation sa, Color c )
    {
        sa.skeleton.SetColor (c);
    }

    public static void SetColorTintObjects (SkeletonAnimation sa, Color c )
    {
        ExposedList<Slot> tintSlots = new ExposedList<Slot> ();
        ExposedList<Slot> slots = sa.skeleton.Slots;

        for (int i = 0; i < slots.Count; i++)
        {
            string tPrefix = slots.Items[i].Data.Name.Split ('_')[0];
            if (tPrefix.Equals ("t"))
                tintSlots.Add (slots.Items[i]);
        }

        for (int x = 0; x < tintSlots.Count; x++)
        {
            tintSlots.Items[x].SetColor (c);
        }

        Debug.LogError ("Done");
    }
}
