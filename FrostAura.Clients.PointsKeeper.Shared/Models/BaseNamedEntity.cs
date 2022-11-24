using FrostAura.Libraries.Data.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
  /// <summary>
  /// Base named entity model.
  /// </summary>
  [DebuggerDisplay("Name: {Name}")]
  public class BaseNamedEntity : BaseEntity
  {
    /// <summary>
    /// Entity name / short description.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = $"A valid {nameof(BaseNamedEntity)} name is required.")]
    public string Name { get; set; } = string.Empty;
  }
}
