using System;

namespace Web.Api.Data.Contracts
{
    public interface ISetting
    {
        int Id { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        string Type { get; set; }
        bool Required { get; set; }
        bool IsEncrypted { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
    }
}
