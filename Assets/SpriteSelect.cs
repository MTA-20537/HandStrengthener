using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSelect : MonoBehaviour
{
    public Sprite[] sprites;
    [Range(0, 3)]
    public int selectedSprite;

    // Start is called before the first frame update
    void Start()
    {
        spriteSelect(sprites[selectedSprite]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spriteSelect(Sprite MC)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = MC;
    }
}
