using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    private PlayerMovement playerMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (playerMovement !=null)
        {
            playerMovement.OnFootstep(animationEvent);
        }
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        if (playerMovement != null)
        {
            playerMovement.OnLand(animationEvent);
        }
    }

    
}
