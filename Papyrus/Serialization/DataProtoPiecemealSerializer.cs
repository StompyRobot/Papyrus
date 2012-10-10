/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using Papyrus.Serialization.Utilities;

namespace Papyrus.Serialization
{
	internal class DataProtoPiecemealSerializer : PiecemealSerializer
	{

		public static readonly string HeaderExtension = "sgpp";

		public static readonly string ItemExtension = "rec";

		public override string Extension
		{
			get { return HeaderExtension; }
		}

		protected override string RecordExtension
		{
			get { return ItemExtension; }
		}

		protected override RecordContainer<T> ReadRecord<T>(FileStream input)
		{

			return ProtoBufUtils.TypeModel.Deserialize(input, null, typeof (RecordContainer<T>)) as RecordContainer<T>;

		}

		protected override void SerializeRecord<T>(RecordContainer<T> record, FileStream output)
		{
			
			ProtoBufUtils.TypeModel.Serialize(output, record);

		}

		protected override PluginHeader ReadHeader(FileStream input)
		{

			return ProtoBuf.Serializer.Deserialize<PluginHeader>(input);

		}

		protected override void WriteHeader(PluginHeader header, Stream stream)
		{

			ProtoBuf.Serializer.Serialize(stream, header);

		}
		
	}
}
