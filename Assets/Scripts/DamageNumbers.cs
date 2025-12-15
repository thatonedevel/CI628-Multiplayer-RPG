using System.Collections;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    [SerializeField] private Sprite[] digits;

    [Header("Digit Sprite GameObjects")]
    [SerializeField] private SpriteRenderer unitSpriteRenderer;
    [SerializeField] private SpriteRenderer tensSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private float displayDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unitSpriteRenderer.enabled = false;
        tensSpriteRenderer.enabled = false;
    }

    public void DisplayDamage(int amount)
    {
        string amtAsString = System.Math.Abs(amount).ToString();
        int tensDigit = 0;
        int unitsDigit = 0;

        // if we have more than three digits, truncate heading digits
        if (amtAsString.Length >= 3) 
        {
            amtAsString = amtAsString.Substring(amtAsString.Length - 2);
        }
        unitsDigit = int.Parse(amtAsString.Substring(amtAsString.Length - 1, 1));

        // if we have a tens digit, set it here

        tensDigit = amtAsString.Length == 2 ? int.Parse(amtAsString.Substring(0, 1)) : 0;

        // set the sprite frames
        unitSpriteRenderer.sprite = digits[unitsDigit];
        tensSpriteRenderer.sprite = digits[tensDigit];

        StartCoroutine("ShowDigits");
    }

    private IEnumerator ShowDigits()
    {
        // enable sprite objects
        unitSpriteRenderer.enabled = true;
        tensSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(displayDuration);
        unitSpriteRenderer.enabled = false;
        unitSpriteRenderer.enabled = false;
    }
}
