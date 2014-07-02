using System.Collections.Generic;

public abstract class HasStats {
    public void ApplyBuff(StatBuff buff) {
        StatByName(buff.name).ApplyBuff(buff);
    }

    public void ApplyBuffs(List<StatBuff> buffs) {
        foreach (StatBuff buff in buffs) {
            ApplyBuff(buff);
        }
    }

    public void RemoveBuff(StatBuff buff) {
        StatByName(buff.name).RemoveBuff(buff);
    }

    public void RemoveBuffs(List<StatBuff> buffs) {
        foreach (StatBuff buff in buffs) {
            RemoveBuff(buff);
        }
    }

    public abstract Stat StatByName(string name);
}
