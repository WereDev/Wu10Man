using System;

namespace WereDev.Utils.Wu10Man.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message, string entityname)
            : base(message)
        {
            EntityName = entityname;
        }

        public string EntityName { get; }
    }
}
