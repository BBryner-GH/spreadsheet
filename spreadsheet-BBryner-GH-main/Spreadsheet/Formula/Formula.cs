// <summary>
//   <para>
//     This code is provided to start your assignment.  It was written
//     by Profs Joe, Danny, Jim, and Travis.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with any other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// <authors> Brenden Bryner </authors>
// <date> September 17, 2025 </date>
// </summary>

using System.Text;

namespace Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one or more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    /// <summary>
    /// List of all token in a formula
    /// </summary>
    private List<string> _tokens;
    /// <summary>
    /// Canonical form of a formula (all lowercase variables get capitalized and spaces are removed)
    /// </summary>
    private readonly string _canonicalFormString;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non-Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // setup before iteration
        _tokens = GetTokens(formula);
        int openParenthesisCount = 0;
        int closeParenthesisCount = 0;
        string prevToken = "";
        bool firstToken = true;
        StringBuilder stringBuilder = new StringBuilder();


        // rule checks outside of loop
        // See if tokens list is empty (breaks one token rule if so)
        if (_tokens.Count == 0)
            throw new FormulaFormatException("The formula needs one or more tokens.");

        //check first and last token rule
        CheckIfValidFirstToken(_tokens);
        CheckIfValidLastToken(_tokens);

        // for each through the tokens and see if it violates any rules
        foreach (string token in _tokens)
        {
            // valid token checks
            if (!CheckIfValidToken(token))
                throw new FormulaFormatException("This token is invalid: " + token);
            
            // building a string representation of the formula for the ToString method
            if (IsNumber(token))
                stringBuilder.Append(double.Parse(token).ToString());
            else if (IsVar(token))
                stringBuilder.Append(token.ToUpper());
            else
                stringBuilder.Append(token);

            // following checks
            if (!firstToken)
                CheckIfValidFollowingToken(prevToken, token);

            // parenthesis counting and checks
            if (IsOpeningParenthesis(token))
                openParenthesisCount++;

            if (IsClosingParenthesis(token))
                closeParenthesisCount++;

            if (closeParenthesisCount > openParenthesisCount)
                throw new FormulaFormatException(
                    "From left to right the number of closing parentheses can't be greater than the number of opening parenthesis");

            prevToken = token;
            firstToken = false;
        }
        
        if (closeParenthesisCount != openParenthesisCount)
            throw new FormulaFormatException("The number of opening and closing parenthesis should be the same");
        
        _canonicalFormString = stringBuilder.ToString();
        
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///     Variables should be returned in canonical form, having all letters converted
    ///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> variables = new HashSet<string>();
        
        foreach (string token in _tokens)
        {
            if (IsVar(token))
                variables.Add(token.ToUpper());
        }

        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All the variable and number tokens in the string will be normalized.
    ///     For numbers, this means that the original string token is converted to
    ///     a number using double.Parse or double.TryParse, then converted back to a
    ///     string using double.ToString.
    ///     For variables, this means all letters are uppercased.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + Y1").ToString() should return "X1+Y1"
    ///       new("x1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This method should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        // only return the string for constant runtime behavior
        return _canonicalFormString;
    }
    
    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==( Formula f1, Formula f2 )
    {
        return f1._canonicalFormString.Equals(f2._canonicalFormString);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=( Formula f1, Formula f2 )
    {
        return !(f1 == f2);
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals( object? obj )
    {
        if (obj is not  Formula formula)
            return false;

        return this._canonicalFormString == formula._canonicalFormString;
    }
    
    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be miniscule.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode( )
    {
        return _canonicalFormString.GetHashCode();
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate( Lookup lookup )
    {
        Stack<double> valueStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();
        
        // processing the tokens from left to right
        foreach (string token in _tokens)
        {
            // If token is a number
            if (IsNumber(token))
            {
                double number = double.Parse(token);
                if (!ProcessNum(number, valueStack, operatorStack))
                    return new FormulaError("Division by zero error");
            }
            // if token is a variable
            else if (IsVar(token))
            {
                try
                {
                    // proceed as above with the lookup value
                    double number = lookup(token.ToUpper());
                    if (!ProcessNum(number, valueStack, operatorStack))
                        return new FormulaError("Division by zero error");
                }
                catch (ArgumentException)
                {
                    return new FormulaError("The variable has no value");
                }
            }
            // if token is + or -
            else if (token == "+" || token == "-")
            {
                ProcessAddSub(token, valueStack, operatorStack);
            }
            // token is * or /
            else if (token == "*" || token == "/")
            {
                operatorStack.Push(token);
            }
            // token is a left parenthesis
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            // token is a right parenthesis
            else if (token == ")")
            {
                if (!ProcessClosingParenthesis(valueStack, operatorStack))
                    return new FormulaError("Division by zero error");
            }
        }
        return ProcessFinalEvaluation(valueStack, operatorStack);
    }
    
    // --- HELPER METHODS BELOW ---
    
    // HELPER METHODS FOR EVALUATE METHOD
    
    /// <summary>
    /// Helper method to process numbers in evaluation method
    /// </summary>
    /// <param name="number"></param>
    /// <param name="valueStack"></param>
    /// <param name="operatorStack"></param>
    /// <returns></returns>
    private bool ProcessNum(double number, Stack<double> valueStack, Stack<string> operatorStack)
    {
        // if the * or / is at the top of the operator stack, pop both stacks and apply popped operator to value and token
        if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/"))
        {
            // get the operators and values from the stacks
            string op = operatorStack.Pop();
            double value = valueStack.Pop();
            
            // check for dividing by zero
            if (op == "/" && number == 0)
                return false;

            // apply the operator to the numbers
            double result;
            if (op == "*")
            {
                result = value * number;
            }
            else
            {
                result = value / number;
            }
            // push the result onto the stack
            valueStack.Push(result);
        }
        else
        {
            // pushing the number to the stack if there were not any operators
            valueStack.Push(number);
        }

        return true;
    }

    /// <summary>
    /// Helper method for evaluating add and subtraction
    /// </summary>
    /// <param name="token"></param>
    /// <param name="valueStack"></param>
    /// <param name="operatorStack"></param>
    private void ProcessAddSub(string token, Stack<double> valueStack, Stack<string> operatorStack)
    {
        if (operatorStack.Count > 0 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
        {
            // pop value stack twice and operator stack and apply operator to numbers
            string op = operatorStack.Pop();
            double secondValue = valueStack.Pop();
            double firstValue = valueStack.Pop();
            
            double result;
            if (op == "+")
            {
                result = firstValue + secondValue;  
            }
            else
            {
                result = firstValue - secondValue;
            }
            valueStack.Push(result);

        }
        // push token onto the operator stack
        operatorStack.Push(token);
    }

    /// <summary>
    /// Helper method for evaluating closing parenthesis
    /// </summary>
    /// <param name="valueStack"></param>
    /// <param name="operatorStack"></param>
    /// <returns></returns>
    private bool ProcessClosingParenthesis(Stack<double> valueStack, Stack<string> operatorStack)
    {
        if (operatorStack.Count > 0 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
        {
            string op = operatorStack.Pop();
            double secondValue = valueStack.Pop();
            double firstValue = valueStack.Pop();

            double result;
            if (op == "+")
            {
                result = firstValue + secondValue;
            }
            else
            {
                result = firstValue - secondValue;
            }
            valueStack.Push(result);
        }
        
        // top of the operator stack will be (
        operatorStack.Pop();

        if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/"))
        {
            string op = operatorStack.Pop();
            double secondValue = valueStack.Pop();
            double firstValue = valueStack.Pop();

            if (op == "/" && secondValue == 0)
                return false;
            
            double result;
            if (op == "*")
            {
                result = firstValue * secondValue;
            }
            else
            {
                result = firstValue / secondValue;
            }
            valueStack.Push(result);
        }
        
        return true;
    }

    /// <summary>
    /// Helper method for evaluate method for the last calculation on the stack
    /// </summary>
    /// <param name="valueStack"></param>
    /// <param name="operatorStack"></param>
    /// <returns></returns>
    private object ProcessFinalEvaluation(Stack<double> valueStack, Stack<string> operatorStack)
    {
        // Condition: Operator stack is empty
        if (operatorStack.Count == 0)
        {
            // value stack will contain a single number
            return valueStack.Pop();
        }
        // Condition: Operator stack is not empty
        else
        {
            string op = operatorStack.Pop();
            double secondValue = valueStack.Pop();
            double firstValue = valueStack.Pop();

            double result;
            if (op == "+")
            {
                result = firstValue + secondValue;
            }
            else
            {
                result = firstValue - secondValue;
            }

            return result;
        }
    }
    
    //private helper methods for the Formula Constructor ---
    
    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
            "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
            lpPattern,
            rpPattern,
            opPattern,
            VariableRegExPattern,
            doublePattern,
            spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
    
    /// <summary>
    /// Checks if the first token in a formula is valid
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <exception cref="FormulaFormatException">Throws if the first token in a formula is not valid</exception>
    private void CheckIfValidFirstToken(List<string> tokens)
    {
        //first token must be a number, variable, or opening parenthesis
        string firstToken = tokens[0];
        if (!IsNumber(firstToken) && !IsVar(firstToken) &&
            !IsOpeningParenthesis(firstToken))
        {
            throw new FormulaFormatException(
                "The first token must be a valid number, valid variable, or opening parenthesis");
        }
    }

    /// <summary>
    /// Checks if the last token in a formula is valid (number, variable, or closing parenthesis).
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <exception cref="FormulaFormatException">Throws if the last token is not valid</exception>
    private void CheckIfValidLastToken(List<string> tokens)
    {
        //last token must be a number, variable, or a closing parenthesis
        string lastToken = tokens[tokens.Count - 1];
        if (!IsNumber(lastToken) && !IsVar(lastToken) &&
            !IsClosingParenthesis(lastToken))
        {
            throw new FormulaFormatException(
                "The last token must be a valid number, variable, or closing parenthesis.");
        }
    }

    /// <summary>
    /// Checks if a token is valid (has to be a parenthesis, number, variable, or operator.)
    /// </summary>
    /// <param name="token">A provided token</param>
    /// <returns>True if a token is valid, false if not.</returns>
    private bool CheckIfValidToken(string token)
    {
        return IsOperator(token)
               || IsOpeningParenthesis(token)
               || IsClosingParenthesis(token)
               || IsNumber(token)
               || IsVar(token);
    }
    
    /// <summary>
    /// Determines if a token following another follows the rules in PS1
    /// </summary>
    /// <param name="prevToken">Previous token</param>
    /// <param name="token">token that is following a previous token</param>
    /// <returns>True if a following token is valid, false if otherwise</returns>
    /// <exception cref="FormulaFormatException">Exception is thrown if a following token breaks the rules of PS1</exception>
    private bool CheckIfValidFollowingToken(string prevToken, string token)
    {
        // check parenthesis/operator following rule
        if (IsOpeningParenthesis(prevToken) || IsOperator(prevToken))
        {
            if (!(IsNumber(token) || IsVar(token) || IsOpeningParenthesis(token)))
                throw new FormulaFormatException(
                    "The parenthesis or operator must be followed by a number, variable, or opening parenthesis");
        }

        // check extra following rule
        if (IsNumber(prevToken) || IsVar(prevToken) || IsClosingParenthesis(prevToken))
        {
            if (!(IsOperator(token) || IsClosingParenthesis(token)))
                throw new FormulaFormatException(
                    "The number, variable, or closing parenthesis must be followed by an operator or a closing parenthesis");
        }
        
        return true;
    }

    /// <summary>
    /// Checks whether a token is a number of not
    /// </summary>
    /// <param name="token">A token that is possibly a number.</param>
    /// <returns>True if a token is a number and false if otherwise.</returns>
    private bool IsNumber(string token)
    {
        return double.TryParse(token, out _);
    }

    /// <summary>
    /// Checks whether a token is an open parenthesis or not.
    /// </summary>
    /// <param name="token">Token that might be an opening parenthesis</param>
    /// <returns>True if a token is an opening parenthesis and false if otherwise</returns>
    private bool IsOpeningParenthesis(string token)
    {
        return (token == "(");
    }
    
    /// <summary>
    /// Checks whether a token is a closing parenthesis or not
    /// </summary>
    /// <param name="token">A token that could be a closing parenthesis</param>
    /// <returns>True if a token is a closing parenthesis and false if otherwise.</returns>
    private bool IsClosingParenthesis(string token)
    {
        return (token == ")");
    }

    /// <summary>
    /// Checks whether a token is a valid operator or not ( valid operators being +,-,*,/
    /// </summary>
    /// <param name="token">A token that might be an operator</param>
    /// <returns>True if provided token is an operator, false if otherwise.</returns>
    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }
    
}

//  --- EXTRA PROVIDED CLASSES/METHODS ----

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}

// FormulaError Class

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError( string message )
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup( string variableName );