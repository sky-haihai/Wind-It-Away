using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        //FindObjectOfType<HudUI>().Add(0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
