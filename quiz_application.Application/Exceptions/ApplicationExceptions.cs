namespace quiz_application.Application.Exceptions
{
    // Base exception for all application exceptions
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    // Used when request parameters are invalid
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }

    // Used when a requested resource is not found
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    // Used for unauthorized access attempts
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }

    // Used when business rules are violated
    public class BusinessRuleViolationException : ApplicationException
    {
        public BusinessRuleViolationException(string message) : base(message)
        {
        }
    }
}
