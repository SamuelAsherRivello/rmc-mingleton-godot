using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using RMC.Core.Debug;
using RMC.Core.Utilities;

namespace RMC.Mingletons
{
    public partial class Mingleton : Node, ISingletonLookup, IDisposable    
    {
        //  Settings --------------------------------------------
        public static readonly bool IsLoggerEnabled = false;

        //  Properties ----------------------------------------
        public static Mingleton Instance
        {
            get
            {
                if (__instance == null)
                {
                    Instantiate();
                }
                return __instance;
            }
            private set
            {
                __instance = value;
            }
        }
        
        public static bool IsInstantiated
        {
            get
            {
                return __instance != null;
            }
        }

        
        public SceneTree SceneTree
        {
            get
            {
                return GetTree();
            }
        }

        //  Fields ----------------------------------------
        
        /// <summary>
        /// A semaphore is a synchronization mechanism that can be used to control access
        /// to a shared resource in a concurrent environment. Semaphores are commonly used
        /// in multithreading and multiprocessing contexts to manage concurrent access to
        /// resources such as memory, files, or shared data structures.
        /// </summary>
        private static readonly SemaphoreSlim _initializationSemaphore = new SemaphoreSlim(1, 1);
        //
        private static Mingleton __instance;
        private static TaskCompletionSource<bool> _onReadyFinishedTask;
        private ISingletonLookup _singletonLookup;
        private bool _isCallingOnReadyInternally { get; set; } = false;
        private ILogger _logger;

        public Mingleton()
        {
            _logger = new Logger(IsLoggerEnabled)
            {
                Prefix = "[Mingleton] ",
                Suffix = ""
            };
            _singletonLookup = new SingletonLookup(IsLoggerEnabled);
        }
        
        
        public static void DisposeStatic()
        {
            if (__instance != null)
            {
                __instance.QueueFree();
                __instance.Dispose();
                __instance = null;
            }
        }

        public new void Dispose()
        {
            _singletonLookup.Dispose();
            _singletonLookup = null;
            _logger = null;
            base.Dispose();
        }
        
        //  Godot Methods ---------------------------------
        public override void _Ready()
        {
            if (!_isCallingOnReadyInternally)
            {
                if (Instance != this)
                {
                    _logger.Print($"QueueFree() for {Instance}");
                    Instance.QueueFree();
                    Instance = this;
                }
            }

            _logger.Print($"_Ready() GetTree()= {Instance.GetTree()}");
            _onReadyFinishedTask?.SetResult(true);
        }

        public override void _EnterTree()
        {
            base._EnterTree();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            // Optional: Handle any cleanup here...
        }

        //  Methods --------------------------------------
        public static void Instantiate()
        {
            _ = InstantiateAsync();
        }
        
        public static async Task InstantiateAsync()
        {
            if (__instance == null)
            {
        
                await _initializationSemaphore.WaitAsync();
                try
                {
                    if (__instance == null)
                    {
                        _onReadyFinishedTask = new TaskCompletionSource<bool>();

                        __instance = new Mingleton();
                        __instance.Name = "Mingleton";
                        SetNodeName(__instance, nameof(Mingleton), false);
                        __instance.SetProcess(false);

                        SceneTree tree = Engine.GetMainLoop() as SceneTree;
                        if (tree != null)
                        {
                  
                            __instance._isCallingOnReadyInternally = true;
                            
                            // Wait for SceneTree to be ready
                            // This setup is required since I'd like NOT to have 
                            // To use AutoLoad or have Mingleton in the scene at edit-time
                            tree.Root.CallDeferred("add_child", __instance);
                            await _onReadyFinishedTask.Task;
                            
                            __instance._isCallingOnReadyInternally = false;
                         
                        }
                        else
                        {
                            GD.PrintErr("Failed to get the SceneTree from Engine.GetMainLoop().");
                        }
                    }
                }
                finally
                {
                    _initializationSemaphore.Release();
                }
            }
            else
            {
                await _onReadyFinishedTask.Task;
                __instance._logger.Print("Instantiate()");
            }
        }

