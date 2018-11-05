using UnityEngine;

public class CitiesBoundaries : MonoBehaviour {

    public ArcadeManager arcadeManager;
    public A_WordManager wordManager;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            AudioController.Instance.PlayFX("Explosion");
                        
            if (wordManager.hasActiveWord)
            {
                wordManager.hasActiveWord = false;
                wordManager.words.Remove(wordManager.activeWord);
            }
            else
            {
                wordManager.words.RemoveAt(wordManager.words.FindIndex(word => true));
            }

            Destroy(other.gameObject);
            arcadeManager.playerLife -= 1;
        }
    }
}
