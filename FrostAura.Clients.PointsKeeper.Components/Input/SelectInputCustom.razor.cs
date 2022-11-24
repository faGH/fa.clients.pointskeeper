using System;
using System.Diagnostics.CodeAnalysis;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FrostAura.Clients.PointsKeeper.Components.Input
{
    public partial class SelectInputCustom<TValue> : InputSelect<TValue>
    {
        [Parameter]
        public List<BaseNamedEntity> DataSource { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ChildContent = (builder) =>
            {
                for (var i = 0; i < DataSource.Count; i++)
                {
                    var dataRow = DataSource[i];
                    builder.AddMarkupContent(i + 1, $"<option value=\"{dataRow.Id}\">{dataRow.Name}</option>");
                }
            };
        }
    }
}

