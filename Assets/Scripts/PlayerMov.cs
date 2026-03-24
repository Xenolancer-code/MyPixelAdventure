using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    private bool isPlayerReady;
    private GameObject sndManager;
    private Rigidbody2D rb;
    [SerializeField] private float jumpSpeed = 7f;
    
    
    public void OnJump()
    {
        if (!isPlayerReady) return; //!IsGrounded()
        sndManager.GetComponent<SoundManager>().PlayFX(0);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
    }
}
