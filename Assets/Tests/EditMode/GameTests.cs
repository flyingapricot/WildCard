using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    private GameObject player;
    private Rigidbody2D rb;
    private PlayerMovement playerMove;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject with a PlayerController and Rigidbody2D
        player = new GameObject("Player");
        rb = player.AddComponent<Rigidbody2D>();

        // Optionally set up initial conditions here
        rb.gravityScale = 0; // Disable gravity if not needed
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        Object.Destroy(player);
    }


    [Test]
    public IEnumerator PlayerMovesRight()
    {
        PlayerStats.instance.MoveSpeed = 10f;

        // Simulate movement
        Vector3 initialPosition = player.transform.position;
        playerMove.MoveRight();

        // Allow some time for movement
        yield return new WaitForSeconds(0.1f);

        // Assert that the player has moved to the right
        Vector3 newPosition = player.transform.position;
        Assert.Greater(newPosition.x, initialPosition.x, "Player moves to the right.");
    }

    public IEnumerator PlayerMovesLeft()
    {
        PlayerStats.instance.MoveSpeed = 10f;

        // Simulate movement
        Vector3 initialPosition = player.transform.position;
        playerMove.MoveLeft();

        // Allow some time for movement
        yield return new WaitForSeconds(0.1f);

        // Assert that the player has moved to the left
        Vector3 newPosition = player.transform.position;
        Assert.Greater(newPosition.x, initialPosition.x, "Player moves to the left.");
    }

    public IEnumerator PlayerMovesUp()
    {
        PlayerStats.instance.MoveSpeed = 10f;

        // Simulate movement
        Vector3 initialPosition = player.transform.position;
        playerMove.MoveUp();

        // Allow some time for movement
        yield return new WaitForSeconds(0.1f);

        // Assert that the player has moved up
        Vector3 newPosition = player.transform.position;
        Assert.Greater(newPosition.x, initialPosition.x, "Player moves up.");
    }

    public IEnumerator PlayerMovesDown()
    {
        PlayerStats.instance.MoveSpeed = 10f;

        // Simulate movement
        Vector3 initialPosition = player.transform.position;
        playerMove.MoveDown();

        // Allow some time for movement
        yield return new WaitForSeconds(0.1f);

        // Assert that the player has moved down
        Vector3 newPosition = player.transform.position;
        Assert.Greater(newPosition.x, initialPosition.x, "Player moves down.");
    }

    [Test]
    public void PlayerHealthDoesNotGoBelowZero()
    {
        // Arrange
        PlayerStats.instance.CurrentHealth = 10; // Set current health close to zero
        int damageAmount = 15;

        // Act
        PlayerStats.instance.TakeDamage(damageAmount);

        // Assert
        Assert.AreEqual(0, PlayerStats.instance.CurrentHealth, "Player's health should not go below zero.");
    }



}
