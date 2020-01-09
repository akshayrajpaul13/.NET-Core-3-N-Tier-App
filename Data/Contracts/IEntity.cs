using System;

namespace Web.Api.Data.Contracts
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
    }
}
