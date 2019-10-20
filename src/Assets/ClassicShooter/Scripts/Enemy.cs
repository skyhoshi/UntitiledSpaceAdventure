using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Classic shooter Enemy with a hp bar
/// </summary>
public class Enemy : MonoBehaviour {

    public HealthBar healthBar;
    public Transform mainCanvas;
    public Destructable destructable;

    void Start()
    {

        // Attach the healthbar to the current hitpoints 
        destructable = GetComponent<Destructable>();
        SimpleHitpoints hp = destructable.Config as SimpleHitpoints;
        healthBar.SetWatchValue(hp, "Hitpoints", hp.Hitpoints);
        healthBar.transform.SetParent(mainCanvas, true);
    }

    // Event delagate
    public delegate void EnemyDied(Enemy _deceased);

    // You can addd your scripts to trigger code when an enemy dies
    public event EnemyDied EnemyDiedEvent;

    void OnDestroy()
    {
        // Destroy the healthbar, we don't want this to clog up the 3d canvas
        if (healthBar != null)
            Destroy(healthBar.gameObject);

        // Trigger events listening to this object being killed.
        if (EnemyDiedEvent != null)
            EnemyDiedEvent(this);
    }
}
