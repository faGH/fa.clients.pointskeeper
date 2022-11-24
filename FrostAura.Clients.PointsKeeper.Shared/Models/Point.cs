using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Point entity model.
  /// </summary>
  [Table("Points")]
  [DebuggerDisplay("Count: {Count}")]
  public class Point : BaseEntity
  {
    /// <summary>
    /// The count / amount of points this particular event represents.
    /// </summary>
    [Required(ErrorMessage = "The count of points are required.")]
    [Range(1, int.MaxValue, ErrorMessage = "The count of points are required to be greater than 1.")]
    public int Count { get; set; }
    /// <summary>
    /// The unique id of the player the respective point belongs to.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid player is required.")]
    public int PlayerId { get; set; }
    /// <summary>
    /// The player context that the respective point belongs to.
    /// </summary>
    [ForeignKey(nameof(PlayerId))]
    public virtual Player Player { get; set; }
  }
}
