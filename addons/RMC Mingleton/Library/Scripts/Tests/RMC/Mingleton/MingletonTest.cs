using System.Threading.Tasks;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace RMC.Mingletons
{
    [TestSuite]
    public partial class MingletonTest
    {
        
        public partial class SampleClass { }
        public partial class SampleNode : Node { }
        
        [Before]
        public void BeforeTestSuite()
        {
            //GD.Print("BeforeTestSuite");
        }
        
        [BeforeTest]
        public void BeforeTest()
        {
            //GD.Print("BeforeTest");
            Mingleton.DisposeStatic();
        }
        
        [AfterTest]
        public void AfterTest()
        {
            //GD.Print("AfterTest");
        }
        
        [After]
        public void AfterTestSuite()
        {
           // GD.Print("AfterTestSuite");
        }
        
        [TestCase]
        public void Instance_IsNotNull_WhenDefault()
        {
            // Arrange

            // Act
            var result = Mingleton.Instance;

            // Assert
            AssertThat(result).IsNotNull();
        }
        
        [TestCase]
        public void Instance_IsNotNull_WhenInstantiateAsync()
        {
            // Arrange
            _ = Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance;
        
            // Assert
            AssertThat(result).IsNotNull();
        }
        
        [TestCase]
        public void IsInstantiated_IsFalseWhenDefault()
        {
            // Arrange
        
            // Act
            var result = Mingleton.IsInstantiated;
        
            // Assert
            AssertThat(result).IsFalse();
        }
        
        [TestCase]
        public void IsInstantiated_IsTrue_WhenInstantiateAsync()
        {
            // Arrange
            _ = Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.IsInstantiated;
        
            // Assert
            AssertThat(result).IsTrue();
        }
        
        [TestCase]
        public void IsInstantiated_IsTrue_WhenInstantiate()
        {
            // Arrange
            Mingleton.Instantiate();
        
            // Act
            var result = Mingleton.IsInstantiated;
        
            // Assert
            AssertThat(result).IsTrue();
        }

        
        [TestCase]
        public void Instance_IsNotNull_WhenInstantiate()
        {
            // Arrange
            Mingleton.Instantiate();
        
            // Act
            var result = Mingleton.Instance;
        
            // Assert
            AssertThat(result).IsNotNull();
        }
        
        [TestCase]
        public async Task AddSingleton_AssertThrown_WhenCalled2Times()
        {
            // Arrange
            var classInstance1 = new SampleClass();
            var classInstance2 = new SampleClass();
            await Mingleton.InstantiateAsync();
            
        
            // Assert
            AssertThrown(() =>
            {
                // Act
                Mingleton.Instance.AddSingleton<SampleClass>(classInstance1);
                Mingleton.Instance.AddSingleton<SampleClass>(classInstance2);
            });
        }
        
        
        [TestCase]
        public async Task HasSingleton_IsFalse_WhenNotAdded()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.HasSingleton<SampleClass>();
        
            // Assert
            AssertThat(result).IsFalse();
        }
        
        [TestCase]
        public async Task HasSingleton_IsTrue_WhenAdded()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
            Mingleton.Instance.AddSingleton<SampleClass>(new SampleClass());
        
            // Act
            var result = Mingleton.Instance.HasSingleton<SampleClass>();
        
            // Assert
            AssertThat(result).IsTrue();
        }
        
        [TestCase]
        public async Task HasSingleton_IsFalse_WhenAddedRemoved()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
            Mingleton.Instance.AddSingleton<SampleClass>(new SampleClass());
            Mingleton.Instance.RemoveSingleton<SampleClass>();
            
            // Act
            var result = Mingleton.Instance.HasSingleton<SampleClass>();
        
            // Assert
            AssertThat(result).IsFalse();
        }
        
        [TestCase]
        public async Task GetSingleton_IsSameAs_WhenAdded()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
            var classInstance = new SampleClass();
            Mingleton.Instance.AddSingleton<SampleClass>(classInstance);
        
            // Act
            var result = Mingleton.Instance.GetSingleton<SampleClass>();
        
            // Assert
            AssertThat(result).Equals(classInstance);
        }
        
        [TestCase]
        public async Task GetSingleton_IsNull_WhenNotNotAdded()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.GetSingleton<SampleClass>();
        
            // Assert
            AssertThat(result).Equals(null);    
        }
        
        [TestCase]
        public async Task GetOrCreateAsClass_ResultTypeIsCorrect_WhenCalled()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.GetOrCreateAsClass<SampleClass>();
        
            // Assert
            AssertThat(result is SampleClass).IsNotNull();
        }
        
        [TestCase]
        public async Task GetOrCreateAsNode_ResultTypeIsCorrect_WhenCalled()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.GetOrCreateAsNode<SampleNode>();
        
            // Assert
            AssertThat(result is SampleNode).IsNotNull();
        }
        
        [TestCase]
        public async Task GetOrCreateAsNode_ResultGetTreeIsNotNull_WhenCalled()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.GetOrCreateAsNode<SampleNode>();
        
            // Assert
            AssertThat(result.GetTree()).IsNotNull();
        }
        
        [TestCase]
        public async Task GetOrCreateAsNode_ResultParentIsMingleton_WhenCalled()
        {
            // Arrange
            await Mingleton.InstantiateAsync();
        
            // Act
            var result = Mingleton.Instance.GetOrCreateAsNode<SampleNode>();
        
            // Assert
            AssertThat(result.GetParent() == Mingleton.Instance).IsTrue();
        }
    }
}
