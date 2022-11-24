using System;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using FrostAura.Libraries.Data.Models.EntityFramework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FrostAura.Clients.PointsKeeper.Components.Models
{
	public class EntitySelectFormPropertyEffect<TValue, TControlToRenderType> : FormPropertyEffect
		where TControlToRenderType : InputBase<TValue>
	{
		public List<BaseNamedEntity>  DataSource { get; private set; }
		public Type ControlToRenderType { get; private set; }

        public EntitySelectFormPropertyEffect(string propertyName, List<BaseNamedEntity> dataSource) :
			base(propertyName)
		{
			DataSource = dataSource;
			ControlToRenderType = typeof(TControlToRenderType);
        }
	}
}
