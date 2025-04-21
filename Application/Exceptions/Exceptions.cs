using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class DataNullException : Exception
    {
        public DataNullException() : base() { }

        public DataNullException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }
    }

    public class DataDuplicateException : Exception
    {
        public DataDuplicateException() : base() { }
        public DataDuplicateException(string message) : base(message) { }
    }

    public class ErrorOpenFileException : Exception
    {
        public ErrorOpenFileException() : base() { }
        public ErrorOpenFileException(string message) : base(message) { }
    }

    public class DataInValidValues : Exception
    {
        public DataInValidValues() : base() { }
        public DataInValidValues(string message) : base(message) { }
    }
}
