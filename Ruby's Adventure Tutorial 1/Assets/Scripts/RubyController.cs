using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    // ruby variables
    public float speed = 3.0f;
    public int maxHealth = 5;

//projectile 
    public GameObject projectilePrefab;
//particles
    public ParticleSystem HealthParticle;
    public ParticleSystem DamageParticle;
//total score and results
    public int score =0;
    public int scoreAmount;
    public TextMeshProUGUI scoreText;
    public bool gameOver = false;
    public TextMeshProUGUI gameOverText;

//health
    public float timeInvincible = 2.0f;

    int currentHealth;
    public int health { get { return currentHealth; }}
    
    bool isInvincible;
    float invincibleTimer;

//rigid body and animation
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

//audio 
    AudioSource audioSource; 
    public AudioClip playerHit; //player hit
    public AudioClip cogThrowClip; //cog 
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip jambiSound;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        
        currentHealth = maxHealth;
        audioSource= GetComponent<AudioSource>();
    }

//play sound function
public void PlaySound(AudioClip clip)
{
    audioSource.PlayOneShot(clip);
}

    // Update is called once per frame
    //update ruby position
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                //if time runs out
                if (invincibleTimer < 0)
                    isInvincible = false;
            }
//launch cog
         if(Input.GetKeyDown(KeyCode.C))
        {
             Launch();
        }
//talk to npc
if (Input.GetKeyDown(KeyCode.X))
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
    if (hit.collider != null)
    {
        NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
        if (character != null)
        {
            character.DisplayDialog();
            audioSource.PlayOneShot(jambiSound);

        }  
    }
}
}
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

//changes rubys health
    public void ChangeHealth(int amount)
    {

        if (currentHealth == 0){
            gameOver = true;
            gameOverText.text = "You lost! Press R to Restart!";
            audioSource.PlayOneShot(loseSound);
            speed = 0.0f;

                if (Input.GetKey(KeyCode.R))
                {
                    if (gameOver == true)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
                    }
                }
        }

        // if health goes down/negative number
          if (amount < 0)
        {
            //play hit
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(playerHit);

            //if already invincible do nothing
            if (isInvincible)
                return;
            
            //turn invincible
            isInvincible = true;
            invincibleTimer = timeInvincible;
            // turn on damage when timer starts
            ParticleSystem damage = Instantiate(DamageParticle, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        if(amount > 0){
             ParticleSystem health = Instantiate(HealthParticle, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);


        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

public void changeScore(int scoreAmount) 
{
    score = score + scoreAmount;
    scoreText.text = "Fixed Robots: " + score.ToString();

    if (score == 2){
        gameOverText.text = "You win!";
        audioSource.PlayOneShot(winSound);
        gameOver = true;
        speed = 0.0f;
    }

    
}

public void changeSpeedUp()
{
    speed = speed + 1.5f;

}

public void changeSpeedDown(){
    speed = speed - 1.5f;
}

// makes ruby throw the cog
void Launch()
{
    //make cog exist
    GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

    Projectile projectile = projectileObject.GetComponent<Projectile>();
    projectile.Launch(lookDirection, 300);

    animator.SetTrigger("Launch");
    audioSource.PlayOneShot(cogThrowClip);
}

}


