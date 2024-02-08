using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator anim;
    public PageManager pageManager;
    
    // Update is called once per frame
   

    public void FadeToLevel()
    {
        anim.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
       
        SceneManager.LoadScene("Win");
    }

    
    public void OnFadeToMenuComplete()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}
