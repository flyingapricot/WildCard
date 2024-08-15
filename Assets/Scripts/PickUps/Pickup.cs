using UnityEngine;

public class Pickup : MonoBehaviour
{
    protected SoulsManager currency;
    protected PlayerStats target; // The object (i.e. the player) that the pickup will move towards.
    [SerializeField] private AudioClip soundEffect; // Assign respective sound effects
    public float lifespan = 0.5f; // How long the pickup will try to move towards the player before being automatically collected.
    protected float speed; // How much distance the pickup will cover in 1 second.

    [Header("Bonuses")]
    public bool instantLevel = false;
    public int minExperience = 0; // Minimum experience value
    public int maxExperience = 0; // Maximum experience value
    public int heal = 0;
    public int souls = 0;

    #region Bobbing Animation
    protected Vector2 initialPosition;
    protected float initialOffset;

    // To represent the bobbing animation of the object.
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new()
    {
        frequency = 2f, direction = new Vector2(0,0.3f)
    };
    #endregion

    protected virtual void Start()
    {
        currency = FindObjectOfType<SoulsManager>();
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if(target)
        {
            // Move it towards the player and check the distance between.
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += speed * Time.deltaTime * (Vector3)distance.normalized;
            else
                Destroy(gameObject);
        }
        else
        {
            // Handle the animation of the object and randomise the starting frame.
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            // This is a useful failsafe, as it prevents gems from flying off out into the borders of the map and becoming a memory leak.
            if (lifespan > 0) this.lifespan = lifespan; 
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
        target.PlayAudio(soundEffect); // Let player cue the sound

        if (instantLevel) // Instantly levels up without increasing level
        {
            GameManager.instance.StartLevelUp();
        }
        else if (minExperience != 0 && maxExperience != 0) 
        {
            target.IncreaseExperience(Random.Range(minExperience, maxExperience + 1) * target.Actual.growth);
        }

        // No need instant healing since its already capped at max health
        if (heal != 0) 
        {
            target.Heal(heal); 
        }

        if (souls != 0) 
        {
            currency.AddSouls((int)(souls * target.Actual.greed));
        }
    }
}