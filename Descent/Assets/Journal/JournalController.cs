using System.Collections.Generic;
using UnityEngine;

namespace Journal
{
    public class JournalController : MonoBehaviour
    {
        [Header("Notebook Settings")]
        [Space(10)]
        [SerializeField] private GameObject journalObject;
        [SerializeField] private GameObject flashlightHolder;
        [SerializeField] private Animator journalAnimator;
        [SerializeField] private Animator notebookAnimator;
        [Space]
        [SerializeField] private int maxPage = 1; // 1 = 2 pages, 0 = 1 pages
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private List<AudioClip> pageTurnSounds = new List<AudioClip>();
        [SerializeField] private AudioClip lighterSound;

        private int currentPage = 0;

        private Vector3 localFlashlightDefPos;

        [HideInInspector] public bool isOpen = false;
        private bool isOnTransition = false;

        private void Start()
        {
            localFlashlightDefPos = flashlightHolder.transform.localPosition;
        }

        public void ShowJournal()
        {
            isOnTransition = true;

            journalObject.SetActive(true);
            Invoke(nameof(SetIsOnTransitionFalse), 0.7f);
            Invoke(nameof(PlayLighterSound), 0.435f);
            flashlightHolder.transform.localPosition = new Vector3(flashlightHolder.transform.localPosition.x, -3.29f, flashlightHolder.transform.localPosition.z);
            journalAnimator.SetTrigger("Show");

            isOpen = true;
        }

        public void HideJournal()
        {
            isOnTransition = true;

            Invoke(nameof(SetActiveFalse), 0.5f);
            Invoke(nameof(SetIsOnTransitionFalse), 0.7f);
            journalAnimator.SetTrigger("Hide");

            isOpen = false;
        }

        private void SetActiveFalse() { BackPage(); journalObject.SetActive(false); flashlightHolder.transform.localPosition = localFlashlightDefPos; }
        private void SetIsOnTransitionFalse() { isOnTransition = false; }
        private void SetIsOnTransitionTrue() { isOnTransition = true; }

        private void Update()
        {
            UpdateJournal();
        }

        private void UpdateJournal()
        {
            if (isOnTransition)
                return;

            // Edit by your input system
            // ..
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (isOpen)
                {
                    // Close
                    HideJournal();
                }
                else
                {
                    // Open
                    ShowJournal();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Next Page
                NextPage();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Next Page
                BackPage();
            }
        }

        public void NextPage()
        {
            if (currentPage == maxPage)
                return;

            isOnTransition = true;

            PlayRandomPageTurn();

            currentPage += 1;
            notebookAnimator.SetTrigger("Page2");
            journalAnimator.SetTrigger("OpenPage");
            Invoke(nameof(SetIsOnTransitionFalse), 1.2f);
        }

        public void BackPage()
        {
            if (currentPage == 0)
                return;

            isOnTransition = true;

            PlayRandomPageTurn();

            currentPage -= 1;
            notebookAnimator.SetTrigger("Page1");
            journalAnimator.SetTrigger("OpenPage");
            Invoke(nameof(SetIsOnTransitionFalse), 1.2f);
        }

        private void PlayRandomPageTurn()
        {
            audioPlayer.PlayOneShot(pageTurnSounds[UnityEngine.Random.Range(0, pageTurnSounds.Count - 1)]);
        }

        private void PlayLighterSound() { audioPlayer.PlayOneShot(lighterSound); }
    }
}
