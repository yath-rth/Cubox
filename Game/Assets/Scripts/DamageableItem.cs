using System.Collections;
using UnityEngine;

public class DamageableItem : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] MeshRenderer graphics;
    [SerializeField] GameObject deathEffect;
    [SerializeField] HealthBar healthBar;
    [SerializeField] bool Alive;

    public void UpdateHealth(int health, Color color)
    {
        if (this.health > health) StartCoroutine(TakeDamage(health, color));
        this.health = health;
        CheckObjectHealth();
        if (healthBar != null) if (healthBar.isActiveAndEnabled) healthBar.UpdateHealthBar(this.health);
    }

    public bool GetAlive()
    {
        return Alive;
    }

    IEnumerator TakeDamage(int health, Color color)
    {
        float damageTimer = 0;
        MaterialPropertyBlock mat = new MaterialPropertyBlock();
        graphics.GetPropertyBlock(mat); ;

        while (damageTimer < .25f)
        {
            mat.SetColor("_color", Color.Lerp(color, Color.red, Mathf.PingPong(damageTimer * 2f, 1)));
            damageTimer += Time.deltaTime;
            graphics.SetPropertyBlock(mat);

            yield return null;
        }

        mat.SetColor("_color", color);
        graphics.SetPropertyBlock(mat);

        this.health = health;
    }

    void CheckObjectHealth()
    {
        if (health > 0)
        {
            Alive = true;
        }
        else if (health <= 0)
        {
            Alive = false;
            GameObject obj = Instantiate(deathEffect, transform.position + new Vector3(0, 2f, 0), Quaternion.Euler(0, -transform.rotation.y, 0), null);
            StartCoroutine(obj.GetComponent<DeathEffect>().cleanUp());
        }


        //Regen logic can implement this later kinda good but too much right know, wait this all should happen in server so okay willl have to export or u know convert this to kotlin code cause this should happen in server
        // if (shouldRegen && Alive)
        // {
        //     if (canRegen <= Time.time)
        //     {
        //         canRegen = Time.time + 5f;
        //         temp = 2 / 100 * playerStats.getStat(StatTypes.maxhitpoints);
        //         temp = Mathf.Clamp(temp, 0, playerStats.getStat(StatTypes.maxhitpoints));

        //         playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.hitpoints) + temp);
        //     }
        // }
    }

    void cleanEffect(GameObject obj)
    {
        DestroyImmediate(obj);
    }
}
