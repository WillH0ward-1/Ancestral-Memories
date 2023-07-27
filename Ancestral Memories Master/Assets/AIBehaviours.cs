using System.Collections;
using ProceduralModeling;
using UnityEngine;

public class AIBehaviours : MonoBehaviour
{
    // Declare your AI variables and settings here

    // Define the delegate for validation and action
    public delegate bool ValidateObject(GameObject obj);
    public delegate void ActOnObject(GameObject obj);

    private MapObjGen mapObjGen;

    private void Awake()
    {
        mapObjGen = FindObjectOfType<MapObjGen>();
    }

    public bool ValidateTree(GameObject tree)
    {
        PTGrowing ptGrowing = tree.GetComponentInChildren<PTGrowing>();
        if (ptGrowing != null && !ptGrowing.isDead && ptGrowing.isFullyGrown)
        {
            Debug.Log("Valid tree found: " + tree.name);
            return true;
        }
        return false;
    }

    public bool ValidateFruit(GameObject fruit)
    {
        FoodAttributes foodAttributes = fruit.GetComponentInChildren<FoodAttributes>();
        return foodAttributes != null && !foodAttributes.isDead;
    }

    public bool ValidateAnimal(GameObject animal)
    {
        AICharacterStats animalAI = animal.GetComponentInChildren<AICharacterStats>();
        return animalAI != null && !animalAI.isDead;
    }



    // Define other AI behaviour methods here
}
