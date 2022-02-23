using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace PTL.UI
{
	public class ModelWindow : MonoBehaviour
	{
        [Header("Header")]
        public GameObject headerObject;
        public TMP_Text title;
        [Header("Content")]
        public GameObject contentObject;
        public bool useHorizontalLayout;
        [Header("Verticle Layout")]
        public GameObject verticalLayoutObject;
        public Image verticalLayoutImage;
        public TMP_Text verticalLayoutText;
        [Header("Horizontal Layout")]
        public GameObject horizontalLayoutObject;
        public Image horizontalLayoutImage;
        public TMP_Text horizontalLayoutText;
        [Header("Footer")]
        public GameObject footerObject;

        public void Initalize(string title, bool useHorizontalLayout, Sprite contentImage, string contentText)
        {
            this.title.text = title;
            this.useHorizontalLayout = useHorizontalLayout;
            verticalLayoutObject.SetActive(!useHorizontalLayout);
            horizontalLayoutObject.SetActive(useHorizontalLayout);
            if (useHorizontalLayout)
            {
                horizontalLayoutImage.sprite = contentImage;
                horizontalLayoutText.text = contentText;
            }
            else
            {
                verticalLayoutImage.sprite = contentImage;
                verticalLayoutText.text = contentText;
            }
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            headerObject.SetActive(title.text != "");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
	}
}