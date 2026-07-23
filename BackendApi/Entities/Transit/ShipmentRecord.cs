using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Transit;

[Table("tbl_shipment_record")]
public sealed class ShipmentRecord : BaseEntity
{
    public int       TransitRouteId { get; set; }
    public int       VehicleId      { get; set; }
    public DateTime  StartedAtUtc   { get; set; }
    public DateTime  CompletedAtUtc { get; set; }
}