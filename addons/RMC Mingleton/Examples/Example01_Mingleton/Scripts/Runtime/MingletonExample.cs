using Godot;

namespace RMC.Mingletons.Samples.Example01_Mingleton
{
    /// <summary>
    /// Demo of <see cref="Mingleton"/>
    /// </summary>
    public partial class MingletonExample : Node
    {
        //  Fields ----------------------------------------
        
        //  Godot Methods ---------------------------------
		
        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// </summary>
        public override async void _Ready()
        {
            //I'd like to remove the need for async, but It's my only solution...
            // * **Without** AutoLoad
            // * **Without** Mingleton sitting in the scene at edit-time
            await Mingleton.InstantiateAsync();

            //
            MySingletonExample();
            
            //
            MySingletonNodeExample();
       
            //
            MySingletonNodeWithReadyExample();

            //
            MySingletonResourceExample();
            
        }


        //  Methods ---------------------------------------

        private void MySingletonExample()
        {
            GDPrintHeader(nameof(MySingleton));
            // ------------------------------------
            
            Mingleton.Instance.AddSingleton<MySingleton>(new MySingleton());

            var hasMySingleton1 = Mingleton.Instance.HasSingleton<MySingleton>();
            GD.Print("hasSingleton1: " + hasMySingleton1);
            
            var mySingleton1 = Mingleton.Instance.GetSingleton<MySingleton>();
            GD.Print("mySingleton1: " + mySingleton1);
                
            Mingleton.Instance.RemoveSingleton<MySingleton>();
            
            var mySingleton2 = Mingleton.Instance.GetSingleton<MySingleton>();
            GD.Print("mySingleton2 is properly empty after removing it so.. : " + mySingleton2);

        }

        private void MySingletonNodeExample()
        {
            GDPrintHeader(nameof(MySingletonNode));
            // ------------------------------------
            
            Mingleton.Instance.GetOrCreateAsNode<MySingletonNode>();

            var mySingletonNode1 = Mingleton.Instance.GetSingleton<MySingletonNode>();
            GD.Print("mySingletonNode1: " + mySingletonNode1.MyCustomField);

        }
        
        private void MySingletonNodeWithReadyExample()
        {
            GDPrintHeader(nameof(MySingletonNodeWithReady));
            // ------------------------------------
            
  
            var hasMySingletonNodeWithReady1 = Mingleton.Instance.HasSingleton<MySingletonNodeWithReady>();
            GD.Print("hasMySingletonNodeWithReady1: " + hasMySingletonNodeWithReady1);
            
            var mySingletonNodeWithReady = Mingleton.Instance.GetSingleton<MySingletonNodeWithReady>();
            GD.Print("mySingletonNodeWithReady: " + mySingletonNodeWithReady);
        }
        
        private void MySingletonResourceExample()
        {
            //You can use long path or short path. Short path must be UNIQUE in the project to work
            
            GDPrintHeader(nameof(MySingletonResource)+ " A");
            // ------------------------------------
            
            string longPathA = "res://addons/RMC Mingleton/Examples/Example01_Mingleton/" +
                           "Scripts/Runtime/Singletons/MySingletonResource_A.tres";
            var mySingletonResourceA = Mingleton.Instance.GetOrCreateAsResource<MySingletonResource>(longPathA);
            GD.Print("mySingletonResourceA.Health: " + mySingletonResourceA.Health);
            
            
            GDPrintHeader(nameof(MySingletonResource) + " B");
            // ------------------------------------
            
            string shortPathB = "MySingletonResource_B.tres";
            var mySingletonResourceB = Mingleton.Instance.GetOrCreateAsResource<MySingletonResource>(shortPathB);
            GD.Print("mySingletonResourceB.Health: " + mySingletonResourceB.Health);
        }

        private void GDPrintHeader(string message)
        {
            GD.Print($"\n");
            GD.Print($"--------------------------------");
            GD.Print($"  -- {message} --  ");
            GD.Print($"--------------------------------");
            GD.Print($"\n");
        }


        //  Event Handlers --------------------------------
    }
}
