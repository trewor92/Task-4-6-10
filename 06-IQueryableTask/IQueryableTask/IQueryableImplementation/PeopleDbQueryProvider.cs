using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableTask.IQueryableImplementation
{
    public class PeopleDbQueryProvider : IQueryProvider, IDisposable
    {
        DbConnection _connection;
        readonly string _connectionString = @"Data Source = C:\Users\trewor\source\repos\IQueryableTask\IQueryableTask.Test\people.db;";

        public PeopleDbQueryProvider()
        {
            _connection = new SQLiteConnection(_connectionString);
            _connection.Open();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)new People(expression);
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return (IQueryable<TResult>) new People(expression);
        }

        public object Execute(Expression expression)
        {
            string commandText = GetSqlQuery(expression);
            return new PersonService().Search(commandText);         
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        /// <summary>
        /// Generates YQL Query
        /// </summary>
        /// <param name="expression">Expression tree</param>
        /// <returns></returns>
        public string GetSqlQuery(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);

            // HINT: This method is not part of IQueryProvider interface and is used here only for tests.
            // HINT: To transform expression to sql query create a class derived from ExpressionVisitor
            // HINT: Read the tutorial https://msdn.microsoft.com/en-us/library/bb546158.aspx for more info
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
