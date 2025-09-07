using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;

namespace Employees.ExceptionFilters
{
    public class DatabaseExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<DatabaseExceptionFilter> _logger;

        public DatabaseExceptionFilter(ILogger<DatabaseExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is PostgresException pgEx)
            {
                _logger.LogWarning(pgEx, "PostgreSQL Error: {Message}", pgEx.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = GetStatusCode(pgEx.SqlState),
                    Title = GetTitle(pgEx.SqlState),
                    Detail = pgEx.Message
                };

                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = problemDetails.Status
                };

                context.ExceptionHandled = true;
                return;
            }

            if (exception is DataException dapperEx)
            {
                _logger.LogWarning(dapperEx, "Dapper mapping error");

                context.Result = new BadRequestObjectResult(new ProblemDetails
                {
                    Status = 400,
                    Title = "Ошибка преобразования данных",
                    Detail = "Проверьте типы данных в запросе или модели."
                });

                context.ExceptionHandled = true;
                return;
            }
        }

        private int GetStatusCode(string sqlState)
        {
            return sqlState switch
            {
                "23505" => 409,
                "23503" => 404,
                "23502" => 400,
                "42703" => 400,
                "42P01" => 400,
                "42601" => 400,
                _ => 500
            };
        }

        private string GetTitle(string sqlState)
        {
            return sqlState switch
            {
                "23505" => "Конфликт уникальности",
                "23503" => "Связанная запись не найдена",
                "23502" => "Обязательное поле не заполнено",
                "42703" => "Несуществующий столбец",
                "42P01" => "Несуществующая таблица",
                "42601" => "Ошибка синтаксиса SQL",
                _ => "Ошибка базы данных"
            };
        }
    }
}
