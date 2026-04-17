// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Brenden Bryner </authors>
// <date> September 17, 2025 </date>

namespace FormulaTests;

using Formula; 

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// <para>
///     The following class also contains tests for the formula constructor based off of
///     syntax rules provided in the PS1 assignment on canvas.
/// </para>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- TESTS FOR CONSTRUCTOR (PS2 TESTS) ---
    
    // --- Tests for One Token Rule --- (There must be at least one token)

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNoTokens_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula(string.Empty));
    }

    /// <summary>
    /// Makes sure that the right kind of exception is thrown when trying to make a formula with
    /// no tokens and just whitespace.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNoTokensWhiteSpace_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("  "));
    }

    /// <summary>
    /// Testing to see if a formula with just one token will be valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneToken_Valid()
    {
        _ = new Formula("1");
    }

    // --- Tests for Valid Token Rule ---(The only tokens in the expressions are (,),+,-,*,/,variables, and numbers.)

    /// <summary>
    /// Testing to see if the correct exception is thrown if there is an invalid token.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestInvalidToken_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("5+%2"));
    }

    /// <summary>
    /// Testing to see if the parenthesis token works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisToken_Valid()
    {
        _ = new Formula("(1)");
    }

    /// <summary>
    /// Testing to see if the plus token works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestPlusToken_Valid()
    {
        _ = new Formula("1 + 2");
    }

    /// <summary>
    /// testing to see if the minus token works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestMinusToken_Valid()
    {
        _ = new Formula("1 - 1");
    }

    /// <summary>
    /// testing to see if the * token works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestMultiplyToken_Valid()
    {
        _ = new Formula("1 * 2");
    }

    /// <summary>
    /// testing to see if the / token works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestDivideToken_Valid()
    {
        _ = new Formula("1/2");
    }

    /// <summary>
    /// Testing to see if multiple tokens work.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestMultipleTokens_Valid()
    {
        _ = new Formula("1+1-1*2/8*A1+2e5");
    }

    /// <summary>
    /// testing that the formula constructor will recognize the e exponent
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotation_Valid()
    {
        _ =  new Formula("2e5");
    }

    /// <summary>
    /// testing that the formula constructor will recognize the capitalized E exponent
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationCapitalized_Valid()
    {
        _ =  new Formula("2E5");
    }

    // --- Tests for Closing Parenthesis Rule (When reading tokens from left to right, at no point should the number of closing
    // parenthesis seen so far be greater than the number of opening parenthesis seen so far.)

    /// <summary>
    /// testing a valid case for the closing parenthesis rule.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesis_Valid()
    {
        _ = new Formula("(1+1)");
    }

    /// <summary>
    /// testing to see if breaking the closing parenthesis rule will use the right throw.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisGreaterThrows_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("1+1)"));
    }

    // --- Tests for Balanced Parentheses Rule (The total number of opening parentheses
    // must equal the number of closing parentheses)

    /// <summary>
    /// testing to see if the valid case for the balanced parenthesis rule works.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestBalancedParentheses_Valid()
    {
        _ = new Formula("((1+1))+(1+2)");
    }

    /// <summary>
    /// Testing to see if an unbalanced amount of parenthesis will throw the right exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesisThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("((1+2)"));
    }

    // --- Tests for First Token Rule (The first token of an expression must be a number, variable, or an opening parenthesis)

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    /// Testing to see if a variable as the first token will be valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("A1");
    }

    /// <summary>
    /// testing to see if the first token being an open parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOpeningParenthesis_Valid()
    {
        _ = new Formula("(1)");
    }

    /// <summary>
    /// testing to see if an exception is thrown if the first token is invalid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("+"));
    }

    /// <summary>
    /// make sure that the first exponent being in scientific notation works
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenScientificNotation_Valid()
    {
        _ = new Formula("1e9");
    }

    // --- Tests for  Last Token Rule --- (The last token of an expression must be a number, a variable, or a closing parenthesis)

    /// <summary>
    /// testing to see if the last token being a number is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("2+1");
    }

    /// <summary>
    /// testing to see if the last token being a variable is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("2+A1");
    }

    /// <summary>
    /// testing to see if the last token being a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenClosingParenthesis_Valid()
    {
        _ = new Formula("(2)");
    }

    /// <summary>
    /// Testing to see if an invalid token at the end will throw the right exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("2+"));
    }

    /// <summary>
    /// testing to make sure that the last token can be in scientific notation
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenScientificNotation_Valid()
    {
        _ = new Formula("90+1e9");
    }

    // --- Tests for Parentheses/Operator Following Rule --- (Any token that immediately follows an opening parenthesis
    // or an operator must be either a number, a variable, or an opening parenthesis)

    /// <summary>
    /// testing to see if a number following an opening parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisNumberFollowing_Valid()
    {
        _ = new Formula("(2)");
    }

    /// <summary>
    /// testing to see if a variable following an opening parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisVariableFollowing_Valid()
    {
        _ = new Formula("(A1)");
    }

    /// <summary>
    /// testing to see if an opening parenthesis following a parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowedByOpeningParenthesis_Valid()
    {
        _ = new Formula("((2))");
    }

    /// <summary>
    /// testing to see if scientific notation can follow an opening parenthesis
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowedByScientificNotation_Valid()
    {
        _ = new Formula("(9e6)");
    }

    /// <summary>
    /// testing to see if a number following an operator is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByNumber_Valid()
    {
        _ = new Formula("2+2");
    }

    /// <summary>
    /// testing to see if an operator followed by a variable is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByVariable_Valid()
    {
        _ = new Formula("2*A1");
    }

    /// <summary>
    /// testing to see if an operator followed by an opening parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByOpeningParenthesis_Valid()
    {
        _ = new Formula("2-(2)");
    }

    /// <summary>
    /// testing to see if an operator following an opening parenthesis will throw the right exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowingThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("(-"));
    }

    /// <summary>
    /// testing to see if an operator following an operator will throw the right exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowingThrow_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("-+"));
    }

    /// <summary>
    /// testing to see if scientific notation can follow an operator
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByScientificNotation_Valid()
    {
        _ = new Formula("1*7e9");
    }

    /// <summary>
    /// see if a parenthesis followed by an operator will throw
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowedByOperatorThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("(*5)"));
    }

    // --- Tests for Extra Following Rule --- (Any token that immediately follows a number, a variable, or a closing parenthesis
    // must be either an operator or a closing parenthesis.)

    /// <summary>
    /// testing to see if a number followed by an operator will be valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNumberFollowedByOperator_Valid()
    {
        _ = new Formula("2*9");
    }

    /// <summary>
    /// testing to see if a variable followed by an operator is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariableFollowedByOperator_Valid()
    {
        _ = new Formula("A1*9");
    }

    /// <summary>
    /// testing to see if a closing parenthesis followed by an operator is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisFollowedByOperator_Valid()
    {
        _ = new Formula("(1)-2");
    }

    /// <summary>
    /// testing to see if a number followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNumberFollowedByClosingParenthesis_Valid()
    {
        _ = new Formula("(2)");
    }

    /// <summary>
    /// testing to see if a variable followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariableFollowedByClosingParenthesis_Valid()
    {
        _ = new Formula("(A1)");
    }

    /// <summary>
    /// testing to see if a closing parenthesis followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisFollowedByClosingParenthesis_Valid()
    {
        _ = new Formula("((A1))");
    }

    /// <summary>
    /// testing to see if a number following a number will throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNumberFollowingThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("2 3"));
    }

    /// <summary>
    /// testing to see if a variable following a variable will throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariableFollowingThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("A1 C1"));
    }

    /// <summary>
    /// testing to see if a non operator or closing parenthesis following a closing parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisFollowingThrow_ThrowsException()
    {
        Assert.Throws<FormulaFormatException>(() => new Formula("(2)3"));
    }
    
    // --- TESTS FOR GetVariables METHOD ---
    
    /// <summary>
    /// basic case for GetVariables
    /// </summary>
    [TestMethod]
    public void GetVariables_TestBasicCase_Valid()
    {
        Formula formula = new Formula("A1");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1"}, variables.ToList());
    }

    /// <summary>
    /// test to see if getvariables works with multiple variables
    /// </summary>
    [TestMethod]
    public void GetVariables_TestMultipleVariables_Valid()
    {
        Formula formula = new Formula("A1+A2+B3");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1", "A2", "B3"}, variables.ToList());
    }

    /// <summary>
    /// testing to see if getvariables will capitalize a lowercase variable
    /// </summary>
    [TestMethod]
    public void GetVariables_TestLowerCaseVariable_Valid()
    {
        Formula formula = new Formula("a1");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1"}, variables.ToList());
    }

    /// <summary>
    /// testing to see if multiple lowercase variables will be canonized
    /// </summary>
    [TestMethod]
    public void GetVariables_TestMultipleLowerCase_Valid()
    {
        Formula formula = new Formula("a1+a2+b3");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1", "A2", "B3"}, variables.ToList());
    }

    /// <summary>
    /// testing to see if mixed case variables will work
    /// </summary>
    [TestMethod]
    public void GetVariables_TestMixedCases_Valid()
    {
        Formula formula = new Formula("A1+b2+C3+d4");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1", "B2", "C3", "D4"}, variables.ToList());
    }

    /// <summary>
    /// test to see if getVariables breaks if there is no variables
    /// </summary>
    [TestMethod]
    public void GetVariables_TestNoVariables_Valid()
    {
        Formula formula = new Formula("1+50/20");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string>() , variables.ToList());
    }

    /// <summary>
    /// testing to see if GetVariables can differentiate between variables and the exponent
    /// </summary>
    [TestMethod]
    public void GetVariables_TestScientificNotation_Valid()
    {
        Formula formula = new Formula("a1+b2+5e2");
        ISet<string> variables = formula.GetVariables();
        CollectionAssert.AreEqual(new List<string> {"A1", "B2"}, variables.ToList());
    }
    
    // --- TESTS FOR ToString METHOD ---

    /// <summary>
    /// test basic case for toString
    /// </summary>
    [TestMethod]
    public void ToString_TestBasicCase_Valid()
    {
        Formula formula = new Formula("A1+5");
        Assert.AreEqual("A1+5", formula.ToString());
    }

    /// <summary>
    /// test to see if the decimal is canonized
    /// </summary>
    [TestMethod]
    public void ToString_TestWithDecimal_Valid()
    {
        Formula formula = new Formula("A1+5.0");
        Assert.AreEqual("A1+5", formula.ToString());
    }

    /// <summary>
    /// test to see if scientific notation is canonized
    /// </summary>
    [TestMethod]
    public void ToString_TestWithScientificNotation_Valid()
    {
        Formula formula = new Formula("A1*5e2");
        Assert.AreEqual("A1*500", formula.ToString());
    }

    /// <summary>
    /// testing to see if the lowercase variables get capitalized
    /// </summary>
    [TestMethod]
    public void ToString_TestLowerCaseVariableCanonized_Valid()
    {
        Formula formula = new Formula("a1+b2*b3*d4");
        Assert.AreEqual("A1+B2*B3*D4", formula.ToString());
    }
    
    /// <summary>
    /// testing to see if every valid token type works
    /// </summary>
    [TestMethod]
    public void ToString_TestAllTokens_Valid()
    {
        Formula formula = new Formula("(30+A1)*b2/20-30");
        Assert.AreEqual("(30+A1)*B2/20-30", formula.ToString());
    }

    /// <summary>
    /// testing to see if spaces are removed.
    /// </summary>
    [TestMethod]
    public void ToString_TestFormulaWithSpaces_Valid()
    {
        Formula formula = new Formula("A30  +  20  - b2");
        Assert.AreEqual("A30+20-B2", formula.ToString());
    }
    
    // --- Tests For Evaluation and (PS4 TESTS) ---
    
    // Testing Operator ==

    /// <summary>
    /// basic case of the == operator
    /// </summary>
    [TestMethod]
    public void EqualOperator_TestEqualBasicCase_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula1 ==  formula2);
    }

    /// <summary>
    /// another basic case for == but just making sure it works in reverse
    /// </summary>
    [TestMethod]
    public void EqualOperator_TestEqualBasicCaseReverse_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula2 ==  formula1);
    }

    /// <summary>
    /// should have the same rule as the equal operator, the formulas should be equal when they get normalized after being made into a formula
    /// </summary>
    [TestMethod]
    public void EqualOperator_TestEqualCanonized_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5");
        Assert.IsTrue(formula1 ==  formula2);
    }

    /// <summary>
    /// the numbers should be turned into their canon forms and return equal formulas
    /// </summary>
    [TestMethod]
    public void EqualOperator_TestEqualNormalizedNumbers_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5.00000");
        Assert.IsTrue(formula1 ==  formula2);
    }

    [TestMethod]
    public void EqualOperator_TestDifferentFormulas_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("B3+4");
        Assert.IsFalse(formula1 ==  formula2);
    }



    [TestMethod]
    public void EqualityOperator_TestNonFormulaObject_False()
    {
        Formula formula1 = new Formula("A1+5");
        Assert.AreNotEqual(new object(), formula1);
    }

    [TestMethod]
    public void EqualityOperator_TestNonFormulaObjectLeftSide_False()
    {
        Formula formula1 = new Formula("A1+5");
        Assert.AreNotEqual(formula1, new object());
    }
    
    // Testing Operator !=

    /// <summary>
    /// Testing basic case of inequality operator
    /// </summary>
    [TestMethod]
    public void InequalityOperator_BasicInequalityCheck_True()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("b4+23");
        Assert.IsTrue(formula1 != formula2);
    }

    /// <summary>
    /// testing inequality operator on same formulas
    /// </summary>
    [TestMethod]
    public void InequalityOperator_BasicInequalityCheckOnSame_False()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsFalse(formula1 != formula2);
    }

    /// <summary>
    /// testing inequality operator on formulas that will be the same after normalization
    /// </summary>
    [TestMethod]
    public void InequalityOperator_InequalityCheckOnDifferentCanon_False()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5.0000");
        Assert.IsFalse(formula1 != formula2);
    }

    /// <summary>
    /// Testing non formula objects on the left side
    /// </summary>
    [TestMethod]
    public void InequalityOperator_InequalityCheckNonFormulaLeftSide_True()
    {
        Formula formula1 = new Formula("A1+5");
        Assert.AreNotEqual(formula1, new object());
    }

    /// <summary>
    /// testing non formula objects on the right side
    /// </summary>
    [TestMethod]
    public void InequalityOperator_InequalityCheckNonFormulaRightSide_False()
    {
        Formula formula1 = new Formula("A1+5");
        Assert.AreNotEqual(new object(), formula1);
    }
    
    // Testing Equals

    /// <summary>
    /// basic case for equals method
    /// </summary>
    [TestMethod]
    public void Equals_TestEqualsBasicCase_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula1.Equals(formula2));
    }

    /// <summary>
    /// same case but seeing if it works in reverse
    /// </summary>
    [TestMethod]
    public void Equals_TestEqualsBasicCaseReverse_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula2.Equals(formula1));
    }

    /// <summary>
    /// Should return true even though the canonical form starts different, it should be normalized before calling equals.
    /// </summary>
    [TestMethod]
    public void Equals_TestCanonicalForm_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+ 5");
        Assert.IsTrue(formula1.Equals(formula2));
    }

    /// <summary>
    /// another test case for seeing if the normalized formulas return equal
    /// </summary>
    [TestMethod]
    public void Equals_TestCanonicalFormNumbers_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5.0000");
        Assert.IsTrue(formula1.Equals(formula2));
    }
    
    /// <summary>
    /// Testing a basic false case 
    /// </summary>
    [TestMethod]
    public void Equals_TestFalseCase_Valid()
    {
        Formula formula1 = new Formula("B3+4");
        Formula formula2 = new Formula("A5+4");
        Assert.IsFalse(formula1.Equals(formula2));
    }

    /// <summary>
    /// should return false if there is a null parameter
    /// </summary>
    [TestMethod]
    public void Equals_NullParameter_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Assert.IsFalse(formula1.Equals(null));
    }

    [TestMethod]
    public void Equals_EqualsNullObject_False()
    {
        Formula formula1 = new Formula("A1+5");
        Formula? formula2 = null;
        Assert.IsFalse(formula1.Equals(formula2));
    }

    /// <summary>
    /// should return false 
    /// </summary>
    [TestMethod]
    public void Equals_TestNotFormulaObject_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        var notFormula = "this is not a formula";
        Assert.IsFalse(formula1.Equals(notFormula));
    }
    
    // Testing GetHashCode

    /// <summary>
    /// basic case for getting hashcode
    /// </summary>
    [TestMethod]
    public void GetHashCode_EqualFormulasSameHashBasic_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.AreEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }

    /// <summary>
    /// See if the different canon forms will have the same hashcode
    /// </summary>
    [TestMethod]
    public void GetHashCode_SameHasCodeCanonicalForm_Valid()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5.0000");
        Assert.AreEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }

    /// <summary>
    /// test GetHashCode with different formulas
    /// </summary>
    [TestMethod]
    public void GetHashCode_DifferentFormulas_NotEqual()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("B3+2");
        Assert.AreNotEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }
    
    // Testing Evaluate

    /// <summary>
    /// tesing basic addition with evaluate
    /// </summary>
    [TestMethod]
    public void Evaulate_SimpleFormulaAddition_Valid()
    {
        Formula formula1 = new Formula("1+1");
        Assert.AreEqual(2.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// testing basic multiplication with evaluate
    /// </summary>
    [TestMethod]
    public void Evaluate_SimpleFormulaMultiplication_Valid()
    {
        Formula formula1 = new Formula("1*2");
        Assert.AreEqual(2.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// testing basic subtraction with evaluate
    /// </summary>
    [TestMethod]
    public void Evaluate_SimpleFormulaSubtraction_Valid()
    {
        Formula formula1 = new Formula("2-1");
        Assert.AreEqual(1.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// testing evaluate with simple division
    /// </summary>
    [TestMethod]
    public void Evaluate_SimpleFormulaDivision_Valid()
    {
        Formula formula1 = new Formula("2/1");
        Assert.AreEqual(2.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// testing to see if multiplication takes precedence over other operators
    /// </summary>
    [TestMethod]
    public void Evaluate_TestingOperatorOrderOfOperationsMultiplication_Valid()
    {
        Formula formula1 = new Formula(" 1 + 5 * 5");
        Assert.AreEqual(26.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// testing to see if multiplication happens before subtraction
    /// </summary>
    [TestMethod]
    public void Evalauate_TestingSubOperatorOrderOfOperationsMultiplcation_Valid()
    {
        Formula formula1 = new Formula(" 1 - 5 * 5");
        Assert.AreEqual(-24.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if division happens before addition
    /// </summary>
    [TestMethod]
    public void Evaluate_TestOrderOfOperationsDivision_Valid()
    {
        Formula formula1 = new Formula(" 1 + 5 / 5");
        Assert.AreEqual(2.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if division happens before subtraction
    /// </summary>
    [TestMethod]
    public void Evaluate_TestSubOrderOfOperationsDivision_Valid()
    {
        Formula formula1 = new Formula(" 1 - 5 / 5");
        Assert.AreEqual(0.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if parenthesis will come first in the order of operations
    /// </summary>
    [TestMethod]
    public void Evaluate_TestParenthesisOrderOfOperations_Valid()
    {
        Formula formula1 = new Formula("(1+4)/5");
        Assert.AreEqual(1.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if nested parenthesis behavior works
    /// </summary>
    [TestMethod]
    public void Evaluate_TestNestedParenthesis_Valid()
    {
        Formula formula1 = new Formula("((1+1) * (1+1)) /4");
        Assert.AreEqual(1.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// tesing to see if evaluate will work with a complex equations
    /// </summary>
    [TestMethod]
    public void Evaluate_TestComplicatedNumbers_Valid()
    {
        Formula formula1 = new Formula("((2+2) - (2+2)) + 30 / ((25 * 4) - 70) ");
        Assert.AreEqual(1.0, formula1.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestMultipleAdditionAndSubtraction_Valid()
    {
        Formula formula1 = new Formula("1+2+3+4-1-2-3");
        Assert.AreEqual(4.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if negative numbers will break the evaluation
    /// </summary>
    [TestMethod]
    public void Evaluate_TestNegativeNumbers_Valid()
    {
        Formula formula1 = new Formula("1-2");
        Assert.AreEqual(-1.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// see if evaluating oen value will break the formula
    /// </summary>
    [TestMethod]
    public void Evaluate_TestOneValue_Valid()
    {
        Formula formula1 = new Formula("1");
        Assert.AreEqual(1.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// basic test case for one variable
    /// </summary>
    [TestMethod]
    public void Evaluate_TestBasicVariable_Valid()
    {
        Formula formula1 = new Formula("1+ A1");
        Assert.AreEqual(6.0, formula1.Evaluate(s =>
        {
            if (s == "A1") return 5;
            return 0;
        }));
    }

    /// <summary>
    /// will evaluating lowercase variables break evaluate method
    /// </summary>
    [TestMethod]
    public void Evaluate_TestBasicLowerCaseVariable_Valid()
    {
        Formula formula1 = new Formula("1+a1");
        Assert.AreEqual(6.0, formula1.Evaluate(s =>
        {
            if (s == "A1") return 5;
            return 0;
        }));
    }

    /// <summary>
    /// see if evaluation can handle multiple variables
    /// </summary>
    [TestMethod]
    public void Evaluate_TestMultipleVariables_Valid()
    {
        Formula formula1 = new Formula("a1 + a2");
        Assert.AreEqual(10.0, formula1.Evaluate(s =>
        {
            if (s == "A1") return 5;
            if (s == "A2") return 5;
            return 0;
        }));
    }

    /// <summary>
    /// testing to see if a more complex amount of variables works
    /// </summary>
    [TestMethod]
    public void Evaluate_TestComplexVariableExpression_Valid()
    {
        Formula formula1 = new Formula("(a1 + a2) * b3");
        Assert.AreEqual(40.0, formula1.Evaluate(s =>
        {
            if (s == "A1") return 5;
            if (s == "A2") return 5;
            if (s == "B3") return 4;
            return 0;
        }));
    }

    [TestMethod]
    public void Evaluate_TestSameVariables_Valid()
    {
        Formula formula1 = new Formula("a1 + a1");
        Assert.AreEqual(10.0, formula1.Evaluate(s =>
        {
            if (s == "A1") return 5;
            return 0;
        }));
    }

    /// <summary>
    /// will scientific notation break evaluate
    /// </summary>
    [TestMethod]
    public void Evaluate_TestScientificNotation_Valid()
    {
        Formula formula1 = new Formula("1 + 5e5");
        Assert.AreEqual(500001.0, formula1.Evaluate(s => 0));
    }

    /// <summary>
    /// seeing in the divide by zero returns a formula error
    /// </summary>
    [TestMethod]
    public void Evaluate_TestDivideByZero_FormulaError()
    {
        Formula formula1 = new Formula("1 / 0");
        object result =  formula1.Evaluate(s => 0);
        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// will formula error be thrown with zero in parenthesis
    /// </summary>
    [TestMethod]
    public void Evaluate_TestDivisionByZeroParenthesis_FormulaError()
    {
        Formula formula1 = new Formula("1 / (1-1)");
        object result =  formula1.Evaluate(s => 0);
        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// see if division by zero will trigger with a variable
    /// </summary>
    [TestMethod]
    public void Evaluate_TestDivisionByZeroVariable_FormulaError()
    {
        Formula formula1 = new Formula("1 / a1");
        object result =  formula1.Evaluate(s => {
            if (s == "A1") return 0;
            return 0;
        });
        Assert.IsTrue(result is FormulaError);
    }
    
    /// <summary>
    /// see if a variable without a value will trigger formula error
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    [TestMethod]
    public void Evaluate_TestVariableNoValue_FormulaError()
    {
        Formula formula1 = new Formula("1 + a1");
        object result = formula1.Evaluate(s => throw new ArgumentException());
        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// testing getter/setter on formula error (this was giving me 99 percent code coverage without)
    /// </summary>
    [TestMethod]
    public void FormulaError_CreateFormulaErrorReason_Valid()
    {
        string errorReason = "Error message";
        FormulaError formulaError = new FormulaError(errorReason);
        Assert.AreEqual(errorReason, formulaError.Reason);
    }
    
}
