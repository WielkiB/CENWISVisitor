using System;
using UnityEngine;
using System.Linq;

public class PartManager : MonoBehaviour
{
    [Serializable]
    public struct PartAppearance
    {
        public Mesh mesh;
        public MeshRenderer meshRenderer;
        public Material[] materials;
    }

    public enum PartCategory
    {
        Screw, CaseLeft, CaseRight, CPU, RAM, Fan, Cooler, GPU, PSU, Drive, Motherboard, M2Heatsink, CableCPU, CableGPU, CableMobo
    }

    public PartCategory partCategory;
    public string partName;
    public PcManager pcManager;
    public EqManager eqManager;
    public CollidedParts collidedObject;
    public CollidedParts collidedObjectB;   // Wy��cznie na potrzeby obs�ugi drugiego ko�ca przewodu
    public MeshRenderer meshRenderer;
    public Material transparentMat;
    public bool isMounted = true;
    public bool drawCondition = true;
    public bool replaceInMission = false;
    public GameObject childToDisable;
    public GameObject vfxObject;            // Obiekt dodatkowych efekt�w wizualnych, kt�re maj� by� widoczne na mocno uszkodzonej cz�ci

    private int mountedPartLayer;
    private int dismountedPartLayer;
    private Material[] defaultMats;

    public bool isDamageable = false;
    public int partCondition = 100;

    [SerializeField] private PartAppearance normalAppearance;
    [SerializeField] private PartAppearance lightDamagedAppearance;
    [SerializeField] private PartAppearance mediumDamagedAppearance;
    [SerializeField] private PartAppearance heavyDamagedAppearance;

    [HideInInspector] public Sprite currentSprite;

    [SerializeField] private Sprite nonDamagedSprite;
    [SerializeField] private Sprite lightDamagedSprite;
    [SerializeField] private Sprite mediumDamagedSprite;
    [SerializeField] private Sprite heavyDamagedSprite;

    private void ApplyProperlyAppearance()
    {
        meshRenderer.materials = normalAppearance.materials;

        if (vfxObject)
            vfxObject.SetActive(false);

        if (partCondition >= 90)
        {
            if (normalAppearance.mesh)
                meshRenderer.GetComponent<MeshFilter>().mesh = normalAppearance.mesh;

            currentSprite = nonDamagedSprite;
        }
        else if (partCondition >= 50)
        {
            if (lightDamagedAppearance.meshRenderer)
                meshRenderer.materials = lightDamagedAppearance.meshRenderer.sharedMaterials;

            if (lightDamagedAppearance.mesh)
                meshRenderer.GetComponent<MeshFilter>().mesh = lightDamagedAppearance.mesh;

            currentSprite = lightDamagedSprite;
        }
        else if (partCondition >= 20)
        {
            if (mediumDamagedAppearance.meshRenderer)
                meshRenderer.materials = mediumDamagedAppearance.meshRenderer.sharedMaterials;

            if (mediumDamagedAppearance.mesh)
                meshRenderer.GetComponent<MeshFilter>().mesh = mediumDamagedAppearance.mesh;

            currentSprite = mediumDamagedSprite;
        }
        else
        {
            if (heavyDamagedAppearance.meshRenderer)
                meshRenderer.materials = heavyDamagedAppearance.meshRenderer.sharedMaterials;

            if (heavyDamagedAppearance.mesh)
                meshRenderer.GetComponent<MeshFilter>().mesh = heavyDamagedAppearance.mesh;

            currentSprite = heavyDamagedSprite;

            if (vfxObject)
                vfxObject.SetActive(true);
        }

        defaultMats = meshRenderer.materials;

        GetComponent<Outline>().RefreshMaterial();
    }

    private void DemountPart()
    {
        if (collidedObject)
            collidedObject.RemoveFromList(gameObject);

        if (collidedObjectB)
            collidedObjectB.RemoveFromList(gameObject);

        meshRenderer.enabled = false;

        Material[] transparentMats = Enumerable.Repeat(transparentMat, meshRenderer.materials.Length).ToArray();
        meshRenderer.materials = transparentMats;

        gameObject.layer = dismountedPartLayer;
        isMounted = false;

        if (childToDisable)
            childToDisable.SetActive(false);

        if (vfxObject)
            vfxObject.SetActive(false);
    }

