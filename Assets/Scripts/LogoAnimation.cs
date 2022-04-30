using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnimation : MonoBehaviour
{
    private Image img;
    [SerializeField]
    private Sprite[] images;
    private bool fadeIn;
    private bool fadeOut;
    [SerializeField]
    private float fadeSpeed;
    private int imageNum = 0;
    [SerializeField]
    private float fps;
    [SerializeField]
    private GameObject menu;
    private void Start()
    {
        Time.timeScale = 1;
        img = GetComponent<Image>();
        fadeIn = true;
    }
    private void FixedUpdate()
    {
        if (fadeIn)
        {
            img.color = new Color(1,1,1, img.color.a + fadeSpeed*Time.deltaTime);
            if (img.color.a >= 1)
            {
                img.color = new Color(1,1,1,1);
                fadeIn = false;
                StartCoroutine("Animate");
            }
        }
        else if (fadeOut)
        {
            img.color = new Color(1, 1, 1, img.color.a - fadeSpeed * Time.deltaTime);
            if (img.color.a <= 0)
            {
                img.color = new Color(1, 1, 1, 0);
                fadeIn = false;
                menu.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
    IEnumerator Animate()
    {
        yield return new WaitForSeconds(1 / fps);
        img.sprite = images[imageNum];
        imageNum++;
        if (imageNum != images.Length)
            StartCoroutine("Animate");
        else
            fadeOut = true;
    }
}
