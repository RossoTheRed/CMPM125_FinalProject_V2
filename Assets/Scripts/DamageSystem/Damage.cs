using UnityEngine;

public class Damage 
{
    public int amount;
    public enum Type
    {
        KINETIC
    }
    public Type type;
    public Damage(int amount, Type type)
    {
        this.amount = amount;
        this.type = type;
    }

    public static Type TypeFromString(string type)
    {
        string t = type.ToLower();
        
        return Type.KINETIC;
    }
}
