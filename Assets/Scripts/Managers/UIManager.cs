using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject buttonInCanBuildPrefab;
    public GameObject buttonSkillsPanelPrefab;
    public GameObject Convas;
    
    
    private static Transform canBuild;
    private static GameObject buttonInCanBuildPrefaba;
    private static GameObject _buttonSkillsPanelPrefab;
    private static Transform _buttonOnUnits;
    private static Transform _skillsPanel;
    private static Transform _small0;
    private static Transform _small1;
    private static Transform _small2;

    private static int _skillNumber;
    private void Awake()
    {
        Transform convas = Convas.transform;
        canBuild = convas.Find("BuildingMenu/CanBuild");
        _skillsPanel = convas.Find("SkillsPanel");
        
        _buttonOnUnits = convas.Find("ButtonOnUnits");
        _small0 = _buttonOnUnits.Find("Small0");
        _small1 = _buttonOnUnits.Find("Small1");
        _small2 = _buttonOnUnits.Find("Small2");
        buttonInCanBuildPrefaba = buttonInCanBuildPrefab;
        _buttonSkillsPanelPrefab = buttonSkillsPanelPrefab;
    }
    
    public static Button createTable(SkillData skillData)
    {
        GameObject b = GameObject.Instantiate(buttonInCanBuildPrefaba, canBuild);
        b.GetComponent<Image>().sprite = skillData.sprite;
        return b.GetComponent<Button>(); 
    }
    public static void DestroychildgameObject()
    {
        foreach (Transform child in canBuild)
            Destroy(child.gameObject);
    }
    public static Button createButtonOnUnit(Vector3 poz,SkillData skillData)
    {
        GameObject b = GameObject.Instantiate(buttonInCanBuildPrefaba, _small1);
        b.GetComponent<Image>().sprite = skillData.sprite;
        _buttonOnUnits.position = poz;
        return b.GetComponent<Button>();
    }

    public static Button createButtonOnUnits(Vector3 poz,int count,SkillData skillData)
    {
        _buttonOnUnits.position = poz;
        if (count > 4)
        {
            var grid = _small1.GetComponent<GridLayoutGroup>();
            grid.spacing = new Vector2(100, 0);
        }
        GameObject b;
        switch (_skillNumber)
        {
            case < 2:
                b = GameObject.Instantiate(buttonInCanBuildPrefaba, _small0);
                break;
            case 2:
            case 3:
                b = GameObject.Instantiate(buttonInCanBuildPrefaba, _small1);
                break;
            case 4:
            case 5:
                b = GameObject.Instantiate(buttonInCanBuildPrefaba, _small2);
                break;
            default:
                b = new GameObject();
                break;
        }
        b.GetComponent<Image>().sprite = skillData.sprite;
        return b.GetComponent<Button>();
    }

    public static void DestroyButtonOnUnits()
    {
        foreach (Transform child in _small0)
            Destroy(child.gameObject);
        foreach (Transform child in _small1)
            Destroy(child.gameObject);
        foreach (Transform child in _small2)
            Destroy(child.gameObject);
    }

    public static Button createButtonOnSkillsPanel(SkillData skillData,int num = 0)
    {
        GameObject b;
        if (num == 0)
        {
            b = GameObject.Instantiate(_buttonSkillsPanelPrefab, _skillsPanel.Find($"Image{_skillNumber}"));
            _skillNumber++;
        }
        else
        {
            b = GameObject.Instantiate(_buttonSkillsPanelPrefab, _skillsPanel.Find($"Image{num}"));
        }

        b.GetComponent<Image>().sprite = skillData.sprite;
        return b.GetComponent<Button>();
    }
    public static void DestroyButtonOnSkillsPanel()
    {
        for (int i = 0; i < 6; i++)
        {
            if (_skillsPanel.Find($"Image{i}").childCount != 0)
            {
                Destroy(_skillsPanel.Find($"Image{i}").GetChild(0).gameObject);
            };
        }
        _skillNumber = 0;
    }
    
}
