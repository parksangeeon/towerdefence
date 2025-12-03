using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSetter : MonoBehaviour
{
    public void startgame(){
        SceneManager.LoadScene("1Stage");
    }

    // Update is called once per frame
    public void turnoffthegame(){
        Application.Quit();
    }
}
