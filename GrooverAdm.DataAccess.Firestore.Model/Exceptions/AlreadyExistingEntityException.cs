using GrooverAdm.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Exceptions
{
    class AlreadyExistingEntityException : GrooverException
    {
        public AlreadyExistingEntityException() : base() { }
        public AlreadyExistingEntityException(string? message) : base(message) { }

    }
}
