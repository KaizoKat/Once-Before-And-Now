using Godot;

public partial class PlayerActionSystem : Node
{
    [Export] public PlayerData playerData;
    [Export] QMS PlayerQMS;
    [Export] RayCast3D actionRay;

    StatusEffects SpeedStatus = new StatusEffects("Speed");
    StatusEffects LuckStatus = new StatusEffects("Luck");
    StatusEffects DammageStatus = new StatusEffects("Dammage");
    StatusEffects HealthStatus = new StatusEffects("Health");
    StatusEffects ShieldStatus = new StatusEffects("Shield");

    StatusEffects BleedStatusTake = new StatusEffects("BleedTake");
    StatusEffects PoisonStatusTake = new StatusEffects("PoisonTake");
    StatusEffects BleedStatusInflict = new StatusEffects("BleedInflict");
    StatusEffects PoisonStatusInflict = new StatusEffects("PoisonInflict");

    public void act_DammagePlayer(bool apply, int dammage)
    {
        if(apply)
        {
            Mathf.Clamp(playerData.hp, 0, 100);

            if (playerData.hp - dammage! < 0)
                playerData.hp -= dammage;
            else
                playerData.hp = 0;
        }

    }
    public void act_HealPlayer(bool apply, int ammount)
    {
        if(apply)
        {
            Mathf.Clamp(playerData.hp, 0, 100);

            if (playerData.hp + ammount! > 100)
                playerData.hp += ammount;
            else
                playerData.hp = 100;
        }
    }

    public void act_DammageEntity(bool apply, int dammage)
    {
        if (apply && actionRay.IsColliding())
        {
            DataRef dataRef = get_DataRefFromRaycast(actionRay);

            if(dataRef.conEntityData != null)
                dataRef.conEntityData.hp -= dammage;
            else if(dataRef.conPlayerData != null)
                dataRef.conPlayerData.hp -= dammage;
        }
    }
    public void act_CnockBackEntity(bool apply, float ammount)
    {
        if (apply)
        {
            DataRef dRef = get_DataRefFromRaycast(actionRay);
            if (dRef != null)
            {
                Vector3 direction = actionRay.GlobalPosition - dRef.GlobalPosition;
                dRef.conRig.ApplyCentralImpulse(-(direction.Normalized() - Vector3.Up) * ammount);
            }
        }
    }

