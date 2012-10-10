using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Papyrus.Serialization.Utilities
{
	class PapyrusJsonContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{

		protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
		{

			var propAttrib = Attribute.GetCustomAttribute(member, typeof(RecordPropertyAttribute)) as RecordPropertyAttribute;

			if (propAttrib == null)
				return base.CreateProperty(member, memberSerialization);
			

			// If we have the property attrib, force it to be serialized
			return base.CreateProperty(member, MemberSerialization.Fields);
			
		}

		protected override Newtonsoft.Json.Serialization.JsonObjectContract  CreateObjectContract(Type objectType)
		{

			
			var ret =  base.CreateObjectContract(objectType);

			return ret;

		}


	}
}
