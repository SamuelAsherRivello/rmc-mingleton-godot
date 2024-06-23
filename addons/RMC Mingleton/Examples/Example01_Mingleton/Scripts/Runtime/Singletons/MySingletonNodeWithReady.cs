using Godot;

namespace RMC.Mingletons.Samples.Example01_Mingleton
{
    /// <summary>
    /// This workflow is for a Singleton that is a Node
    ///
    /// Where you want to add it to the Mingleton in the _Ready method
    /// </summary>
    public partial class MySingletonNodeWithReady : Node
    {
        public override async void _Ready()
        {
            base._Ready();
            await Mingleton.InstantiateAsync();
            Mingleton.Instance.AddSingleton<MySingletonNodeWithReady>(this);
        }
    }
}
