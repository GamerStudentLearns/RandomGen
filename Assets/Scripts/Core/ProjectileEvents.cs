using System;

public static class ProjectileEvents
{
    public static Action<EnemyHealth> OnPlayerProjectileHitEnemy;
    public static void PlayerProjectileHit(EnemyHealth enemy)
        => OnPlayerProjectileHitEnemy?.Invoke(enemy);

    public static Action<Projectile> OnPlayerProjectileFired;
    public static void PlayerProjectileFired(Projectile proj)
        => OnPlayerProjectileFired?.Invoke(proj);

    public static Action OnEnemyProjectileDestroyed;
    public static void EnemyProjectileDestroyed()
        => OnEnemyProjectileDestroyed?.Invoke();
}
