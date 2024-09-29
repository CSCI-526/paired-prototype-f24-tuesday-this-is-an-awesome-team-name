using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public IEnumerator Die()
    {
        UIManager.Instance.loseText.SetActive(true);
        yield return new WaitForSeconds(3f);
        Restart();
    }
}
