using Godot;

[GlobalClass, Tool]
public partial class MySingletonResource : Resource
{
    [Export] 
    public int Health = 10;
    
    [Export] 
    public int Attack = 10;
    
    [Export] 
    public int Damage = 10;
}
