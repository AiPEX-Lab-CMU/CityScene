using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigation : MonoBehaviour
{

    public static SceneNavigation instance;

    [SerializeField] GameObject citySceneButton;
    [SerializeField] GameObject supermarketSceneButton;

    // Start is called before the first frame update
    void Start()
    {
        //Adding the load scene function signatures to button onclick event
        citySceneButton.GetComponent<Button>().onClick.AddListener(LoadCityScene);
        supermarketSceneButton.GetComponent<Button>().onClick.AddListener(LoadSupermarketScene);

        //activate the appropriate button depending on which scene is active
        SceneManager.sceneLoaded += SceneFinishedLoading;

    }

    
    //Singleton pattern to prevent initializing multiple instances of this class in the same scene
    private void Awake()
    {
        if (instance == null)
        {

            instance = this;

            DontDestroyOnLoad(this.gameObject);

        }
        else
        {

            Destroy(this.gameObject);

        }


    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadCityScene()
    {
        //0 is the index of the city scene in the build settings
        SceneManager.LoadScene(0);

    }


    public void LoadSupermarketScene()
    {
        //1 is the index of the supermarket in the build settings
        SceneManager.LoadScene(1);

    }

    void SceneFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log(scene.name + " finished loading");

        if(scene.buildIndex == 0)
        {

            //city was loaded, hence disable the city button and activate the supermarket button

            citySceneButton.SetActive(false);
            supermarketSceneButton.SetActive(true);
        }

        if (scene.buildIndex == 1)
        {

            //supermarket was loaded, hence disable the supermarket button and activate the city button

            supermarketSceneButton.SetActive(false);
            citySceneButton.SetActive(true);
        }

    }


    

}
