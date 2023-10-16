using Godot;
using System;
using System.Collections.Generic;

public partial class StatusEffects : Node
{
    public string status;
    public double timer;
    public bool applied;
    public Effect effect;

    float nextTime = 0;
    bool flick;
   

    public StatusEffects(string _status) 
    { 
        status = _status;
        
        Callable callable = new Callable(this, status);
        callable.Call();

        applied = false;
    }

    public void ApplyEffect(Effect _effect)
    {
        effect = _effect;
        timer = effect.duration;
        applied = true;
    }

    public struct Effect
    {
        public float value;
        public bool debuff;
        public int duration;
    }

    public float ValueChangeTemporary(double delta, ref List<StatusEffects> statuses)
    {
        float strength = effect.value;
        timer -= delta;

        if (timer < 0)
        {
            timer = 0;
            statuses.Remove(this);
            applied = false;
        }

        return strength;
    }

    public float ValueChangeSwitch(float initial,ref List<StatusEffects> statuses)
    {
        if (applied)
            return effect.value;
        else
            return initial;
    }

    public float ValueChangeStagnated(float interval, double delta, ref List<StatusEffects> statuses)
    {
        float strength = effect.value;

        timer -= delta;

        if (timer <= nextTime)
        {
            nextTime -= interval;
            return strength;
        }

        if (nextTime == 0)
        {
            nextTime = effect.duration - interval;
            return strength;
        }

        if (timer < 0)
        {
            timer = 0;
            statuses.Remove(this);
            applied = false;
        }

        return 0;
    }
}
