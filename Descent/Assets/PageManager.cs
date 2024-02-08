using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    public static PageManager Instance;

    public List<Pages> pages = new List<Pages>();

    public int pageCount = 5;

   

    public LevelChanger levelChanger;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPage(Pages page)
    {
        pages.Add(page);
    }

    // Method to handle when an objective is complete
    public void PagesComplete(string pageID)
    {
        Pages page = pages.Find(obj => obj.pageID == pageID);
        if (pages != null)
        {
            // Handle any logic related to completing the objective
            Debug.Log($"Objective Completed: {page.description}");
        }
    }

    void Update()
    {
        if (pageCount == 0)
        {
            Debug.Log("all pages collected");
            
            levelChanger.FadeToLevel();
        }
    }
}
