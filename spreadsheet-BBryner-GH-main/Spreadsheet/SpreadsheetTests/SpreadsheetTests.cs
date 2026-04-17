// <authors> Brenden Bryner </authors>
// <date> September 19, 2025 </date>

using Spreadsheets;

namespace SpreadsheetTests;

using Formula;

/// <summary>
/// <para>
/// The following class contains tests for the Spreadsheets implementation
/// </para>
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    
    // --- TESTS BELOW ---
    
    // --- Spreadsheet Constructor ---

    /// <summary>
    /// make sure that a new spreadsheet is just empty
    /// </summary>
    [TestMethod]
    public void SpreadsheetConstructor_CreateSpreadsheet_Valid()
    {
        var spreadsheet = new Spreadsheet();
        Assert.IsEmpty(spreadsheet.GetNamesOfAllNonemptyCells());
    }
    
    // --- GetNamesOfAllNonemptyCells TESTS ---

    /// <summary>
    /// basic test case for one cell
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_OneCell_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        var nonEmpty = spreadsheet.GetNamesOfAllNonemptyCells();
        Assert.Contains("A1", nonEmpty);
    }
    
    /// <summary>
    /// test case for multiple cells
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_MultipleCells_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        spreadsheet.SetContentsOfCell("B2", "String");
        spreadsheet.SetContentsOfCell("C3", "A1+C4");
        spreadsheet.SetContentsOfCell("C4", "1.0");
        var nonEmpty = spreadsheet.GetNamesOfAllNonemptyCells();
        Assert.Contains("A1", nonEmpty);
        Assert.Contains("B2", nonEmpty);
        Assert.Contains("C3", nonEmpty);
        Assert.Contains("C4", nonEmpty);
    }    
    
    // --- GetCellContents TESTS ---
    
    /// <summary>
    /// testing with an empty cell
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestEmptyCell_Valid(){
        Spreadsheet spreadsheet = new Spreadsheet();
        var empty = spreadsheet.GetCellContents("A1");
        Assert.AreEqual("", empty);
    }
    
    /// <summary>
    /// get cell contents with a double
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestCellWithDouble_Valid(){
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        var doubleCell =  spreadsheet.GetCellContents("A1");
        Assert.AreEqual(1.0, doubleCell);
    }

    /// <summary>
    /// get cell contents with a string
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestCellWithString_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        var doubleCell =  spreadsheet.GetCellContents("A1");
        Assert.AreEqual("String", doubleCell);
    }

    /// <summary>
    /// see if testing with an invalid name will throw on double method
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestInvalidNameDouble_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.Throws<InvalidNameException>(() => spreadsheet.GetCellContents("1A1"));
    }
    
    /// <summary>
    /// see if testing with an invalid name will throw on string method
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestInvalidNameString_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "string");
        Assert.Throws<InvalidNameException>(() => spreadsheet.GetCellContents("1A1"));
    }
    
    /// <summary>
    /// see if testing with a null name will throw
    /// </summary>
    [TestMethod]
    public void GetCellContents_TestInvalidNameNull_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "string");
        Assert.Throws<InvalidNameException>(() => spreadsheet.GetCellContents(null));
    }
    
    // --- SetContentsOfCell TESTS ---

    /// <summary>
    /// setting a cell with a string
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_BasicString_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// setting a cell with a double
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_BasicDouble_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(1.0, spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// setting a cell with a formula
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_BasicFormula_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=A2");
        Assert.AreEqual(new Formula("A2"), spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// will setting a cell with an empty string make it empty
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_EmptyString_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        spreadsheet.SetContentsOfCell("A1", "");
        Assert.AreEqual("", spreadsheet.GetCellContents("A1"));
    }
    
    /// <summary>
    /// will setting a cell with an empty string take it off the nonempty cell list
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_EmptyStringRemoveFromList_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "");
        Assert.DoesNotContain("A1", spreadsheet.GetNamesOfAllNonemptyCells());
    }

    /// <summary>
    /// see if you can change the cell from double to string
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_DoubleToString_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(1.0, spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
    }
    
    /// <summary>
    /// see if you can change the cell from string to double
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_StringToDouble_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(1.0, spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// changing the cell from string to formula
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_StringToFormula_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "=A2+B2");
        Assert.AreEqual(new Formula("A2+B2"), spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// changing the cell from double to formula
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_DoubleToFormula_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(1.0, spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "=A2+B2");
        Assert.AreEqual(new Formula("A2+B2"), spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// see if you can change the cell contents from formula to string
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_FormulaToString_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=A2+B2");
        Assert.AreEqual(new Formula("A2+B2"), spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "String");
        Assert.AreEqual("String", spreadsheet.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetContentsOfCell_FormulaToDouble_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=A2+B2");
        Assert.AreEqual(new Formula("A2+B2"), spreadsheet.GetCellContents("A1"));
        spreadsheet.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(1.0, spreadsheet.GetCellContents("A1"));
    }

    /// <summary>
    /// see if setting cell contents with a lowercase cell name will still work
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_TestCellNamesNormalize_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "1.0");
        var formulaCell = spreadsheet.GetCellContents("A1");
        Assert.AreEqual(1.0, formulaCell);
    }
    
    /// <summary>
    /// see if setting cell contents with an invalid name will work on the double method
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_TestInvalidNameDouble_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => spreadsheet.SetContentsOfCell("1A1", "1.0"));
    }
    
    /// <summary>
    /// see if setting cell contents with an invalid name will work on the string method
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_TestInvalidNameString_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => spreadsheet.SetContentsOfCell("1A1", "string"));
    }
    
    /// <summary>
    /// see if setting cell contents with an invalid name will work on the formula method
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_TestInvalidNameFormula_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => spreadsheet.SetContentsOfCell("1A1", "=A1"));
    }

    /// <summary>
    /// see if a circular exception will be thrown 
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_TestCircularDependency_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=B1 + 2");
        Assert.Throws<CircularException>(() => spreadsheet.SetContentsOfCell("B1", "=A1 +2"));
    }
    
    // --- PS6 Tests --- 
    
    // Save tests

    /// <summary>
    /// basic test case for saving
    /// </summary>
    [TestMethod]
    public void Save_TestBasicSave_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.Save("basicsave.json");
        Assert.IsTrue(File.Exists("basicsave.json"));
        File.Delete("basicsave.json");
    }
    
    /// <summary>
    /// will a nonexistent filepath throw a read write exception
    /// </summary>
    [TestMethod]
    public void Save_TestSavingToNonexistentPath_ReadWriteException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        Assert.Throws<SpreadsheetReadWriteException>(() => spreadsheet.Save("/some/nonsense/path.txt"));
    }
    
    /// <summary>
    /// saving spreadsheet with string cell
    /// </summary>
    [TestMethod]
    public void Save_WithStringCell_IsValid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "hello world");
        ss.Save("stringsave.json");
    
        string json = File.ReadAllText("stringsave.json");
        Assert.IsTrue(json.Contains("hello world"));
    
        File.Delete("stringsave.json");
    }
    
    // Load tests with spreadsheet

    /// <summary>
    /// see if a spreadsheet can load from a saved one
    /// </summary>
    [TestMethod]
    public void Spreadsheet_TestBasicLoad_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.Save("basicload.json");
        
        Spreadsheet loadedSpreadsheet = new Spreadsheet("basicload.json");
        
        Assert.AreEqual(5.0, loadedSpreadsheet.GetCellContents("A1"));
        File.Delete("basicload.json");
    }

    /// <summary>
    /// see if a spreadsheet can load multiple formulas
    /// </summary>
    [TestMethod]
    public void Spreadsheet_TestMultipleFormulas_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "10");
        spreadsheet.SetContentsOfCell("B1", "=A1*2");
        spreadsheet.SetContentsOfCell("C1", "=B1+5");        
        spreadsheet.Save("formulaload.json");
        
        Spreadsheet loadedSpreadsheet = new Spreadsheet("formulaload.json");
        
        Assert.AreEqual(10.0, loadedSpreadsheet.GetCellValue("A1"));
        Assert.AreEqual(20.0, loadedSpreadsheet.GetCellValue("B1"));
        Assert.AreEqual(25.0, loadedSpreadsheet.GetCellValue("C1"));        
        File.Delete("formulaload.json");
    }
    
    /// <summary>
    /// loading file with invalid formula should throw spreadsheet read write exception
    /// </summary>
    [TestMethod]
    public void Spreadsheet_InvalidFormula_ThrowsException()
    {
        string json = @"{""Cells"": {""A1"": { ""StringForm"": ""=++"" }}}";
        File.WriteAllText("invalidformula.json", json);
    
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("invalid_formula.json"));
        File.Delete("invalidformula.json");
    }
    
    /// <summary>
    /// loading file with bad json file should throw
    /// </summary>
    [TestMethod]
    public void Spreadsheet_InvalidStructureForm_ThrowsException()
    {
        string json = @"{""Cells"": {""A1"": { }}}";
        File.WriteAllText("invalidstructure.json", json);
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("invalid_structure.json"));
        File.Delete("invalidstructure.json");
    }
    
    /// <summary>
    /// loading file with missing StringForm
    /// </summary>
    [TestMethod]
    public void Spreadsheet_NoStringForm_ThrowsException()
    {
        string json = @"{ ""Cells"": { ""A1"" : { ""WrongProperty"": ""5""}}}";
        File.WriteAllText("missingstringform.json", json);
    
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("missingstringform.json"));
        File.Delete("missingstringform.json");
    }
    
    /// <summary>
    /// loading file with circular dependency should throw 
    /// </summary>
    [TestMethod]
    public void Spreadsheet_CircularDependency_ThrowsException()
    {
        string json = @"{""Cells"": {""A1"": { ""StringForm"": ""=B1"" },""B1"": { ""StringForm"": ""=A1"" }}}";
        File.WriteAllText("circular.json", json);
    
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("circular.json"));
        File.Delete("circular.json");
    }

    /// <summary>
    /// losing with an invalid name should throw exception
    /// </summary>
    [TestMethod]
    public void Spreadsheet_TestInvalidName_ThrowsException()
    {
        Assert.Throws<SpreadsheetReadWriteException>(() => 
        {
            Spreadsheet spreadsheet = new Spreadsheet("/some/nonsense/path.txt");
        });    
    }
    
    /// <summary>
    ///  loading file with missing Cells should throw
    /// </summary>
    [TestMethod]
    public void Spreadsheet_MissingCellsProperty_ThrowsException()
    {
        string json = @"{ ""NotCells"": {} }";
        File.WriteAllText("missingcells.json", json);
    
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("missingcells.json"));
        File.Delete("missingcells.json");
    }

    /// <summary>
    ///  loading file with null data should throw
    /// </summary>
    [TestMethod]
    public void Spreadsheet_NullData_ThrowsException()
    {
        File.WriteAllText("nulldata.json", "null");
    
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("nulldata.json"));
        File.Delete("nulldata.json");
    }
    
    // Changed Tests
    
    /// <summary>
    /// new spreadsheet should have changed = false
    /// </summary>
    [TestMethod]
    public void Changed_NewSpreadsheet_IsFalse()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.IsFalse(spreadsheet.Changed);
    }
    
    /// <summary>
    /// test if the setcontents will turn changed to true
    /// </summary>
    [TestMethod]
    public void Changed_AfterSetContents_IsTrue()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        Assert.IsTrue(spreadsheet.Changed);
    }
    
    /// <summary>
    /// test to see if changed is false after save
    /// </summary>
    [TestMethod]
    public void Changed_AfterSave_IsFalse()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.Save("testfile1.json");
        Assert.IsFalse(spreadsheet.Changed);
        File.Delete("testfile1.json");
    }
    
    /// <summary>
    /// testing to see if saving and modifying changes the value
    /// </summary>
    [TestMethod]
    public void Changed_AfterSaveThenModify_IsTrue()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.Save("testfile2.json");
        Assert.IsFalse(spreadsheet.Changed);
    
        spreadsheet.SetContentsOfCell("B1", "10");
        Assert.IsTrue(spreadsheet.Changed);
        File.Delete("testfile2.json");
    }
    
    // Indexer/This[] tests
    
    /// <summary>
    /// basic test of the indexer
    /// </summary>
    [TestMethod]
    public void Indexer_BasicTest_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
    
        Assert.AreEqual(5.0, spreadsheet["A1"]);
    }
    
    /// <summary>
    /// indexer test with formula
    /// </summary>
    [TestMethod]
    public void Indexer_GetCellValue_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.SetContentsOfCell("B1", "=A1+2");
    
        Assert.AreEqual(7.0, spreadsheet["B1"]);
    }

    /// <summary>
    /// see if the invalid name will throw an exception
    /// </summary>
    [TestMethod]
    public void Indexer_TestInvalidName_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => spreadsheet["1A1"]);
    }
    
    // GetCellValue tests
    
    /// <summary>
    /// test GetCellValue with integer contents to return double
    /// </summary>
    [TestMethod]
    public void GetCellValue_CellWithNumber_ReturnDouble()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        Assert.AreEqual(5.0, spreadsheet.GetCellValue("A1"));
    }
    
    /// <summary>
    /// GetCellValue with double contents
    /// </summary>
    [TestMethod]
    public void GetCellValue_CellWithDouble_RetrnDouble()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5.0");
        Assert.AreEqual(5.0, spreadsheet.GetCellValue("A1"));
    }
    
    /// <summary>
    /// GetCellValue with string contents
    /// </summary>
    [TestMethod]
    public void GetCellValue_WithString_ReturnString()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "string");
        Assert.AreEqual("string", spreadsheet.GetCellValue("A1"));
    }
    
    /// <summary>
    /// getCellValue with simple formula
    /// </summary>
    [TestMethod]
    public void GetCellValue_BasicFormula_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.SetContentsOfCell("B1", "=A1+2");
        Assert.AreEqual(7.0, spreadsheet.GetCellValue("B1"));
    }
    
    /// <summary>
    /// GetCellValue referencing a cell that is undefines should return a formulaerror
    /// </summary>
    [TestMethod]
    public void GetCellValue_UndefinedFormula_ReturnFormulaError()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=B1+2");
        var result = spreadsheet.GetCellValue("A1");
        Assert.IsTrue(result is FormulaError);
    }
    
    /// <summary>
    /// division by zero should cause formula error
    /// </summary>
    [TestMethod]
    public void GetCellValue_FormulaDivisionByZero_ReturnsFormulaError()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "0");
        spreadsheet.SetContentsOfCell("B1", "=5/A1");
        var result = spreadsheet.GetCellValue("B1");
        Assert.IsTrue(result is FormulaError);
    }
    
    /// <summary>
    /// getting cell value of an empty cell should return an empty string
    /// </summary>
    [TestMethod]
    public void GetCellValue_EmptyCell_ReturnsEmptyString()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.AreEqual("", spreadsheet.GetCellValue("A1"));
    }

    /// <summary>
    /// testing invalid names for getcellvalue
    /// </summary>
    [TestMethod]
    public void GetCellValue_InvalidName_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => spreadsheet.GetCellValue("1A1"));
    }
    
    /// <summary>
    /// formula referencing a string should be a formula error
    /// </summary>
    [TestMethod]
    public void GetCellValue_FormulaReferencingString_ReturnsFormulaError()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "hello");
        ss.SetContentsOfCell("B1", "=A1+2");
    
        var result = ss.GetCellValue("B1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    
    /// <summary>
    /// formula referencing another with an empty cell returns error
    /// </summary>
    [TestMethod]
    public void GetCellValue_FormulaReferencingEmptyCell_ReturnsFormulaError()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("B1", "=A1+2");
    
        var result = ss.GetCellValue("B1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    } 
    
    // SetContentsOfCell Tests (mainly checking if the formulas and calculations change, more tests have happened above)
    
    /// <summary>
    /// test that changing the cell updates the formulas
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_UpdateDependents_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "5");
        spreadsheet.SetContentsOfCell("B1", "=A1*2");
        spreadsheet.SetContentsOfCell("C1", "=B1+10");
    
        // Verify initial values
        Assert.AreEqual(10.0, spreadsheet.GetCellValue("B1"));
        Assert.AreEqual(20.0, spreadsheet.GetCellValue("C1"));
    
        // Change A1 and verify dependencies update
        spreadsheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(20.0, spreadsheet.GetCellValue("B1"));
        Assert.AreEqual(30.0, spreadsheet.GetCellValue("C1"));
    }
    
    /// <summary>
    /// test to see if setting circular dpendencies will throw
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_CircularDependency_ThrowsException()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "=B1");
        spreadsheet.SetContentsOfCell("B1", "=C1");
    
        Assert.Throws<CircularException>(() => 
        {
            spreadsheet.SetContentsOfCell("C1", "=A1"); 
        });
    }
    
    
    // Stress Tests
    
    /// <summary>
    /// stresstest
    /// </summary>
    [TestMethod]
    [Timeout(5000)] // 5 second timeout
    public void Stresstest1_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
    
        // A1- A100 and all of the cells depend on the previous cell
        spreadsheet.SetContentsOfCell("A1", "1");
        for (int i = 2; i <= 100; i++)
        {
            spreadsheet.SetContentsOfCell($"A{i}", $"=A{i-1}+1");
        }
    
        // Change A1 and verify all update correctly
        spreadsheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(109.0, spreadsheet.GetCellValue("A100"));
    }
    
    /// <summary>
    /// longer stresstest
    /// </summary>
    [TestMethod]
    [Timeout(5000)] // 5 second timeout
    public void Stresstest2_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
    
        spreadsheet.SetContentsOfCell("A1", "1");
        for (int i = 2; i <= 1000; i++)
        {
            spreadsheet.SetContentsOfCell($"A{i}", $"=A{i-1}+1");
        }
    
        // Change A1 and verify all update correctly
        spreadsheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(1009.0, spreadsheet.GetCellValue("A1000"));
    }
    
    /// <summary>
    /// longer stresstest
    /// </summary>
    [TestMethod]
    [Timeout(5000)] // 5 second timeout
    public void Stresstest3_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
    
        spreadsheet.SetContentsOfCell("A1", "1");
        for (int i = 2; i <= 5000; i++)
        {
            spreadsheet.SetContentsOfCell($"A{i}", $"=A{i-1}+1");
        }
    
        // Change A1 and verify all update correctly
        spreadsheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(5009.0, spreadsheet.GetCellValue("A5000"));
    }
    
    /// <summary>
    /// longer stresstest
    /// </summary>
    [TestMethod]
    [Timeout(5000)] // 5 second timeout
    public void Stresstest3_WithSavingAndLoading_IsValid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
    
        spreadsheet.SetContentsOfCell("A1", "1");
        for (int i = 2; i <= 5000; i++)
        {
            spreadsheet.SetContentsOfCell($"A{i}", $"=A{i-1}+1");
        }
        
        spreadsheet.Save("stresstest.json");
        
        Spreadsheet loadedStressTest = new Spreadsheet("stresstest.json");
    
        // Change A1 and verify all update correctly
        loadedStressTest.SetContentsOfCell("A1", "10");
        Assert.AreEqual(5009.0, loadedStressTest.GetCellValue("A5000"));
        File.Delete("stresstest.json");
    }
    
}
