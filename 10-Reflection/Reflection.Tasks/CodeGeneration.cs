using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Reflection.Tasks
{
    public class CodeGeneration
    {
        /// <summary>
        /// Returns the functions that returns vectors' scalar product:
        /// (a1, a2,...,aN) * (b1, b2, ..., bN) = a1*b1 + a2*b2 + ... + aN*bN
        /// Generally, CLR does not allow to implement such a method via generics to have one function for various number types:
        /// int, long, float. double.
        /// But it is possible to generate the method in the run time! 
        /// See the idea of code generation using Expression Tree at: 
        /// http://blogs.msdn.com/b/csharpfaq/archive/2009/09/14/generating-dynamic-methods-with-expression-trees-in-visual-studio-2010.aspx
        /// </summary>
        /// <typeparam name="T">number type (int, long, float etc)</typeparam>
        /// <returns>
        ///   The function that return scalar product of two vectors
        ///   The generated dynamic method should be equal to static MultuplyVectors (see below).   
        /// </returns>
        public static Func<T[], T[], T> GetVectorMultiplyFunction<T>() where T : struct {
            ParameterExpression param1 = Expression.Parameter(typeof(T[]), "array1");
            ParameterExpression param2 = Expression.Parameter(typeof(T[]), "array2");
            ParameterExpression paramResult = Expression.Parameter(typeof(T), "result");
            LabelTarget label = Expression.Label(typeof(T));
            ParameterExpression paramCount = Expression.Parameter(typeof(int),"count");
            BlockExpression block = Expression.Block(
            new[] { paramResult,  paramCount},
            Expression.Assign(paramCount,Expression.Constant(0)),
            Expression.Assign(paramResult, Expression.Constant(default(T))),
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.And(
                        Expression.LessThan(paramCount, Expression.ArrayLength(param1)),
                        Expression.LessThan(paramCount, Expression.ArrayLength(param2))),
                        Expression.AddAssign(paramResult, Expression.MultiplyAssign(
                            Expression.ArrayAccess(param1, paramCount),
                            Expression.ArrayAccess(param2, Expression.PostIncrementAssign(paramCount)))),
                            Expression.Break(label, paramResult)),
                            label
                ));

            Expression<Func<T[], T[], T>> lambda = Expression.Lambda<Func<T[], T[], T>>(block, param1, param2);
            return lambda.Compile();
        }


        // Static solution to check performance benchmarks
        public static int MultuplyVectors(int[] first, int[] second) {
            int result = 0;
            for (int i = 0; i < first.Length; i++) {
                result += first[i] * second[i];
            }
            return result;
        }

    }
}
