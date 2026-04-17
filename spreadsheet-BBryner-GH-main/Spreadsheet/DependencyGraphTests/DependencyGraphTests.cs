// Testing Class for DependencyGraph
// Implemented by Brenden Bryner
// StressTest provided via CS3500 Canvas
// September 11, 2025

namespace DependencyGraphTests;

using DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
  
  // provided test from canvas for stresstest example
  
  /// <summary>
  ///  Test to see if the Graph works under a lot of dependencies with lots of add/remove operations,
  /// makes sure the results are correct as well
  /// </summary>
  [TestMethod]
  [Timeout( 2000 )]  // 2 second run time limit
  public void StressTest()
  {
    DependencyGraph dg = new();

    // A bunch of strings to use
    const int SIZE = 200;
    string[] letters = new string[SIZE];
    for ( int i = 0; i < SIZE; i++ )
    {
      letters[i] = string.Empty + ( (char) ( 'a' + i ) );
    }

    // The correct answers
    HashSet<string>[] dependents = new HashSet<string>[SIZE];
    HashSet<string>[] dependees = new HashSet<string>[SIZE];
    for ( int i = 0; i < SIZE; i++ )
    {
      dependents[i] = [];
      dependees[i] = [];
    }

    // Add a bunch of dependencies
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 1; j < SIZE; j++ )
      {
        dg.AddDependency( letters[i], letters[j] );
        dependents[i].Add( letters[j] );
        dependees[j].Add( letters[i] );
      }
    }

    // Remove a bunch of dependencies
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 4; j < SIZE; j += 4 )
      {
        dg.RemoveDependency( letters[i], letters[j] );
        dependents[i].Remove( letters[j] );
        dependees[j].Remove( letters[i] );
      }
    }

    // Add some back
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 1; j < SIZE; j += 2 )
      {
        dg.AddDependency( letters[i], letters[j] );
        dependents[i].Add( letters[j] );
        dependees[j].Add( letters[i] );
      }
    }

    // Remove some more
    for ( int i = 0; i < SIZE; i += 2 )
    {
      for ( int j = i + 3; j < SIZE; j += 3 )
      {
        dg.RemoveDependency( letters[i], letters[j] );
        dependents[i].Remove( letters[j] );
        dependees[j].Remove( letters[i] );
      }
    }

    // Make sure everything is right
    for ( int i = 0; i < SIZE; i++ )
    {
      Assert.IsTrue( dependents[i].SetEquals( new HashSet<string>( dg.GetDependents( letters[i] ) ) ) );
      Assert.IsTrue( dependees[i].SetEquals( new HashSet<string>( dg.GetDependees( letters[i] ) ) ) );
    }
  }
  
  // Extra Tests (All non provided below) ---
  
  // Testing Constructor --- 

  /// <summary>
  /// Test to see if the basic constructor call works (should have the two dictionaries and a size of zero)
  /// </summary>
  [TestMethod]
  public void DependencyGraph_TestBasicConstructor_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    Assert.AreEqual(0, dependencyGraph.Size);
  }
  
  // Testing Size ---

  /// <summary>
  /// Test to see if having one dependency makes the size 1
  /// </summary>
  [TestMethod]
  public void Size_TestSizeOne_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    Assert.AreEqual(1, dependencyGraph.Size);
  }

  [TestMethod]
  public void Size_TestSizeMultiple_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    for (int i = 0; i < 10; i++)
    {
      dependencyGraph.AddDependency("dependee" + i, "dependent" + i);
    }
    Assert.AreEqual(10, dependencyGraph.Size);
  }
  
  // Testing HasDependents ---

  /// <summary>
  /// see if HasDependents works with a single dependent
  /// </summary>
  [TestMethod]
  public void HasDependents_NodeWithDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    Assert.IsTrue(dependencyGraph.HasDependents("dependee"));
  }

  /// <summary>
  /// should return false when testing for a node without dependents
  /// </summary>
  [TestMethod]
  public void HasDependents_NodeWithoutDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    Assert.IsFalse(dependencyGraph.HasDependents("dependent"));
  }

  /// <summary>
  /// Will an empty graph break HasDependents
  /// </summary>
  [TestMethod]
  public void HasDependents_EmptyGraph_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    Assert.IsFalse(dependencyGraph.HasDependents("node"));
  }
  
  // Testing HasDependees ---

  /// <summary>
  /// Testing Basic case of HasDependees
  /// </summary>
  [TestMethod]
  public void HasDependees_NodeWithDependees_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    Assert.IsTrue(dependencyGraph.HasDependees("dependent"));
  }

  /// <summary>
  /// Should return false with a node without dependee
  /// </summary>
  [TestMethod]
  public void HasDependees_NodeWithoutDependees_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    Assert.IsFalse(dependencyGraph.HasDependees("dependee"));
  }

  /// <summary>
  /// see if an empty graph will break HasDependee
  /// </summary>
  [TestMethod]
  public void HasDependees_EmptyGraph_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    Assert.IsFalse(dependencyGraph.HasDependees("node"));
  }
  
  // Testing GetDependents ---

  /// <summary>
  /// test to see if one Dependent breaks GetDependents
  /// </summary>
  [TestMethod]
  public void GetDependents_NodeWithDependent_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    var dependentList = dependencyGraph.GetDependents("dependee");
    Assert.IsTrue(new HashSet<string> {"dependent"}.SetEquals(dependentList));
  }

  /// <summary>
  /// testing to see if GetDependents will grab multiple dependents
  /// </summary>
  [TestMethod]
  public void GetDependents_MultipleDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    for (int i = 0; i < 10; i++)
    {
      dependencyGraph.AddDependency("dependee", "dependent" + i);
    }
    var dependentList = dependencyGraph.GetDependents("dependee");
    Assert.IsTrue(new HashSet<string> {"dependent0", "dependent1", "dependent2", "dependent3" ,"dependent4", 
      "dependent5", "dependent6", "dependent7", "dependent8", "dependent9"}.SetEquals(dependentList));
  }

  /// <summary>
  /// make sure that GetDependents doesn't get the wrong dependents
  /// </summary>
  [TestMethod]
  public void GetDependents_MultipleDependencies_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    for (int i = 0; i < 10; i++)
    {
      dependencyGraph.AddDependency("dependee", "dependent" + i);
    }
    // add another dependee
    dependencyGraph.AddDependency("otherDependee", "otherDependent");
    var dependentList = dependencyGraph.GetDependents("otherDependee");
    Assert.IsTrue(new HashSet<string> {"otherDependent"}.SetEquals(dependentList));
  }

  /// <summary>
  /// see if searching an empty graph breaks GetDependents
  /// </summary>
  [TestMethod]
  public void GetDependents_EmptyGraph_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    var dependentList = dependencyGraph.GetDependents("node");
    Assert.IsFalse(new HashSet<string> {"dependent"}.SetEquals(dependentList));
  }
  
  // Testing GetDependees ---
  
  /// <summary>
  /// test to see if one Dependent breaks GetDependents
  /// </summary>
  [TestMethod]
  public void GetDependees_NodeWithDependee_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    var dependentList = dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"dependee"}.SetEquals(dependentList));
  }

  /// <summary>
  /// testing to see if GetDependents will grab multiple dependents
  /// </summary>
  [TestMethod]
  public void GetDependees_MultipleDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    for (int i = 0; i < 10; i++)
    {
      dependencyGraph.AddDependency("dependee" + i, "dependent");
    }
    var dependeeList = dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"dependee0", "dependee1", "dependee2", "dependee3" ,"dependee4", 
      "dependee5", "dependee6", "dependee7", "dependee8", "dependee9"}.SetEquals(dependeeList));
  }

  /// <summary>
  /// make sure that GetDependents doesn't get the wrong dependents
  /// </summary>
  [TestMethod]
  public void GetDependee_MultipleDependencies_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    for (int i = 0; i < 10; i++)
    {
      dependencyGraph.AddDependency("dependee" + i, "dependent");
    }
    // add another dependee
    dependencyGraph.AddDependency("otherDependee", "otherDependent");
    var dependeeList = dependencyGraph.GetDependees("otherDependent");
    Assert.IsTrue(new HashSet<string> {"otherDependee"}.SetEquals(dependeeList));
  }

  /// <summary>
  /// see if searching an empty graph breaks GetDependents
  /// </summary>
  [TestMethod]
  public void GetDependee_EmptyGraph_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    var dependeeList = dependencyGraph.GetDependees("node");
    Assert.IsFalse(new HashSet<string> {"dependent"}.SetEquals(dependeeList));
  }
  
  // Testing AddDependency ---

  /// <summary>
  /// basic instance of adding a dependency
  /// </summary>
  [TestMethod]
  public void AddDependency_Basic_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.AddDependency("otherDependee", "otherDependent");
    Assert.AreEqual(2, dependencyGraph.Size);
  }

  /// <summary>
  /// test to see if adding a dependency multiple times adds to the size
  /// </summary>
  [TestMethod]
  public void AddDependency_SeeIfAddingUpdatesSize_MultipleDependencies_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    Assert.AreEqual(0, dependencyGraph.Size);
    dependencyGraph.AddDependency("1", "1");
    Assert.AreEqual(1, dependencyGraph.Size);
    dependencyGraph.AddDependency("2", "2");
    Assert.AreEqual(2, dependencyGraph.Size);
  }

  /// <summary>
  /// test to see if duplicate dependencies increase the size
  /// </summary>
  [TestMethod]
  public void AddDependency_DuplicateDependenciesDontIncrementSize_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("1", "1");
    Assert.AreEqual(1, dependencyGraph.Size);
    dependencyGraph.AddDependency("1", "1");
    Assert.AreEqual(1, dependencyGraph.Size);
    dependencyGraph.AddDependency("1", "1");
    Assert.AreEqual(1, dependencyGraph.Size);
    dependencyGraph.AddDependency("1", "1");
    Assert.AreEqual(1, dependencyGraph.Size);
  }
  
  // Testing RemoveDependency ---

  /// <summary>
  /// basic case of removing dependency
  /// </summary>
  [TestMethod]
  public void RemoveDependency_BasicTest_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.RemoveDependency("dependee", "dependent");
    Assert.AreEqual(0, dependencyGraph.Size);
  }

  /// <summary>
  /// will removing the dependencies decrement the size
  /// </summary>
  [TestMethod]
  public void RemoveDependency_MultipleDependenciesDecrementSize_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.AddDependency("dependee1", "dependent1");
    dependencyGraph.AddDependency("dependee2", "dependent2");
    dependencyGraph.AddDependency("dependee3", "dependent3");
    Assert.AreEqual(4, dependencyGraph.Size);
    dependencyGraph.RemoveDependency("dependee", "dependent");
    dependencyGraph.RemoveDependency("dependee1", "dependent1");
    dependencyGraph.RemoveDependency("dependee2", "dependent2");
    dependencyGraph.RemoveDependency("dependee3", "dependent3");
    Assert.AreEqual(0, dependencyGraph.Size);
  }

  /// <summary>
  /// make sure that removing a dependency doesn't break an empty graph or decrement past 0
  /// </summary>
  [TestMethod]
  public void RemoveDependency_TestRemoveOnEmptyGraph_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.RemoveDependency("dependee", "dependee");
    Assert.AreEqual(0, dependencyGraph.Size);
  }
  
  // Testing ReplaceDependents ---

  /// <summary>
  /// see if ReplaceDependents can replace a single dependent
  /// </summary>
  [TestMethod]
  public void ReplaceDependents_ReplaceOneDependent_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.ReplaceDependents("dependee", new List<string> { "replacedDependent" });
    var dependentsList =  dependencyGraph.GetDependents("dependee");
    Assert.IsTrue(new HashSet<string> {"replacedDependent"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// Testing to see if multiple dependents can be replaced
  /// </summary>
  [TestMethod]
  public void ReplaceDependents_ReplaceMultipleDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent1");
    dependencyGraph.AddDependency("dependee", "dependent2");
    dependencyGraph.AddDependency("dependee", "dependent3");
    dependencyGraph.AddDependency("dependee", "dependent4");
    dependencyGraph.AddDependency("dependee", "dependent5");
    dependencyGraph.ReplaceDependents("dependency", new List<string> { "replacedDependent" });
    var dependentsList = dependencyGraph.GetDependents("dependency");
    Assert.IsTrue(new HashSet<string> {"replacedDependent"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// testing to see if something can be replaced with multiple dependents
  /// </summary>
  [TestMethod]
  public void ReplaceDependents_ReplaceWithMultipleDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.ReplaceDependents("dependency", new List<string> { "replacedDependent1", "replacedDependent2", "replacedDependent3" });
    var dependentsList = dependencyGraph.GetDependents("dependency");
    Assert.IsTrue(new HashSet<string> {"replacedDependent1", "replacedDependent2", "replacedDependent3"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// testing to see if multiple dependents can be replaced with multiple dependents
  /// </summary>
  [TestMethod]
  public void ReplaceDependents_ReplaceMultipleDependentsWithMultipleDependents_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent1");
    dependencyGraph.AddDependency("dependee", "dependent2");
    dependencyGraph.AddDependency("dependee", "dependent3");
    dependencyGraph.AddDependency("dependee", "dependent4");
    dependencyGraph.AddDependency("dependee", "dependent5");
    dependencyGraph.ReplaceDependents("dependency", new List<string> { "replacedDependent1", "replacedDependent2", "replacedDependent3", "replacedDependent4", "replacedDependent5" });
    var dependentsList = dependencyGraph.GetDependents("dependency");
    Assert.IsTrue(new HashSet<string> {"replacedDependent1", "replacedDependent2", "replacedDependent3", "replacedDependent4", "replacedDependent5"}.SetEquals(dependentsList));
  }
  
  // Testing ReplaceDependees ---
  
  /// <summary>
  /// testing a replacement of one dependee
  /// </summary>
    [TestMethod]
  public void ReplaceDependees_ReplaceOneDependee_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.ReplaceDependees("dependent", new List<string> { "replacedDependee" });
    var dependentsList =  dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"replacedDependee"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// testing to see if replacedependees can replace multiple dependees
  /// </summary>
  [TestMethod]
  public void ReplaceDependee_ReplaceMultipleDependees_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee1", "dependent");
    dependencyGraph.AddDependency("dependee2", "dependent");
    dependencyGraph.AddDependency("dependee3", "dependent");
    dependencyGraph.AddDependency("dependee4", "dependent");
    dependencyGraph.AddDependency("dependee5", "dependent");
    dependencyGraph.ReplaceDependees("dependent", new List<string> { "replacedDependee" });
    var dependentsList = dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"replacedDependee"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// testing to see if something can be replaced with multiple dependees
  /// </summary>
  [TestMethod]
  public void ReplaceDependees_ReplaceWithMultipleDependees_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("dependee", "dependent");
    dependencyGraph.ReplaceDependees("dependent", new List<string> { "replacedDependee1", "replacedDependee2", "replacedDependee3" });
    var dependentsList = dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"replacedDependee1", "replacedDependee2", "replacedDependee3"}.SetEquals(dependentsList));
  }

  /// <summary>
  /// testing the replacement of multiple dependees with multiple dependees
  /// </summary>
  [TestMethod]
  public void ReplaceDependees_ReplaceMultipleDependeesWithMultipleDependees_Valid()
  {
    DependencyGraph dependencyGraph = new DependencyGraph();
    dependencyGraph.AddDependency("replacedDependee1", "dependent");
    dependencyGraph.AddDependency("replacedDependee2", "dependent");
    dependencyGraph.AddDependency("replacedDependee3", "dependent");
    dependencyGraph.AddDependency("replacedDependee4", "dependent");
    dependencyGraph.AddDependency("replacedDependee5", "dependent");
    dependencyGraph.ReplaceDependees("dependent", new List<string> { "replacedDependee1", "replacedDependee2", "replacedDependee3", "replacedDependee4", "replacedDependee5" });
    var dependentsList = dependencyGraph.GetDependees("dependent");
    Assert.IsTrue(new HashSet<string> {"replacedDependee1", "replacedDependee2", "replacedDependee3", "replacedDependee4", "replacedDependee5"}.SetEquals(dependentsList));
  }
  
}