using System;

namespace FrostAura.Clients.PointsKeeper.Components.Models
{
	public abstract class FormPropertyEffect
    {
        public static List<FormPropertyEffect> Effects = new List<FormPropertyEffect>();

        public string PropertyName { get; private set; }

        public FormPropertyEffect(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
