// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta, de St. Germain, Martin, Fall 2021, Fall 2024, Fall 2025
//     - Updated return types
//     - Updated documentation
// Rest of methods implemented by:
// Brenden Bryner
// October 22, 2025

using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Spreadsheets;

using Formula;
using DependencyGraph;
using System.Text.Json;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException( string msg )
        : base( msg )
    {
    }
}

/// <summary>
///   <para>
///     A Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case-insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{

    // Instance Variables
    
    /// <summary>
    /// Dependency graph to track dependencies between cells
    /// </summary>
    private DependencyGraph _dependencies;
    /// <summary>
    /// Dictionary to keep track of non-empty cells
    /// </summary>
    private Dictionary<string, Cell> _nonEmptyCells;
    /// <summary>
    /// True if this spreadsheet has been changed since it was
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    public bool Changed { get; private set; }
    
    // Spreadsheet constructors
    
    /// <summary>
    /// Zero argument constructor to create a spreadsheet with a DependencyGraph
    /// to track cell dependencies and a dictionary to track the non empty cells
    /// </summary>
    public Spreadsheet()
    {
        _dependencies = new DependencyGraph();
        _nonEmptyCells = new Dictionary<string, Cell>();
        Changed =  false;
    }
    
    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file referred to by
    /// the given filename.
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        _dependencies = new DependencyGraph();
        _nonEmptyCells = new Dictionary<string, Cell>();
        Changed = false;

        // load the saved file
        try
        {
            // get all the text from the file
            string jsonString = File.ReadAllText(filename);
            
            //deserialize into the dictionaries
            var data =
                JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(
                    jsonString);

            // if the json is missing the cells part there should be a spreadsheetreadwrite exception
            if (data == null || !data.ContainsKey("Cells"))
            {
                throw new SpreadsheetReadWriteException("File format is invalid");
            }

            var cells = data["Cells"];

            // iterate through and set the contents
            foreach (var cellEntry in cells)
            {
                string cellName = cellEntry.Key;

                if (!cellEntry.Value.ContainsKey("StringForm"))
                {
                    throw new SpreadsheetReadWriteException("missing stringform property");
                }

                string content = cellEntry.Value["StringForm"];

                //setcontents to rebuild the spreadsheet
                SetContentsOfCell(cellName, content);
            }

            // loading a file should set the change to false
            Changed = false;
        }
        catch (SpreadsheetReadWriteException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // all the other exceptions should throw read write exception
            throw new SpreadsheetReadWriteException("error reading/writing spreadsheet");
        }
    }
    
    /// <summary>
    ///   Provides a copy of the normalized names of all the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        // make a new copy and return
        return new HashSet<string>(_nonEmptyCells.Keys);
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        // make sure the cell name is valid first
        if (!IsValidCellName(name))
            throw new InvalidNameException();

        // make the name consistent 
        string validName = NormalizeCellName(name);
        
        // get the value of the cell or return an empty string
        if (_nonEmptyCells.TryGetValue(validName, out Cell? cell))
            return cell.CellContents;
        else
            return string.Empty;
    }
    
    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue( string name )
    {
        // check if it is a valid cell name 
        if (!IsValidCellName(name))
            throw new InvalidNameException();

        string validName = NormalizeCellName(name);
          
        // check the cell contents type and return values
        if (_nonEmptyCells.TryGetValue(validName, out Cell cell))
        {
            return cell.CellValue;
        }

        return "";
    }
    
    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
      get { return GetCellValue(name); }
    }

    /// <summary>
    /// Saves this spreadsheet to a file
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file,
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            // { "Cells": ... }
            Dictionary<string, object> mainObject = new Dictionary<string, object>();
            // { "A1": { "StringForm": "5" }, "B2": { "StringForm": "hello" } }
            Dictionary<string, object> cellsDictionary = new Dictionary<string, object>();
        
            // add the cells to the cells dictionary
            foreach (var cellEntry in _nonEmptyCells)
            {
                string cellName = cellEntry.Key;
                Cell cell = cellEntry.Value;
            
                // stringform property
                Dictionary<string, string> cellData = new Dictionary<string, string>();
                cellData["StringForm"] = cell.StringForm;
            
                cellsDictionary[cellName] = cellData;
            }
        
            mainObject["Cells"] = cellsDictionary;

            // json options to write indented and allow characters
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        
            //serialize
            string jsonString = JsonSerializer.Serialize(mainObject, options);
            File.WriteAllText(filename, jsonString);
        
            // reset changed after save
            Changed = false;
        }
        // if anything happens, catch and only throw spreadsheet read write exception
        catch (Exception ex) when (ex is not SpreadsheetReadWriteException)
        {
            throw new SpreadsheetReadWriteException("Error saving spreadsheet");
        }
    } 
    
