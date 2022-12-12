using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Donor entity model.
  /// </summary>
  [Table("Donors")]
  [DebuggerDisplay("Name: {Name}")]
  public class Donor : BaseNamedEntity
  {
    [Required]
    [Range(0.1, int.MaxValue, ErrorMessage = "A valid amount per point is required greater than 0.1.")]
    public double Amount { get; set; }
    [Required]
    public bool OnceOff { get; set; }
  }
}
