namespace Application.Exceptions;

public class DataNullException : Exception
{
    public DataNullException()
    {
    }

    public DataNullException(string message) : base(message)
    {
    }
}

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}

public class DataDuplicateException : Exception
{
    public DataDuplicateException()
    {
    }

    public DataDuplicateException(string message) : base(message)
    {
    }
}

public class ErrorOpenFileException : Exception
{
    public ErrorOpenFileException()
    {
    }

    public ErrorOpenFileException(string message) : base(message)
    {
    }
}

public class DataInValidValues : Exception
{
    public DataInValidValues()
    {
    }

    public DataInValidValues(string message) : base(message)
    {
    }
}