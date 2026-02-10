using System;

public static class RunEvents
{
    public static Action OnFloorChanged;

    public static void FloorChanged()
        => OnFloorChanged?.Invoke();
}
