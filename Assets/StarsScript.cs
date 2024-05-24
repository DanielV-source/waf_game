using UnityEngine;

public class StarsScript : MonoBehaviour
{
    private AudioSource animationSoundStars;
    [SerializeField] private SpriteRenderer enemy;
    [SerializeField] private SpriteRenderer shadow;

    // Start is called before the first frame update
    void Start()
    {
        animationSoundStars = GetComponent<AudioSource>();
    }

    private void StarsAppearSound()
    {
        animationSoundStars.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (animationSoundStars != null && animationSoundStars.time > 0.5f)
        {
            enemy.enabled = true;
            shadow.enabled = true;
        }
    }
}
