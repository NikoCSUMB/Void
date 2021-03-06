using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Sword sword;
    public Animator animator;
    Rigidbody2D rb;

    public AudioClip hurt;
    public AudioClip death;
    public AudioClip dash;

    public GameObject deathParticle;

    public float speed = 1;
    public float dashSpeed = 8;
    public float hp = 3;

    float horizontal;
    float vertical;

    bool isDashing;
    bool invuln = false;
   
    
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical"); 
        // transform.position += new Vector3(horizontal, vertical, 0) * Time.deltaTime * speed;

        if (Input.GetKeyDown(KeyCode.Space) && !sword.isSlashing)
        {
            sword.Swing();
            animator.SetTrigger("Swing");
        }
        
        if (!isDashing) rb.velocity = new Vector2(horizontal, vertical) * speed;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            SoundManager.speaker.PlaySound(dash);
            StartCoroutine("Dash");            
        }

    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if ((collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Bullet"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (!invuln)
        {
            StartCoroutine("Flicker");
            SoundManager.speaker.PlaySound(hurt);
            hp--;
            GameManager.master.PlayerHurt();
            if (hp <= 0)
            {
                SoundManager.speaker.PlaySound(death);
                GameObject newParticle = GameObject.Instantiate(deathParticle, transform.position, Quaternion.identity);
                Destroy (newParticle, 1f);
                this.transform.position = new Vector3(-500, -500, -500);
                this.GetComponent<Player>().enabled = false;
            }
        }
    }

    public void Respawn()
    {
        hp = 3;
        StartCoroutine("Flicker");
        this.transform.position = new Vector3(0, -1, 0);
    }

    IEnumerator Dash()
    {
        // Dash forward if idle
        this.GetComponent<TrailRenderer>().enabled = true;
        if (rb.velocity.magnitude == 0) rb.velocity = new Vector2(0, 1);
        rb.velocity = (rb.velocity.normalized * dashSpeed);
        yield return new WaitForSeconds(0.5f);
        isDashing = false;
        this.GetComponent<TrailRenderer>().enabled = false;
    }

    IEnumerator Flicker()
    {
        invuln = true;
        Renderer model = this.transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        for (int i = 0; i < 7; i++)
        {
            model.enabled = false;
            yield return new WaitForSeconds(0.1f);
            model.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
       
        invuln = false;
    }
}
