using Godot;

public partial class EntityActionSystem : Node
{
    [Export] EntityData entityData;

    //ad here the entity movment system instead
    [Export] EQMS EntityQMS;

    StatusEffects SpeedStatus = new StatusEffects("Speed");
    StatusEffects DammageStatus = new StatusEffects("Dammage");
    StatusEffects HealthStatus = new StatusEffects("Health");
    StatusEffects ShieldStatus = new StatusEffects("Shield");

    StatusEffects BleedStatusTake = new StatusEffects("BleedTake");
    StatusEffects PoisonStatusTake = new StatusEffects("PoisonTake");
    StatusEffects BleedStatusInflict = new StatusEffects("BleedInflict");
    StatusEffects PoisonStatusInflict = new StatusEffects("PoisonInflict");

    public void act_DammagePlayer(int dammage)
    {
        Mathf.Clamp(entityData.hp, 0, 100);

        if (entityData.hp - dammage! < 0)
            entityData.hp -= dammage;
        else
            entityData.hp = 0;
    }
    public void act_HealPlayer(int ammount)
    {
        Mathf.Clamp(entityData.hp, 0, 100);

        if (entityData.hp + ammount! > 100)
            entityData.hp += ammount;
        else
            entityData.hp = 100;
    }

    void ApplySpeed(bool apply, bool debuff, float value, int duration, double delta)
    {
        StatusEffects status = SpeedStatus;
        entityData.SpaceData = EntityQMS.get_EntityData();

        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                entityData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref entityData.Debuffs);
            else
                entityData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref entityData.Buffs);

            EntityQMS.set_EntityData(entityData.SpaceData);
        }
        else
            EntityQMS.set_defaults();
    }
    void ApplyHealth(bool apply, bool debuff, float value, int duration, double delta)
        {
            StatusEffects status = HealthStatus;

            if (apply)
            {
                if (debuff  && !entityData.Debuffs.Contains(status))
                    entityData.Debuffs.Add(status);
                else if (!entityData.Buffs.Contains(status))
                    entityData.Buffs.Add(status);

                status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
            }

            if (status.applied)
            {
                if (debuff)
                    entityData.maxHP = (int)status.ValueChangeTemporary(delta, ref entityData.Debuffs);
                else
                    entityData.maxHP = (int)status.ValueChangeTemporary(delta, ref entityData.Buffs);
            }
        }
    void ApplyShield(bool apply, bool debuff, float value, int duration, double delta)
    {
        StatusEffects status = ShieldStatus;

        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                entityData.shield = (int)status.ValueChangeTemporary(delta, ref entityData.Debuffs);
            else
                entityData.shield = (int)status.ValueChangeTemporary(delta, ref entityData.Buffs);
        }
    }
    void ApplyDammageChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = DammageStatus;

        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            entityData.dammage = (int)status.ValueChangeSwitch(1.0f, ref entityData.Debuffs);
        else
            entityData.dammage = (int)status.ValueChangeSwitch(1.0f, ref entityData.Buffs);
    }
    void ApplyHealthChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = HealthStatus;

        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            entityData.maxHP = (int)status.ValueChangeSwitch(1.0f, ref entityData.Debuffs);
        else
            entityData.maxHP = (int)status.ValueChangeSwitch(1.0f, ref entityData.Buffs);
    }
    void ApplyShieldChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = ShieldStatus;

        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            entityData.shield = (int)status.ValueChangeSwitch(1.0f, ref entityData.Debuffs);
        else
            entityData.shield = (int)status.ValueChangeSwitch(1.0f, ref entityData.Buffs);
    }
    void ApplyBleedTake(bool apply, bool debuff, float value, float interval, int duration, double delta)
    {
        StatusEffects status = BleedStatusTake;
        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                entityData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref entityData.Debuffs);
            else
                entityData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref entityData.Buffs);
        }
    }
    void ApplyPoisonTake(bool apply, bool debuff, float value, float interval, float moveSpeed, int duration, double delta)
    {
        StatusEffects status = PoisonStatusTake;
        if (apply)
        {
            if (debuff && !entityData.Debuffs.Contains(status))
                entityData.Debuffs.Add(status);
            else if (!entityData.Buffs.Contains(status))
                entityData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
            {
                entityData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref entityData.Debuffs);

                status.effect.value = moveSpeed;
                entityData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref entityData.Debuffs);
            }
            else
            {
                entityData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref entityData.Buffs);

                status.effect.value = moveSpeed;
                entityData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref entityData.Buffs);
            }
        }
    }
}