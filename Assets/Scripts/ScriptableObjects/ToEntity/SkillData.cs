using UnityEngine;
using UnityEngine.AI;

public enum SkillType
{
    BUILDER,
    UNIT,
    UNIT_BUILD
}

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill", order = 4)]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public SkillType type;
    public Sprite sprite;

    

    public void Trigger(GameObject source, GameObject target = null)
    {
        switch (type)
        {
            // case SkillType.INSTANTIATE_CHARACTER:
            //     {
            //     }
            //     break;
            // case SkillType.INSTANTIATE_BUILDING:
            //     {
            //     }
            //     break;
            // default:
            //     break;
        }
    }
}
