
namespace NaveXR.InputDevices
{
    public enum XRKeyCode
    {
        //Button 输入按钮
        //触发按下bool/触发抬起bool/触摸bool/力度float/按压bool
        Trigger = 0,
        Grip,
        TouchMiddle,
        TouchNorth,
        TouchWest,
        TouchSouth,
        TouchEast,
        Primary,
        Secondary,
        Menu,

        //Axis 输入轴
        //向量值vector2
        TouchAxis,

        //HandPose 手势
        HandPose,

        //Eye
        Eye,
    }
}
