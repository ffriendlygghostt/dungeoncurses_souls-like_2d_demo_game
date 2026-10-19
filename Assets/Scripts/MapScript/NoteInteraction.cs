using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class NoteInteraction : MonoBehaviour
{
    public GameObject ePrompt;
    public GameObject noteCanvas;
    public GameObject noteMaskShine;
    public ParticleSystem yellowFireFly;
    public Light2D light2d;
    private bool noteIsOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        if (ePrompt == null)
        {
            Transform found = transform.Find("[E]");
            if (found != null) ePrompt = found.gameObject;
            Debug.LogWarning("[E] is Null");
        }

        if (noteCanvas == null)
        {
            Transform found = transform.Find("NoteCanvas");
            if (found != null) noteCanvas = found.gameObject;
            Debug.LogWarning("NoteCanvas is Null");
        }

        if (noteMaskShine == null)
        {
            Transform found = transform.Find("NoteMaskShine");
            if (found != null) noteCanvas = found.gameObject;
        }

        yellowFireFly = GetComponentInChildren<ParticleSystem>();
        
        light2d = GetComponentInChildren<Light2D>();

        if (ePrompt != null) ePrompt.SetActive(false);
        if (noteCanvas != null) noteCanvas.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!noteIsOpen)
            {
                OpenNote();
            }
            else
            {
                CloseNote();
            }
        }

        if (noteIsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    void OpenNote()
    {
        if (noteCanvas != null) noteCanvas.SetActive(true);
        if (ePrompt != null) ePrompt.SetActive(false);
        if (noteMaskShine != null) Destroy(noteMaskShine);
        if (light2d != null) Destroy(light2d);
        if (yellowFireFly != null) Destroy(yellowFireFly);
        noteIsOpen = true;
    }

    void CloseNote()
    {
        if (noteCanvas != null) noteCanvas.SetActive(false);
        if (ePrompt != null) ePrompt.SetActive(true);
        noteIsOpen = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (ePrompt != null) ePrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (ePrompt != null) ePrompt.SetActive(false);
            if (noteCanvas != null) noteCanvas.SetActive(false);
            noteIsOpen = false;
        }
    }
}