        public void AddSingleton<T>(T instance, string key = "") where T : class
        {
            _singletonLookup.AddSingleton(instance, key);
        }

        public bool HasSingleton<T>(string key = "") where T : class
        {
            return _singletonLookup.HasSingleton<T>(key);
        }

        public T GetSingleton<T>(string key = "") where T : class
        {
            return _singletonLookup.GetSingleton<T>(key);
        }

        public void RemoveSingleton<T>(string key = "") where T : class
        {
            _singletonLookup.RemoveSingleton<T>(key);
        }

        public T GetOrCreateAsClass<T>(string key = "") where T : class, new()
        {
            if (typeof(T).IsSubclassOf(typeof(Node)))
            {
                throw new Exception(
                    $"GetOrCreateAsClass<T>() failed. Must call GetOrCreateAsNode<T>() instead for type of {typeof(T)}");
            }

            if (typeof(T).IsSubclassOf(typeof(Resource)))
            {
                throw new Exception(
                    $"GetOrCreateAsClass<T>() failed. Must call GetOrCreateAsResource<T>() instead for type of {typeof(T)}");
            }

            var type = typeof(T);
            if (_singletonLookup.HasSingleton<T>(key))
            {
                return _singletonLookup.GetSingleton<T>(key);
            }
            else
            {
                T newInstance = new T();
                _logger.Print($"GetOrCreateAsClass() Type = '{type.Name}', Key = '{key}'.");
                _singletonLookup.AddSingleton(newInstance, key);
                return newInstance;
            }
        }

        public T GetOrCreateAsNode<T>(string key = "") where T : Node, new()
        {
            if (!typeof(T).IsSubclassOf(typeof(Node)))
            {
                throw new Exception(
                    $"GetOrCreateAsNode<T>() failed for type of {typeof(T)}. Only Node types are allowed.");
            }

            var type = typeof(T);
            if (_singletonLookup.HasSingleton<T>(key))
            {
                return _singletonLookup.GetSingleton<T>(key);
            }
            else
            {
                // Add it to the scene tree
                T newInstance = new T();
                Instance.AddChild(newInstance as Node);
                
                SetNodeName(newInstance as Node, newInstance.GetType().Name, true);
                
                _singletonLookup.AddSingleton(newInstance, key);
                return newInstance;
            }
        }

        private static void SetNodeName(Node node, string nodeName, bool isChild)
        {
            if (isChild)
            {
                //Child of Mingleton
                node.Name = $"{nodeName} (Added)"; 
            }
            else
            {
                //Mingleton
                node.Name = $"{nodeName} ({node.GetInstanceId()})";
            }

        
        }

        public T GetOrCreateAsResource<T>(string filePathAndKey) where T : Resource, new()
        {
            if (!typeof(T).IsSubclassOf(typeof(Resource)))
            {
                throw new Exception(
                    $"GetOrCreateAsResource<T>() failed for type of {typeof(T)}. Only Resource types are allowed.");
            }

            var type = typeof(T);
            if (_singletonLookup.HasSingleton<T>(filePathAndKey))
            {
                return _singletonLookup.GetSingleton<T>(filePathAndKey);
            }
            else
            {
                T newInstance;

                // Is it a full path?
                if (FileAccessUtility.IsPathWithinResources(filePathAndKey))
                {
                    newInstance = GD.Load<T>(filePathAndKey);
                    _singletonLookup.AddSingleton(newInstance, filePathAndKey);
                }
                // Or is it just a filename?
                else
                {
                    string foundFilePath = FileAccessUtility.FindFileOnceInResources(filePathAndKey);
                    newInstance = GD.Load<T>(foundFilePath);
                    _singletonLookup.AddSingleton(newInstance, foundFilePath);
                }

                _logger.Print($"GetOrCreateAsResource() Type = '{type.Name}', Key = '{filePathAndKey}'.");
                return newInstance;
            }
        }

        private void GDPrint(string message)
        {
            _logger.Print(message);
        }

        private void GDPrintErr(string message)
        {
            _logger.PrintErr(message);
        }
    }
}
