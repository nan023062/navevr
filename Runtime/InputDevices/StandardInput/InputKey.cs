﻿
namespace Nave.VR
{
    public enum InputKey
    {
        //Button 输入按钮
        //触发按下bool/触发抬起bool/触摸bool/力度float/按压bool
        Trigger = 0,
        Grip,
        Middle,
        North,
        West,
        South,
        East,
        Primary,
        Secondary,
        Menu,

        //Axis 输入轴
        //向量值vector2
        Axis,

        //HandPose 手势
        Pose,

        //Eye
        Eye,

        //Foot
        Foot,

        //Pevis
        Pevis,
    }
}
