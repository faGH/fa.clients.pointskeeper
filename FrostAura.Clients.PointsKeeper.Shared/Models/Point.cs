using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel;
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
    /// The unique id of the player the respective point belongs to.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid player 1 is required.")]
    [Description("Player 1")]
    public int Player1Id { get; set; }
    /// <summary>
    /// The count / amount of points this particular event represents.
    /// </summary>
    [Required(ErrorMessage = "The score for player 1 is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "The score for player 1 is required to be greater than 1.")]
    [Description("Player 1's Score")]
    public int Player1Score { get; set; }
    /// <summary>
    /// The unique id of the player the respective point belongs to.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid player 2 is required.")]
    [Description("Player 2")]
    public int Player2Id { get; set; }
    /// <summary>
    /// The count / amount of points this particular event represents.
    /// </summary>
    [Required(ErrorMessage = "The score for player 2 is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "The score for player 2 is required to be greater than 1.")]
    [Description("Player 2's Score")]
    public int Player2Score { get; set; }
    /// <summary>
    /// The player1 context that the respective point belongs to.
    /// </summary>
    [ForeignKey(nameof(Player1Id))]
    public virtual Player Player1 { get; set; }
    /// <summary>
    /// The player2 context that the respective point belongs to.
    /// </summary>
    [ForeignKey(nameof(Player2Id))]
    public virtual Player Player2 { get; set; }
    }
}
