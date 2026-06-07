using UnityEngine;

public class AgentDevelopmentSystem : MonoBehaviour
{
    public float constructionSkill;
    public float foragingSkill;
    public float explorationSkill;
    public float socialSkill;
    public float cultureScore;

    public float DevelopmentScore
    {
        get { return constructionSkill + foragingSkill + explorationSkill + socialSkill + cultureScore; }
    }

    public void NotifyResourceCollected(string resourceType)
    {
        foragingSkill += resourceType == "EnergyCrystal" ? 1.5f : 1f;
    }

    public void NotifyStructureBuilt(AgentIntentType intentType)
    {
        constructionSkill += intentType == AgentIntentType.BuildCommunity ? 1.5f : 1f;
        if (intentType == AgentIntentType.BuildCommunity)
            cultureScore += 0.5f;
    }

    public void NotifyNewExplorationCell()
    {
        explorationSkill += 0.25f;
    }

    public void NotifySocialContact()
    {
        socialSkill += 0.1f;
    }
}