    public void ApplySpeed(bool apply, bool debuff, float value, int duration, double delta)
    {
        StatusEffects status = SpeedStatus;
        playerData.SpaceData = PlayerQMS.get_PlayerData();

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                playerData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref playerData.Debuffs);
            else
                playerData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref playerData.Buffs);

            PlayerQMS.set_playerData(playerData.SpaceData);
        }
        else
            PlayerQMS.set_defaults();
    }
    public void ApplyLuck(bool apply, bool debuff, float value, int duration, double delta)
    {
        StatusEffects status = LuckStatus;

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                playerData.luck = (int)status.ValueChangeTemporary(delta, ref playerData.Debuffs);
            else
                playerData.luck = (int)status.ValueChangeTemporary(delta, ref playerData.Buffs);
        }
    }
    public void ApplyHealth(bool apply, bool debuff, float value, int duration, double delta)
        {
            StatusEffects status = HealthStatus;

            if (apply)
            {
                if (debuff  && !playerData.Debuffs.Contains(status))
                    playerData.Debuffs.Add(status);
                else if (!playerData.Buffs.Contains(status))
                    playerData.Buffs.Add(status);

                status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
            }

            if (status.applied)
            {
                if (debuff)
                    playerData.maxHP = (int)status.ValueChangeTemporary(delta, ref playerData.Debuffs);
                else
                    playerData.maxHP = (int)status.ValueChangeTemporary(delta, ref playerData.Buffs);
            }
        }
    public void ApplyShield(bool apply, bool debuff, float value, int duration, double delta)
    {
        StatusEffects status = ShieldStatus;

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                playerData.shield = (int)status.ValueChangeTemporary(delta, ref playerData.Debuffs);
            else
                playerData.shield = (int)status.ValueChangeTemporary(delta, ref playerData.Buffs);
        }
    }
    public void ApplyDammageChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = DammageStatus;

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            playerData.dammage = (int)status.ValueChangeSwitch(1.0f, ref playerData.Debuffs);
        else
            playerData.dammage = (int)status.ValueChangeSwitch(1.0f, ref playerData.Buffs);
    }
    public void ApplyHealthChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = HealthStatus;

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            playerData.maxHP = (int)status.ValueChangeSwitch(1.0f, ref playerData.Debuffs);
        else
            playerData.maxHP = (int)status.ValueChangeSwitch(1.0f, ref playerData.Buffs);
    }
    public void ApplyShieldChange(bool apply, bool debuff, float value)
    {
        StatusEffects status = ShieldStatus;

        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = 0 });
        }

        if (debuff)
            playerData.shield = (int)status.ValueChangeSwitch(1.0f, ref playerData.Debuffs);
        else
            playerData.shield = (int)status.ValueChangeSwitch(1.0f, ref playerData.Buffs);
    }
    public void ApplyBleedTake(bool apply, bool debuff, float value, float interval, int duration, double delta)
    {
        StatusEffects status = BleedStatusTake;
        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
                playerData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref playerData.Debuffs);
            else
                playerData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref playerData.Buffs);
        }
    }
    public void ApplyPoisonTake(bool apply, bool debuff, float value, float interval, float moveSpeed, int duration, double delta)
    {
        StatusEffects status = PoisonStatusTake;
        if (apply)
        {
            if (debuff && !playerData.Debuffs.Contains(status))
                playerData.Debuffs.Add(status);
            else if (!playerData.Buffs.Contains(status))
                playerData.Buffs.Add(status);

            status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
        }

        if (status.applied)
        {
            if (debuff)
            {
                playerData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref playerData.Debuffs);

                status.effect.value = moveSpeed;
                playerData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref playerData.Debuffs);
            }
            else
            {
                playerData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref playerData.Buffs);

                status.effect.value = moveSpeed;
                playerData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref playerData.Buffs);
            }
        }
    }

    public void ApplyBleedInflict(bool apply, bool debuff, float value, float interval, int duration, double delta)
    {
        if (actionRay.IsColliding())
        {
            StatusEffects status = BleedStatusTake;

            DataRef dataRef = get_DataRefFromRaycast(actionRay);
            EntityData eData = new EntityData();
            PlayerData pData = new PlayerData();

            if (dataRef.conEntityData != null)
                eData = dataRef.conEntityData;
            else if (dataRef.conPlayerData != null)
                pData = dataRef.conPlayerData;

            if (apply)
            {
                if(eData != null)
                {
                    if (debuff && !eData.Debuffs.Contains(status))
                        eData.Debuffs.Add(status);
                    else if (!eData.Buffs.Contains(status))
                        eData.Buffs.Add(status);
                }
                else if(pData != null)
                {
                    if (debuff && !pData.Debuffs.Contains(status))
                        pData.Debuffs.Add(status);
                    else if (!pData.Buffs.Contains(status))
                        pData.Buffs.Add(status);
                }

                status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
            }

            if (status.applied)
            {
                if (eData != null)
                {
                    if (debuff)
                        eData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref eData.Debuffs);
                    else
                        eData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref eData.Buffs);
                }
                else if(pData != null)
                {
                    if (debuff)
                        pData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref pData.Debuffs);
                    else
                        pData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref pData.Buffs);
                }
            }
        }
    }
    public void ApplyPoisonInflict(bool apply, bool debuff, float value, float interval, float moveSpeed, int duration, double delta)
    {
        if (actionRay.IsColliding())
        {
            StatusEffects status = PoisonStatusTake;

            DataRef dataRef = get_DataRefFromRaycast(actionRay);
            EntityData eData = new EntityData();
            PlayerData pData = new PlayerData();

            if (dataRef.conEntityData != null)
                eData = dataRef.conEntityData;
            else if (dataRef.conPlayerData != null)
                pData = dataRef.conPlayerData;

            if(apply)
            {
                if (eData != null)
                {
                    if (debuff && !eData.Debuffs.Contains(status))
                        eData.Debuffs.Add(status);
                    else if (!eData.Buffs.Contains(status))
                        eData.Buffs.Add(status);
                }
                else if (pData != null)
                {
                    if (debuff && !pData.Debuffs.Contains(status))
                        pData.Debuffs.Add(status);
                    else if (!pData.Buffs.Contains(status))
                        pData.Buffs.Add(status);
                }

                status.ApplyEffect(new StatusEffects.Effect() { debuff = debuff, value = value, duration = duration });
            }

            if (status.applied)
            {
                if (eData != null)
                {
                    if (debuff)
                    {
                        eData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref eData.Debuffs);

                        status.effect.value = moveSpeed;
                        eData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref eData.Debuffs);
                    }
                    else
                    {
                        eData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref eData.Buffs);

                        status.effect.value = moveSpeed;
                        eData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref eData.Buffs);
                    }
                }
                else if (pData != null)
                {
                    if (debuff)
                    {
                        pData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref pData.Debuffs);

                        status.effect.value = moveSpeed;
                        pData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref pData.Debuffs);
                    }
                    else
                    {
                        pData.hp -= (int)status.ValueChangeStagnated(interval, delta, ref pData.Buffs);

                        status.effect.value = moveSpeed;
                        pData.SpaceData._walkSpeed = status.ValueChangeTemporary(delta, ref pData.Buffs);
                    }
                }
            }
        }
    }

    public bool case_InsideOfRange(float max)
    {
        DataRef dRef = get_DataRefFromRaycast(actionRay);

        if (dRef != null)
        {
            if (actionRay.GlobalPosition.DistanceTo(dRef.GlobalPosition) <= max)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    DataRef get_DataRefFromRaycast(RayCast3D ray)
    {
        ray.ForceRaycastUpdate();
        if (ray.IsColliding())
        {
            Node col = (Node)ray.GetCollider();
            if (col != null && col.GetChildOrNull<DataRef>(0) != null)
                return col.GetChild<DataRef>(0);
            else
                return null;
        }
        else
            return null;
    }
}