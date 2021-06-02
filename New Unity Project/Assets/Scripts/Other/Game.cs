using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum PlayerState
{
    walk,
    attack,
    stagger
}
public enum GunType
{
    rapid_fire,
    armor_piercing
}
public enum BotType
{
    Destroyer,
    Stormtropper
}
public class Game : MonoBehaviour
{
    public static Game g;
    public Text label;

    void Awake()
    {
        g = this;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        label.text = "Game Over";
        label.gameObject.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0;
        label.text = "You Win!";
        label.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}