// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// Methods for GUI operation implemented by:
// Brenden Bryner 
// October 22, 2025
// </copyright>

using System.Diagnostics;
using Formula;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Spreadsheets;

namespace GUI.Components.Pages;

/// <summary>
/// Spreadsheet page component
/// </summary>
public partial class SpreadsheetPage
{
    // instance variables
    
    /// <summary>
    /// the preview text for the formula preview box
    /// </summary>
    private string FormulaPreview { get; set; } = string.Empty;
    
    /// <summary>
    /// the boolean that triggers whether or not the formula preview is shown
    /// </summary>
    private bool ShowFormulaPreview { get; set; } = false;
    
    /// <summary>
    /// the error message for the error display
    /// </summary>
    private string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// the actual spreadsheet for the spreadsheet logic
    /// </summary>
    private Spreadsheet Spreadsheet { get; set; } = new Spreadsheet();
    
    /// <summary>
    /// the currently selected row (should automatically be set at the top left
    /// </summary>
    private int SelectedRow { get; set; } = 0;

    /// <summary>
    /// currently selected cell column
    /// </summary>
    private int SelectedCol { get; set; } = 0;

    /// <summary>
    /// the name of the currently selected cell
    /// </summary>
    private string SelectedCellName => $"{Alphabet[SelectedCol]}{SelectedRow + 1}";

    /// <summary>
    /// the value of the selected cell
    /// </summary>
    private string SelectedCellValue { get; set; } = string.Empty;

    /// <summary>
    /// contents in the selected cell
    /// </summary>
    private string _selectedCellContents = string.Empty;
    
    /// <summary>
    /// the actual editable contents of the selected cell and trigger for the formula preview
    /// </summary>
    private string SelectedCellContents
    {
        get => _selectedCellContents;
        set
        {
            if (_selectedCellContents != value)
            {
                _selectedCellContents = value;
                UpdateFormulaPreview(value);
            }
        }
    }
    
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int Rows = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int Cols = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    /// Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";

    /// <summary>
    ///   <para> Gets or sets the data for all the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[Rows, Cols];

    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked(int row, int col)
    {
        // update which cell is selected
        SelectedRow = row;
        SelectedCol = col;

        // get the cell contents from the actual spreadsheet
        object contents = Spreadsheet.GetCellContents(SelectedCellName);
        
        // the backing field used here to stop the formula preview from triggering
        if (contents is Formula.Formula formula)
        {
            _selectedCellContents = "=" + formula.ToString();
        }
        else if (contents is string strContents)
        {
            _selectedCellContents = strContents;
        }
        else if (contents is double dblContents)
        {
            _selectedCellContents = dblContents.ToString();
        }
        else
        {
            _selectedCellContents = "";
        }
    
        // get the value from the spreadsheet
        object value = Spreadsheet.GetCellValue(SelectedCellName);
        SelectedCellValue = value?.ToString() ?? "";
    
        //refreshes the spreasheet
        StateHasChanged();
    }
    
    /// <summary>
    /// Returns the CSS class for a cell based on whether it's selected
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="col">Column index</param>
    /// <returns>CSS class string</returns>
    private string GetCellClass(int row, int col)
    {
        return (row == SelectedRow && col == SelectedCol) ? "selected-cell" : "";
    }
    
    /// <summary>
    /// Handles key presses in the contents text box (mainly Enter key)
    /// </summary>
    /// <param name="args">The keyboard event arguments</param>
    private void HandleContentsKeyPress(KeyboardEventArgs args)
    {
        // pressing enter should add the cell contents from the box
        if (args.Key == "Enter")
        {
            ApplyCellContents();
        }
    }
    
    /// <summary>
    /// Applies the current contents to the selected cell
    /// </summary>
    private void ApplyCellContents()
    {
        try
        {
            // clear the errors and hide the formula preview
            ErrorMessage = "";
            ShowFormulaPreview = false;
        
            // set the cell contents inside of the spreadsheet
            var changedCells = Spreadsheet.SetContentsOfCell(SelectedCellName, _selectedCellContents);
    
            // Update all the cells that changed
            foreach (var cellName in changedCells)
            {
                UpdateCellDisplay(cellName);
            }
    
            // update the display
            SelectedCellValue = Spreadsheet.GetCellValue(SelectedCellName)?.ToString() ?? "";
    
            // refresh
            StateHasChanged();
        }
        // error messages
        catch (CircularException)
        {
            ErrorMessage = "ERROR: Circular dependency detected";
            ShowFormulaPreview = false;
            StateHasChanged();
        }
        catch (FormulaFormatException ex)
        {
            ErrorMessage = $"ERROR: Invalid formula - {ex.Message}";
            ShowFormulaPreview = false;
            StateHasChanged();
        }
        catch (InvalidNameException)
        {
            ErrorMessage = "ERROR: Invalid cell name";
            ShowFormulaPreview = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"ERROR: {ex.Message}";
            ShowFormulaPreview = false;
            StateHasChanged();
        }
    }
    
