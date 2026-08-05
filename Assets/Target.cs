using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    public void TakeDamage(float amount)
    {
        health = health - amount;
        Debug.Log(gameObject.name + "Zərər aldı! Qalan Can: " + health);

        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log(gameObject.name + "Məhv edildi!");
        Destroy(gameObject);
    }
   
}
