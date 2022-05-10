using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   FireballCollision.cs                                                                                                             //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Fireball Collision class                                                                                                         //
//                                                                                                                                           //
//  Notes:  Checks for collision with the fireballs the player shoots out of the staff                                                       //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class FireballCollision : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    float damage = 10.0f;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public GameObject hitImpactExplosion;
    public AudioSource audioSource;
    public AudioClip collision1;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // OnCollisionEnter                                                                                                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function called when a fireball collides with an object in the world                                                                                                                                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    void OnCollisionEnter(Collision collision)
    {
        // Check colision point
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        
        // Activate impact explosion animation and then destroy it after a second
        hitImpactExplosion = Instantiate(hitImpactExplosion, pos, rot) as GameObject;
        Destroy(hitImpactExplosion, 1.0f);

        gameObject.transform.position = new Vector3(-100.0f, 0.0f, -100.0f);

        // Play impact sound
        audioSource.PlayOneShot(collision1);

        // Check for collision with an enemy
        if (collision.gameObject.name == "Enemy(Clone)" || collision.gameObject.name == "Enemy 2(Clone)")
        {
            // Reduce enemies health
            EnemyHealth target = collision.transform.GetComponent<EnemyHealth>();
            target.TakeDamage(damage);
        
        }

        // Destroy the fireball
        Destroy(gameObject, 1.0f);


    }

} // End of FireballCollsion class
