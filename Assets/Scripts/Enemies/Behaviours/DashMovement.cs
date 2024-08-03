using UnityEngine;

public class DashMovement : EnemyMovement
{
    Vector2 chargeDirection;

    // We calculate the direction where the enemy charges towards first,
    // i.e. where the player is when the enemy spawns.
    protected override void Start()
    {
        base.Start();
        chargeDirection = (player.transform.position - transform.position).normalized;
    }

    // Instead of moving towards the player, we just move towards
    // the direction we are charging towards.
    public override void Move()
    {
        transform.position += stats.currentSpeed * Time.deltaTime * (Vector3)chargeDirection;

        if (chargeDirection.x != 0) // Flip the sprite based on the horizontal direction
        {
            sprite.flipX = chargeDirection.x > 0; // Flip when moving right since default is left
        }
    }
}