    private void MountPart()
    {
        if (collidedObject)
            collidedObject.AddToList(gameObject);

        if (collidedObjectB)
            collidedObjectB.AddToList(gameObject);

        meshRenderer.materials = defaultMats;
        gameObject.layer = mountedPartLayer;
        meshRenderer.enabled = true;
        isMounted = true;

        if (childToDisable)
            childToDisable.SetActive(true);

        GetComponentInParent<SwitchPcLayers>().CheckAllMounted();

        pcManager.UpdateMissionCondition();
    }

    public void MountPart(int condition)
    {
        if (collidedObject)
            collidedObject.AddToList(gameObject);

        if (collidedObjectB)
            collidedObjectB.AddToList(gameObject);

        partCondition = condition;
        meshRenderer.materials = defaultMats;

        ApplyProperlyAppearance();

        gameObject.layer = mountedPartLayer;
        meshRenderer.enabled = true;
        isMounted = true;

        if (childToDisable)
            childToDisable.SetActive(true);

        GetComponentInParent<SwitchPcLayers>().CheckAllMounted();

        pcManager.UpdateMissionCondition();
    }

    private void Awake()
    {
        if (isDamageable)
        {
            normalAppearance.meshRenderer = meshRenderer;
            normalAppearance.materials = meshRenderer.materials;
            normalAppearance.mesh = meshRenderer.GetComponent<MeshFilter>().mesh;

            // Losowanie stanu cz�ci
            if (drawCondition)
                partCondition = UnityEngine.Random.Range(pcManager.minCondiiton, pcManager.maxCondiiton);

            ApplyProperlyAppearance();

            // Ka�da cz�� "dodaje siebie" na podmian� do katalogu, je�li jest tryb swobodny lub cz�� ma by� zmieniona w ramach danego zadania
            if (replaceInMission || !pcManager.missionManager)
                eqManager.AddToEQ(partCategory, partName, nonDamagedSprite, 100);
        }
        // Zapisanie domy�lnego wygl�du dla cz�ci nieuszkadzaj�cych si� (jak �rubki)
        else
        {
            defaultMats = meshRenderer.materials;
        }
    }

    private void Start()
    {
        mountedPartLayer = LayerMask.NameToLayer("PcPartMounted");
        dismountedPartLayer = LayerMask.NameToLayer("PcPartDismounted");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Demonta� podzespo�u
            if (isMounted)
            {
                // Specjalny warunek dla z�amanej pami�ci RAM
                if (partCategory == PartCategory.RAM && partCondition < 20)
                    partName = "GSkill DDR4 4.47GB";

                if (partCategory != PartCategory.Screw)
                    eqManager.AddToEQ(partCategory, partName, currentSprite, partCondition);

                DemountPart();
            }
            // Monta� podzespo�u
            else
            {
                if (partCategory != PartCategory.Screw)
                    eqManager.ShowEQ(partCategory, this);
                else
                    MountPart();
            }
        }
    }

    public void ChangeMeshVisibility(bool newState)
    {
        // Wykluczenie wy��czania mesha dla obiekt�w, kt�rych w danej chwili nie da si� zamontowa� bo nie maj� nadrz�dnego
        if (newState)
        {
            if (collidedObject && !collidedObject.GetComponent<PartManager>().isMounted)
                return;

            if (collidedObjectB && !collidedObjectB.GetComponent<PartManager>().isMounted)
                return;
        }

        meshRenderer.enabled = newState;
    }

    public void SetOutline(Color color, bool enableManager)
    {
        bool collidedObjectMounted = collidedObject == null ||
                             (collidedObject.GetComponent<PartManager>() != null &&
                              collidedObject.GetComponent<PartManager>().isMounted);

        bool collidedObjectBMounted = collidedObjectB == null ||
                                      (collidedObjectB.GetComponent<PartManager>() != null &&
                                       collidedObjectB.GetComponent<PartManager>().isMounted);

        // W��czanie outline tylko wtedy, gdy zamontowany jest jego obiekt nadrz�dny
        if (collidedObjectMounted && collidedObjectBMounted)
        {
            if (enableManager)
            {
                enabled = enableManager;
                GetComponent<Outline>().PlacementMode = Outline.Placement.Top;
            }
            else
            {
                GetComponent<Outline>().PlacementMode = Outline.Placement.Bottom;
            }

            GetComponent<Outline>().enabled = true;
            GetComponent<Outline>().OutlineColor = color;
        }
    }

    public void DisableOutline()
    {
        enabled = false;
        GetComponent<Outline>().enabled = false;
    }
}
