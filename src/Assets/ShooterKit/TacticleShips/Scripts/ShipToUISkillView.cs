using UnityEngine;
using System.Collections;

/// <summary>
/// This takes skills from the Tactical ship and displays them in the skill button it is assigned to
/// </summary>
public class ShipToUISkillView : MonoBehaviour {

    /// <summary>
    /// Player to read skills from
    /// </summary>
    public TacticalPlayer Player;

    /// <summary>
    /// Skillbutton to apply skills to
    /// </summary>
    private SkillButton SkillButton;

    /// <summary>
    /// Skill name of skill to display
    /// </summary>
    public string SKillName;

    void Reset()
    {
        SkillButton = GetComponent<SkillButton>();
    }


    /// <summary>
    /// Stored skill
    /// </summary>
    private Skill mSkill;

    /// <summary>
    /// Finds the skill
    /// </summary>
    void OnEnable()
    {
        Reset();

        for (int i = 0; i < Player.Skills.Length; i++)
        {
            if (Player.Skills[i].SkillName == SKillName)
            {
                mSkill = Player.Skills[i];
                SkillButton.SetSkill(mSkill);
            }
        }
    }


}
