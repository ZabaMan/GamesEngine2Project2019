using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
struct Scene
{
    public int sceneNumber;
    public float time;
}

[System.Serializable]
struct Dialogue
{
    public AudioSource audio;
    public float secondsIn;
}


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader SL = null;              //Static instance of GameManager which allows it to be accessed by any other script. 
    [SerializeField] private List<Scene> sceneList = new List<Scene>();
    private int currentScene = 0;
    [SerializeField] private List<Dialogue> dialogue = new List<Dialogue>();
    private int currentDialogue = 0;


    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (SL == null)

            //if not, set instance to this
            SL = this;

        //If instance already exists and it's not this:
        else if (SL != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        StartCoroutine("sceneLength", sceneList[currentScene].time);
    }



    private void playScene(int scene)
    {
        SceneManager.LoadScene(scene);
        currentScene = scene;

        if (currentScene+1 != sceneList.Count)
        StartCoroutine("sceneLength", sceneList[currentScene].time);
    }

    private IEnumerator sceneLength(float t)
    {
        yield return new WaitForSeconds(t);
        playScene(sceneList[currentScene + 1].sceneNumber);
    }

    private void Update()
    {
        print(Time.time);
        if (currentDialogue != dialogue.Count && Time.time > dialogue[currentDialogue].secondsIn)
        {
            dialogue[currentDialogue].audio.Play();
            currentDialogue++;
        }
    }
}
