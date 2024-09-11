using System;
using Core;
using Creature;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    private Camera _cam;
    private CanvasScaler _cs;
    private Image _image;

    private void Awake()
    {
        _cam = Camera.main;
        _cs = GetComponentInParent<CanvasScaler>();
        _image = GetComponent<Image>();
    }

    // Points to closest enemy to player, if further than 10 units away
    private void Update()
    {
        if (Locator.Player == null) return;
        var playerPos = Locator.Player.transform.position;
        EnemyBase closestEnemy = null;
        var closestDistance = float.MaxValue;
        foreach (var creature in Locator.CreatureManager.creatures)
        {
            if (creature is not EnemyBase enemy || !enemy) continue;
            var distance = Vector3.Distance(playerPos, enemy.transform.position);
            if (distance > closestDistance) continue;
            closestDistance = distance;
            closestEnemy = enemy;
        }

        if (closestEnemy == null) return;
        _image.enabled = closestDistance > 15f;
        var enemyPos = closestEnemy.transform.position;
        var direction = (enemyPos - playerPos).normalized;
        transform.up = -direction;
    }

    private void LateUpdate()
    {
        // Show underneath player
        var screenPoint = _cam.WorldToScreenPoint(Locator.Player.transform.position - (Vector3.up * 0.8f));
        transform.position = screenPoint / _cs.scaleFactor;
    }
}