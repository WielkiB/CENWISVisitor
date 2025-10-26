using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EqManager : MonoBehaviour
{
    public struct EqValuesStruct
    {
        public PartManager.PartCategory category;
        public string name;
        public Sprite sprite;
        public int condition;
        public int model;
    }

    public Transform eqParent;
    public GameObject eqElementPrefab;
    public TextMeshProUGUI categoryText;
    public GameObject categoriesButtons;
    public bool catalogShowing = false;

    private List<EqValuesStruct> eqValues = new List<EqValuesStruct>();
    private List<GameObject> eqElements = new List<GameObject>();
    private PartManager currentElementManager;

    // Zmienna na potrzeby katalogu podzespo³ów
    private PartManager.PartCategory currentCategory = PartManager.PartCategory.CPU;

    public void ShowCatalog()
    {
        if (eqValues.Count == 0)
            return;

        Time.timeScale = 0f;

        catalogShowing = true;
        categoriesButtons.SetActive(true);

        ShowEQ(currentCategory, null, true);
    }

    public void PreviousCategory()
    {
        if (currentCategory == PartManager.PartCategory.CaseLeft)
            currentCategory = PartManager.PartCategory.CableMobo;
        else
            currentCategory--;

        ShowEQ(currentCategory, null, true);
    }

    public void NextCategory()
    {
        if (currentCategory == PartManager.PartCategory.CableMobo)
            currentCategory = PartManager.PartCategory.CaseLeft;
        else
            currentCategory++;

        ShowEQ(currentCategory, null, true);
    }

    public void HideEQ()
    {
        Time.timeScale = 1f;

        catalogShowing = false;
        gameObject.SetActive(false);
        categoriesButtons.SetActive(false);
    }

    public void AddToEQ(PartManager.PartCategory partCategory, string partName, Sprite partSprite, int partCondition, int partModel = 0)
    {
        EqValuesStruct eqValuesStruct = new();
        eqValuesStruct.category = partCategory;
        eqValuesStruct.name = partName;
        eqValuesStruct.sprite = partSprite;
        eqValuesStruct.condition = partCondition;
        eqValuesStruct.model = partModel;

        eqValues.Add(eqValuesStruct);
    }

    public void ShowEQ(PartManager.PartCategory partCategory, PartManager addingPart, bool showAllCatalog = false)
    {
        // Ustawianie odpowiedniej nazwy kategorii
        switch (partCategory)
        {
            case PartManager.PartCategory.CaseLeft:     categoryText.text = "Lewe panele boczne";   break;
            case PartManager.PartCategory.CaseRight:    categoryText.text = "Prawe panele boczne";  break;
            case PartManager.PartCategory.CPU:          categoryText.text = "Procesory";            break;
            case PartManager.PartCategory.RAM:          categoryText.text = "Pamiêci RAM";          break;
            case PartManager.PartCategory.Fan:          categoryText.text = "Wentylatory";          break;
            case PartManager.PartCategory.Cooler:       categoryText.text = "Ch³odzenia procesora"; break;
            case PartManager.PartCategory.GPU:          categoryText.text = "Karty graficzne";      break;
            case PartManager.PartCategory.PSU:          categoryText.text = "Zasilacze";            break;
            case PartManager.PartCategory.Drive:        categoryText.text = "Urz¹dzenia SSD";       break;
            case PartManager.PartCategory.Motherboard:  categoryText.text = "P³yty g³ówne";         break;
            case PartManager.PartCategory.M2Heatsink:   categoryText.text = "Radiatory M.2";        break;
            case PartManager.PartCategory.CableCPU:   categoryText.text = "Przewody CPU 8-pin";        break;
            case PartManager.PartCategory.CableGPU:   categoryText.text = "Przewody GPU 8-pin";        break;
            case PartManager.PartCategory.CableMobo:   categoryText.text = "Przewody ATX 24-pin";        break;
        }

        // Usuwanie dotychczasowej zawartoœci EQ
        foreach (var obj in eqElements)
            Destroy(obj);

        // Wype³nianie EQ
        foreach (var eqValue in eqValues)
        {
            // Dodawanie do EQ tylko mo¿liwych do zamontowania w danym miejscu obiektów
            if (eqValue.category == partCategory)
            {
                GameObject newElement = Instantiate(eqElementPrefab, eqParent);
                eqElements.Add(newElement);

                EqElement eqElement = newElement.GetComponent<EqElement>();
                eqElement.manager = this;
                eqElement.valuesStruct = eqValue;
                eqElement.selectButton.SetActive(!showAllCatalog);
                eqElement.RefreshUI();

                currentElementManager = addingPart;
            }

            gameObject.SetActive(true);
        }
    }

    public void MountElement(EqValuesStruct elementStruct)
    {
        currentElementManager.MountPart(elementStruct.condition);

        eqValues.Remove(elementStruct);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PreviousCategory();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            NextCategory();
    }
}
