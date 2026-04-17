# Spreadsheet (C#)

This project is an implementation of a simplified spreadsheet system written in C#. It was developed as part of CS 3500 – Software Practice at the University of Utah.

The spreadsheet supports an infinite grid of cells where each cell can contain a string, a double, or a formula. The system focuses on dependency tracking, circular dependency detection, and correct evaluation ordering using a graph-based structure.

---

## Features

- Infinite spreadsheet with named cells
- Cell contents supported:
  - Strings
  - Doubles
  - Formulas
- Formula evaluation with dependency resolution
- Circular dependency detection with exception handling
- Automatic recalculation of dependent cells
- Save and load functionality using JSON
- Case-insensitive cell naming
- Indexer support for direct cell value access

---

## Architecture

The spreadsheet is designed around a dependency-driven evaluation model.

### Core Components

- Spreadsheet
  - Manages cell storage, updates, and overall state
- DependencyGraph
  - Tracks relationships between cells for evaluation ordering and cycle detection
- Formula Engine
  - Evaluates formulas and resolves dependencies dynamically
- Cell Class
  - Stores raw contents and computed values for each cell

---

## Key Design Concepts

- Directed graph for dependency tracking
- Depth-first search for topological evaluation ordering
- Circular dependency detection
- Separation of contents vs computed values
- Recalculation of dependent cells when values change

---

## Cell Model

Each cell contains:

- Contents: Raw input (string, double, or formula)
- Value: Evaluated result of the contents

Rules:
- Empty string represents an empty cell
- Formulas begin with =
- Circular references are not allowed and will throw a CircularException

---

## Persistence

The spreadsheet supports saving and loading via JSON serialization.

### Example Save Format

{
  "Cells": {
    "A1": {
      "StringForm": "5"
    },
    "B1": {
      "StringForm": "=A1+2"
    }
  }
}

---

### Persistence Features

- Save spreadsheet state to a file
- Load spreadsheet from a file
- Load from JSON string directly
- Track whether the spreadsheet has changed using the Changed property

---

## Technologies Used

- C#
- .NET
- System.Text.Json
- Regular Expressions for cell validation
- Graph-based dependency management

---

## Academic Context

This project was created for CS 3500 – Software Practice at the University of Utah.

It was developed as part of coursework focused on software design, data structures, dependency management, and large-scale system implementation in C#.

---

## Academic Integrity Notice

This repository is provided for portfolio and educational purposes only.

The code was originally submitted as coursework at the University of Utah. It is not intended for redistribution or submission for academic credit in any form. Using this code for graded coursework or submitting it as original work may violate academic integrity policies.

This project is shared solely to demonstrate programming ability and software engineering experience.

