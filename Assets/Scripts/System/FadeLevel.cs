using UnityEngine;

public class FadeLevel : MonoBehaviour {

    public Animator animator;

    public void FadeOut()
    {
        animator.SetTrigger("fadeOut");
    }

    public void FadeIn()
    {
        animator.SetTrigger("fadeIn");
    }
   
}