//todo add documentation
public string GetSpreadsheetJson()
{
    // Create the same structure as Save
    Dictionary<string, object> mainObject = new Dictionary<string, object>();
    Dictionary<string, object> cellsDictionary = new Dictionary<string, object>();

    foreach (var cellEntry in _nonEmptyCells)
    {
        string cellName = cellEntry.Key;
        Cell cell = cellEntry.Value;
        
        Dictionary<string, string> cellData = new Dictionary<string, string>();
        cellData["StringForm"] = cell.StringForm;
        
        cellsDictionary[cellName] = cellData;
    }

    mainObject["Cells"] = cellsDictionary;

    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    return JsonSerializer.Serialize(mainObject, options);
}

/// <summary>
/// Replaces the current spreadsheet with one loaded from a JSON string
/// </summary>
/// <param name="jsonString">JSON string representing a spreadsheet</param>
/// <exception cref="SpreadsheetReadWriteException">If the JSON is invalid</exception>
public void LoadFromJson(string jsonString)
{
    try
    {
        // Clear the current spreadsheet
        _nonEmptyCells.Clear();
        _dependencies = new DependencyGraph();
        
        // Deserialize the JSON
        var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(jsonString);

        if (data == null || !data.ContainsKey("Cells"))
        {
            throw new SpreadsheetReadWriteException("Invalid file format: missing 'Cells' property");
        }

        var cells = data["Cells"];

        // Rebuild the spreadsheet
        foreach (var cellEntry in cells)
        {
            string cellName = cellEntry.Key;

            if (!cellEntry.Value.ContainsKey("StringForm"))
            {
                throw new SpreadsheetReadWriteException($"Invalid file format: cell {cellName} missing 'StringForm' property");
            }

            string content = cellEntry.Value["StringForm"];
            SetContentsOfCell(cellName, content);
        }

        Changed = false;
    }
    catch (SpreadsheetReadWriteException)
    {
        throw;
    }
    catch (Exception ex)
    {
        throw new SpreadsheetReadWriteException($"Error loading spreadsheet: {ex.Message}");
    }
}
    
    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names
    ///     of all other cells whose value depends, directly or indirectly,
    ///     on the named cell. The order of the list should be any order
    ///     such that if cells are re-evaluated in that order, their dependencies
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    { 
        // check if it is a valid cell name 
        if (!IsValidCellName(name))
            throw new InvalidNameException();

        // normalize the cell name
        string validName = NormalizeCellName(name);

        IList<string> changedCells;

        // parses as a double
        if (double.TryParse(content, out double number))
        {
            changedCells = SetCellContents(validName, number);
        }
        // if it is a formula
        else if (content.StartsWith("="))
        {
            string formulaString = content.Substring(1);
            Formula formula = new Formula(formulaString);
            changedCells = SetCellContents(validName, formula);
        }
        else
        {
            // string
            changedCells = SetCellContents(validName, content);
        }

        //recalculate values for all affected cells
        RecalculateCellValues(changedCells);

        //setting the spreadsheet contents changes it
        Changed = true;
        return changedCells;
    }
        
    // --- helper methods for the set contents of cell ---

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all the cells, i.e., if you re-evaluate each cell in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        
        // get rid of dependees if changing from formula
        _dependencies.ReplaceDependees(name, []);
        
        // Set the cell 
        _nonEmptyCells[name] = new Cell(number);

        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        // get rid of dependencies if changing from formula
        _dependencies.ReplaceDependees(name, []);
        
        // if the text entered is empty the cell should be turned into an empty cell
        if (text == "")
        {
            // Remove the cell if it exists
            if (_nonEmptyCells.ContainsKey(name))
            {
                _nonEmptyCells.Remove(name);
            }
        }
        else
        {
            // Set the cell
            _nonEmptyCells[name] = new Cell(text);
        }
        
        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // save this in case the exception happens
        var oldDependees = _dependencies.GetDependees(name).ToList();
        
        // make sure the dependencies get updated
        _dependencies.ReplaceDependees(name, formula.GetVariables());

        try
        {
            // Check for circular dependencies
            var cellsToRecalculate = GetCellsToRecalculate(name).ToList();
            
            // make sure the new cell is put in the nonempty cell
            _nonEmptyCells[name] = new Cell(formula);
            
            return cellsToRecalculate;
        }
        catch (CircularException)
        {
            // make sure no changes were made to the spreadsheet if the CircularException happens
            _dependencies.ReplaceDependees(name, oldDependees);
            throw;
        }
    }
    
    // --- other helper methods ---

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        // use get dependents from the dependency graph class
        return _dependencies.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    /// A helper for the GetCellsToRecalculate method.
    ///
    /// Depth First Search that topologically sorts a DependencyGraph 
    /// </summary>
    /// <param name="start">name of the original cell that was changed</param>
    /// <param name="name">name of the current cell being visited</param>
    /// <param name="visited">list of cells that have been visited in the search</param>
    /// <param name="changed">linked list for cells that get changed</param>
    /// <exception cref="CircularException">throws if there is circular logic in the dependency graph</exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        // mark current cell as visited 
        visited.Add(name);
        // go through the depedents of the cell 
        foreach (string n in GetDirectDependents(name))
        {
            // if any dependent equals the current cell there will be a circular exception
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            // if the dependent hasnt been visited, then call this method again
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        // add to the cell to the front of the list
        changed.AddFirst(name);
    }
    
        /// <summary>
    /// Recalculates the value of a cell and all dependent cells
    /// </summary>
    private void RecalculateCellValues(IList<string> cellsToRecalculate)
    {
        foreach (string cellName in cellsToRecalculate)
        {
            if (_nonEmptyCells.TryGetValue(cellName, out Cell cell))
            {
                cell.CellValue = CalculateCellValue(cell);
            }
        }
    }

    /// <summary>
    /// Calculates the value of a single cell
    /// </summary>
    private object CalculateCellValue(Cell cell)
    {
        if (cell.CellContents is double d)
            return d;
        if (cell.CellContents is string s)
            return s;
        
        return ((Formula)cell.CellContents).Evaluate(LookupCellValue);
    }

    /// <summary>
    /// Lookup for formula evaluation and returns the value of a cell as a double
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">if cell does not exist or is not a double</exception>
    private double LookupCellValue(string variable)
    {
        string normalizedName = NormalizeCellName(variable);
    
        if (_nonEmptyCells.TryGetValue(normalizedName, out Cell cell))
        {
            if (cell.CellValue is double d)
                return d;
            else
                throw new ArgumentException();
        }
        else
        {
            throw new ArgumentException();
        }
    }
    
    // --- Cell class and logic ---
    
    /// <summary>
    /// Cell class to represent a cell inside of a spreadsheet. A cell can have a string, formula, or double
    /// </summary>
    private class Cell
    {
        // instance variables
        
        /// <summary>
        /// get or set the contents of a cell
        /// </summary>
        public object CellContents { get; set; }
        /// <summary>
        /// the value of the cell
        /// </summary>
        public object CellValue { get; set; }

        /// <summary>
        /// string form of the cell value to help with json
        /// </summary>
        public string StringForm
        {
            get
            {
                if (CellContents is double d)
                    return d.ToString();
                else if (CellContents is string s)
                    return s;
                else // Must be Formula (the only other possibility)
                    return "=" + ((Formula)CellContents).ToString();
            }
        }

        // constructor
        
        /// <summary>
        /// Cell constructor to make a cell with contents that will be either a formula, double, or string
        /// </summary>
        /// <param name="cellContents">the contents that are being set in the cell</param>
        public Cell(object cellContents)
        {
            CellContents = cellContents;
            
            // put cell values in the cell so calculation dont have to happen all the time
            if (cellContents is Formula)
            {
                // return this or calculate the formula first
                CellValue = new FormulaError("Not evaluated");
            }
            else
            {
                // the strings and doubles should just be set as the value
                CellValue = cellContents;
            }
        }
    }

    /// <summary>
    /// make sure that the cell names are actual cell names
    /// </summary>
    /// <param name="name">name of the valid or invalid cell name</param>
    /// <returns>true if the cell name is valid and false if not</returns>
    private static bool IsValidCellName(string? name)
    {
        // null cell names should not work
        if (name is null)
            return false;
        // make sure cell names are letters followed by numbers
        return Regex.IsMatch(name, @"^[a-zA-Z]+\d+$");
    }

    /// <summary>
    /// makes sure that the cell names are all capitalized 
    /// </summary>
    /// <param name="name">name of the cell name that is being capitalized</param>
    /// <returns>a normalized cell name</returns>
    private static string NormalizeCellName(string name)
    {
        return name.ToUpper();
    }
    
}