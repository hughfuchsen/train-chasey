using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneOptimizerTrig : MonoBehaviour
{

    
    // Start is called before the first frame update

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerCollider"))
        {
            CharacterMovement cm =
                collision.gameObject.GetComponentInParent<CharacterMovement>();

            if (cm != null)
            {
                cm.playerTouchingDoorCol = true;
                Debug.Log("touching");
            }
            
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerCollider"))
        {
            CharacterMovement cm =
                collision.gameObject.GetComponentInParent<CharacterMovement>();

            if (cm != null)
            {
                cm.playerTouchingDoorCol = false;
                Debug.Log("adskfadhsfiudsf");
            }
        }
    }

}
