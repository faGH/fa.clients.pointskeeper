using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FrostAura.Clients.PointsKeeper.Components.Models
{
	public class ImagePickerFormPropertyEffect : FormPropertyEffect
	{
		public List<BaseNamedEntity>  DataSource { get; private set; }
		public Type ControlToRenderType { get; private set; }
		public string DefaultUrl { get; private set; }

        public ImagePickerFormPropertyEffect(string propertyName, string defaultUrl = "https://via.placeholder.com/256x256") :
			base(propertyName)
		{
			ControlToRenderType = typeof(InputFile);
			DefaultUrl = defaultUrl;
        }
	}
}