    /// <summary>
    /// Updates the formula preview as the user types
    /// </summary>
    /// <param name="content">The current content being typed</param>
    private void UpdateFormulaPreview(string content)
    {
        // make sure the cnotent is a formula to update the preview
        if (content.StartsWith("=") && content.Length > 1)
        {
            try
            {
                string formulaString = content.Substring(1);
                Formula.Formula formula = new Formula.Formula(formulaString);
                
                // use the current spreadsheet values
                object result = formula.Evaluate(LookupForPreview);
                
                FormulaPreview = result?.ToString() ?? "Empty";
                ShowFormulaPreview = true;
            }
            // errors inside the formula preview
            catch (FormulaFormatException ex)
            {
                FormulaPreview = $"Invalid formula: {ex.Message}";
                ShowFormulaPreview = true;
            }
            catch (Exception)
            {
                FormulaPreview = "Cannot evaluate - contains undefined cells";
                ShowFormulaPreview = true;
            }
        }
        else
        {
            ShowFormulaPreview = false;
        }
    }
    
    /// <summary>
    /// Helper method for formula evaluation in preview
    /// </summary>
    /// <param name="variable">Cell name to look up</param>
    /// <returns>Cell value as double</returns>
    private double LookupForPreview(string variable)
    {
        try
        {
            string normalizedName = variable.ToUpper();
            object value = Spreadsheet.GetCellValue(normalizedName);
            
            if (value is double d)
                return d;
            else if (value is string s && double.TryParse(s, out double parsed))
                return parsed;
            else
                throw new ArgumentException();
        }
        catch
        {
            throw new ArgumentException($"Cell {variable} not found or not a number");
        }
    }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        // get JSON string from the spreadsheet
        string jsonContent = Spreadsheet.GetSpreadsheetJson();
    
        // download the file
        await JsRuntime.InvokeVoidAsync("downloadFile", FileSaveName, jsonContent);
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
        try
        {
            // get rid of the previous errors
            ErrorMessage = ""; 
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if (eventArgs.FileCount == 1)
            {
                var file = eventArgs.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // Get the file contents
                fileContent = await reader.ReadToEndAsync();

                // Load the spreadsheet from JSON
                Spreadsheet.LoadFromJson(fileContent);
            
                // Clear and update the display for all cells
                ClearDisplay();
                UpdateAllCellsDisplay();
            
                // Reset selection to A1
                SelectedRow = 0;
                SelectedCol = 0;
                
                // Update the selected cell display using backing field
                object contents = Spreadsheet.GetCellContents(SelectedCellName);
                if (contents is Formula.Formula formula)
                {
                    _selectedCellContents = "=" + formula.ToString();
                }
                else if (contents is string strContents)
                {
                    _selectedCellContents = strContents;
                }
                else if (contents is double dblContents)
                {
                    _selectedCellContents = dblContents.ToString();
                }
                else
                {
                    _selectedCellContents = "";
                }
                
                SelectedCellValue = Spreadsheet.GetCellValue(SelectedCellName)?.ToString() ?? "";

                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("an error occurred while loading the file..." + e);
            // Show error to user in error message, not cell value
            ErrorMessage = $"ERROR loading file: {e.Message}";
            StateHasChanged();
        }
    }
    
    /// <summary>
    /// Clears all cells in the display
    /// </summary>
    private void ClearDisplay()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                CellsBackingStore[row, col] = string.Empty;
            }
        }
    }

    /// <summary>
    /// Updates the display for all non-empty cells in the spreadsheet
    /// </summary>
    private void UpdateAllCellsDisplay()
    {
        // Clear the display first
        ClearDisplay();
    
        // Update all non-empty cells
        foreach (var cellName in Spreadsheet.GetNamesOfAllNonemptyCells())
        {
            UpdateCellDisplay(cellName);
        }
    }

    /// <summary>
    /// Updates the display for a single cell based on the spreadsheet's value
    /// </summary>
    /// <param name="cellName">The name of the cell to update (e.g., "A1")</param>
    private void UpdateCellDisplay(string cellName)
    {

        // parse the cell name to get the row and column
        char colLetter = cellName[0];
        // this converts the letter to the number
        int col = colLetter - 'A'; 

        // parse the row number
        string rowStr = cellName.Substring(1);
        if (int.TryParse(rowStr, out int row))
        {
            // make sure the numbers are correct 1 to 0, and 2 to 1... etc
            row--; 

            // should only update if its within the visible grid
            if (row >= 0 && row < Rows && col >= 0 && col < Cols)
            {
                // Get the value from the spreadsheet and display it
                object value = Spreadsheet.GetCellValue(cellName);
                CellsBackingStore[row, col] = value?.ToString() ?? "";
            }
        }
    }
    
    /// <summary>
    /// Clears the error message
    /// </summary>
    private void ClearError()
    {
        ErrorMessage = "";
        StateHasChanged();
    }
    
}