using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Card Effect/DamageEffect")]
public class DamageEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        if (target == null)
        {
            Debug.LogWarning("Null target");
            return;
        }

        switch (targetType)
        {
            case EffectTargetType.Target:
                var damage = (int)math.round(value*from.baseStrength);
                target.TakeDamage(damage);
                Debug.Log($"执行了{damage}伤害");
                break;
            case EffectTargetType.All:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().TakeDamage(value);
                }
                break;
        }
    }
}