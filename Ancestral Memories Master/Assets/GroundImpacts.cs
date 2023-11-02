using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleSystemPrefab
{
    public string type; // Identifier like "Smoke", "SpecialFX", etc.
    public ParticleSystem prefab;
}

[System.Serializable]
public class ImpactEffectSet
{
    public string name;
    public List<ParticleSystemPrefab> particleSystemPrefabs;
}

// Assuming implementation of ParticleEffectKey
public class ParticleEffectKey
{
    public string EffectSetName;
    public string EffectType;

    public ParticleEffectKey(string setName, string type)
    {
        EffectSetName = setName;
        EffectType = type;
    }

    public override bool Equals(object obj)
    {
        if (obj is ParticleEffectKey other)
        {
            return EffectSetName == other.EffectSetName && EffectType == other.EffectType;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return EffectSetName.GetHashCode() ^ EffectType.GetHashCode();
    }
}

public class ParticleSystemKey : MonoBehaviour
{
    public ParticleEffectKey Key;
}

public class GroundImpacts : MonoBehaviour
{
    [SerializeField] private List<ImpactEffectSet> impactEffectSets;
    private Dictionary<ParticleEffectKey, Queue<ParticleSystem>> particleSystemPool = new Dictionary<ParticleEffectKey, Queue<ParticleSystem>>();

    public void ActivateImpactEffects(string impactName, Vector3 position)
    {
        foreach (var impactSet in impactEffectSets)
        {
            if (impactSet.name == impactName)
            {
                foreach (var psPrefab in impactSet.particleSystemPrefabs)
                {
                    PlayEffect(psPrefab.prefab, impactSet, psPrefab.type, position);
                }
                break;
            }
        }
    }

    private void PlayEffect(ParticleSystem originalEffect, ImpactEffectSet impactSet, string effectType, Vector3 position)
    {
        var key = new ParticleEffectKey(impactSet.name, effectType);
        var effect = GetPooledEffect(key, originalEffect);

        effect.transform.position = position;

        if (effect != null)
        {
            effect.Play();
            StartCoroutine(ReturnToPoolAfterCompletion(effect, effect.main.startLifetime.constantMax));
        }
    }

    private ParticleSystem GetPooledEffect(ParticleEffectKey key, ParticleSystem originalEffect)
    {
        if (!particleSystemPool.ContainsKey(key))
            particleSystemPool[key] = new Queue<ParticleSystem>();

        if (particleSystemPool[key].Count == 0 && originalEffect != null)
        {
            var newEffect = Instantiate(originalEffect, transform);
            newEffect.gameObject.AddComponent<ParticleSystemKey>().Key = key; // Storing the key
            return newEffect;
        }

        return particleSystemPool[key].Dequeue();
    }

    private IEnumerator ReturnToPoolAfterCompletion(ParticleSystem effect, float maxDuration)
    {
        yield return new WaitForSeconds(maxDuration);

        var keyComponent = effect.gameObject.GetComponent<ParticleSystemKey>();
        if (keyComponent != null)
        {
            var key = keyComponent.Key;
            particleSystemPool[key].Enqueue(effect);
        }
        else
        {
            Debug.LogError("ParticleSystemKey component not found on effect GameObject");
        }
    }
}
