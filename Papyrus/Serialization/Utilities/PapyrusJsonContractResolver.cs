/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Papyrus.Serialization.Utilities
{
	class PapyrusJsonContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{

		protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
		{

			JsonProperty ret;

			var propAttrib = Attribute.GetCustomAttribute(member, typeof(RecordPropertyAttribute)) as RecordPropertyAttribute;

			if (propAttrib == null) {
				ret = base.CreateProperty(member, memberSerialization);
			} else {
				// If we have the property attrib, force it to be serialized
				ret = base.CreateProperty(member, MemberSerialization.Fields);
			}

			// Conditionally serialize the Plugin property if it is different from the Source.
			if (typeof(RecordReference).IsAssignableFrom(ret.DeclaringType) && ret.PropertyName == "Plugin") {

				ret.ShouldSerialize = o =>
				{
					RecordReference pointer = (RecordReference) o;
					return pointer.Plugin != pointer.Source;
				};

			}

			if (typeof (IRecordContainer).IsAssignableFrom(ret.DeclaringType) && ret.PropertyName == "Destination") {

				ret.ShouldSerialize = o =>
				{
					IRecordContainer pointer = (IRecordContainer)o;
					return pointer.Destination != pointer.Location;
				};

			}

			return ret;

		}

		protected override Newtonsoft.Json.Serialization.JsonObjectContract  CreateObjectContract(Type objectType)
		{

			
			var ret =  base.CreateObjectContract(objectType);

			return ret;

		}


	}
}
