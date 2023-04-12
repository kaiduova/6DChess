using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField]
    private int testSingleplayerIndex;
    
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void StartSingleplayerGame()
    {
        GameManager.Instance.ClientSide = Side.Normal;
        GameManager.Instance.GameType = GameType.Singleplayer;
        LoadScene(testSingleplayerIndex);
    }
}