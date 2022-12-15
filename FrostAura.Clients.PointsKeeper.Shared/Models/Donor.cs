using System.ComponentModel;
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
    /// <summary>
    /// The amount to donate.
    /// </summary>
    [Required]
    public double Amount { get; set; }
    /// <summary>
    /// Whether the donation is once-off (True) or per-point (False).
    /// </summary>
    [Required]
    [Description("Are Donations Once-Off?")]
    public bool OnceOff { get; set; }
    /// <summary>
    /// The donor's company logo.
    /// </summary>
    [Description("Company Logo")]
    public string Logo { get; set; } = "https://via.placeholder.com/256x256";
  }
}
