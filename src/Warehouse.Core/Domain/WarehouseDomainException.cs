namespace Warehouse.Core.Domain
{
    public class WarehouseDomainException : Exception
    {
        public WarehouseDomainException(string message) : base(message) { }
    }
}
