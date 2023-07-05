using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    public class Group
    {
        public GameObject target;
        public List<HumanAI> agents = new List<HumanAI>();
        public FormationManager.FormationType formation;

        public Group(GameObject target, FormationManager.FormationType formation)
        {
            this.target = target;
            this.formation = formation;
        }
    }

    private List<Group> groups = new List<Group>();

    public void RegisterAgent(HumanAI agent, GameObject target, FormationManager.FormationType formation)
    {
        Group group = groups.Find(g => g.target == target);
        if (group == null)
        {
            group = new Group(target, formation);
            groups.Add(group);
        }
        group.agents.Add(agent);
        UpdateFormation(group);
    }

    public void UnregisterAgent(HumanAI agent)
    {
        foreach (var group in groups)
        {
            group.agents.Remove(agent);
        }
        groups.RemoveAll(g => g.agents.Count == 0);
    }

    public Group GetGroupForAgent(HumanAI agent)
    {
        foreach (var group in groups)
        {
            if (group.agents.Contains(agent))
            {
                return group;
            }
        }
        return null;
    }

    private void UpdateFormation(Group group)
    {
        List<Vector3> positions = FormationManager.GetPositions(group.target.transform.position, group.formation, group.agents.Count, 1.0f);
        for (int i = 0; i < group.agents.Count; i++)
        {
            group.agents[i].SetTargetPosition(positions[i]);
        }
    }
}

