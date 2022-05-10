using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   EnemyHealth.cs                                                                                                                   //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Enemy Health class                                                                                                               //
//                                                                                                                                           //
//  Notes:  Deals with enemy death and plays death sounds                                                                                    //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class EnemyHealth : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//


    [SerializeField] float hitPoints = 100.0f;
    bool isDead = false;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioSource audioSource;
    public AudioClip hit1;
    public AudioClip hit2;
    public AudioClip hit3;
    public AudioClip hit4;
    public AudioClip hit5;
    public AudioClip hit6;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // IsDead                                                                                                                                //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to check if an enemy is dead or not                                                                                          //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public bool IsDead()
    {
        return isDead;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // TakeDamage                                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //  Function that is called when the enemy takes damage from the player. A broadcast message is used. hitpoints are reduced and if they  //
    //  have zero hit points, the die function is called                                                                                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void TakeDamage(float damage)
    {
        BroadcastMessage("OnDamageTaken");

        // Reduce hit points
        hitPoints -= damage;

        // Check if dead
        if (hitPoints <= 0)
        {
            Die();

        }

        // play hit sound effect, random between 6 different sound effects
        int ranNum = UnityEngine.Random.Range(1, 6);

        if      (ranNum == 1) audioSource.PlayOneShot(hit1);
        else if (ranNum == 2) audioSource.PlayOneShot(hit2);
        else if (ranNum == 3) audioSource.PlayOneShot(hit3);
        else if (ranNum == 4) audioSource.PlayOneShot(hit4);
        else if (ranNum == 5) audioSource.PlayOneShot(hit5);
        else if (ranNum == 6) audioSource.PlayOneShot(hit6);



    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // Die                                                                                                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function called if an enemy dies                                                                                                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void Die()
    {
        if (isDead)
        {
            return; // Do nothing if already dead
        }

        // Set dead to true
        isDead = true;

        // increase the number of enemies the player has killed by 1
        FindObjectOfType<EnemiesKilled>().IncreaseCurrentEnemiesKilled();

        // play death animations
        GetComponent<Animator>().SetTrigger("Die");
    }

} // End of EnemyHealth Class
