using GdUnit4;
using static GdUnit4.Assertions;

namespace RMC.Mingletons.Tests
{
    [TestSuite]
    public partial class SingletonLookupTests
    {
        // Nested Test Class -----------------------------
        private class TestClass
        {
        }
        
        // Fields ----------------------------------------
        private SingletonLookup _singletonLookup;

        // Initialization -------------------------------
        [Before]
        public void BeforeTestSuite()
        {
            // GD.Print("BeforeTestSuite");
        }

        [BeforeTest]
        public void BeforeTest()
        {
            _singletonLookup = new SingletonLookup(true);
        }

        [AfterTest]
        public void AfterTest()
        {
            _singletonLookup.Dispose();
        }

        [After]
        public void AfterTestSuite()
        {
            // GD.Print("AfterTestSuite");
        }

        // Methods ---------------------------------------
        [TestCase]
        public void Test_AddSingleton()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            _singletonLookup.AddSingleton(instance);

            // Assert
            AssertObject(_singletonLookup.GetSingleton<TestClass>()).IsNotNull();
        }

        [TestCase]
        public void Test_AddSingleton_WithKey()
        {
            // Arrange
            var instance = new TestClass();
            string key = "TestKey";

            // Act
            _singletonLookup.AddSingleton(instance, key);

            // Assert
            AssertObject(_singletonLookup.GetSingleton<TestClass>(key)).IsNotNull();
        }

        [TestCase]
        public void Test_AddSingleton_AlreadyExists()
        {
            // Arrange
            var instance = new TestClass();
            var instance2 = new TestClass();
            string key = "TestKey";

            // Act
            _singletonLookup.AddSingleton(instance, key);
            _singletonLookup.AddSingleton(instance2, key);

            // Assert
            AssertObject(_singletonLookup.GetSingleton<TestClass>(key)).IsEqual(instance);
            // Assert that the second instance was not added and logger printed an error
        }

        [TestCase]
        public void Test_HasSingleton()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            _singletonLookup.AddSingleton(instance);
            bool hasSingleton = _singletonLookup.HasSingleton<TestClass>();

            // Assert
            AssertBool(hasSingleton).IsTrue();
        }

        [TestCase]
        public void Test_HasSingleton_WithKey()
        {
            // Arrange
            var instance = new TestClass();
            string key = "TestKey";

            // Act
            _singletonLookup.AddSingleton(instance, key);
            bool hasSingleton = _singletonLookup.HasSingleton<TestClass>(key);

            // Assert
            AssertBool(hasSingleton).IsTrue();
        }

        [TestCase]
        public void Test_GetSingleton()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            _singletonLookup.AddSingleton(instance);
            var retrievedInstance = _singletonLookup.GetSingleton<TestClass>();

            // Assert
            AssertObject(retrievedInstance).IsEqual(instance);
        }

        [TestCase]
        public void Test_GetSingleton_WithKey()
        {
            // Arrange
            var instance = new TestClass();
            string key = "TestKey";

            // Act
            _singletonLookup.AddSingleton(instance, key);
            var retrievedInstance = _singletonLookup.GetSingleton<TestClass>(key);

            // Assert
            AssertObject(retrievedInstance).IsEqual(instance);
        }

        [TestCase]
        public void Test_RemoveSingleton()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            _singletonLookup.AddSingleton(instance);
            _singletonLookup.RemoveSingleton<TestClass>();

            // Assert
            AssertObject(_singletonLookup.GetSingleton<TestClass>()).IsNull();
        }

        [TestCase]
        public void Test_RemoveSingleton_WithKey()
        {
            // Arrange
            var instance = new TestClass();
            string key = "TestKey";

            // Act
            _singletonLookup.AddSingleton(instance, key);
            _singletonLookup.RemoveSingleton<TestClass>(key);

            // Assert
            AssertObject(_singletonLookup.GetSingleton<TestClass>(key)).IsNull();
        }

        [TestCase]
        public void Test_Dispose()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            _singletonLookup.AddSingleton(instance);
            _singletonLookup.Dispose();

            // Assert
            AssertBool(_singletonLookup.HasSingleton<TestClass>()).IsFalse();
        }
    }
}
