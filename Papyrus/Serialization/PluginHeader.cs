/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProtoBuf;

namespace Papyrus.Serialization
{
	[ProtoContract]
	[JsonObject(MemberSerialization.OptIn)]
	class PluginHeader
	{

		[ProtoMember(1)]
		[JsonProperty]
		public string Name;

		[ProtoMember(2)]
		[JsonProperty]
		public string DirectoryName;

		[ProtoMember(3)]
		[JsonProperty]
		public string Description;

		[ProtoMember(4)]
		[JsonProperty]
		public string Author;

		[ProtoMember(5, OverwriteList = true)] 
		[JsonProperty] 
		public List<Guid> ModuleDependencies = new List<Guid>();

		[ProtoMember(6, OverwriteList = true)]
		[JsonProperty]
		public List<string> PluginDependencies = new List<string>();

		[ProtoMember(7)]
		[JsonProperty]
		public DateTime LastModified;

		[JsonIgnore]
		public string SourceFile;

	}
}