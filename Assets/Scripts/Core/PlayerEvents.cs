using System;

public static class PlayerEvents
{
    public static Action OnPlayerDamaged;
    public static PlayerStats PlayerStatsRef;

    public static void PlayerDamaged() => OnPlayerDamaged?.Invoke();
}
