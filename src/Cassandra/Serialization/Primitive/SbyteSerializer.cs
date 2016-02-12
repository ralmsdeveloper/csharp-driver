﻿//
//      Copyright (C) 2012-2016 DataStax Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Cassandra.Serialization.Primitive
{
    internal class SbyteSerializer : TypeSerializer<sbyte>
    {
        public override ColumnTypeCode CqlType
        {
            get { return ColumnTypeCode.TinyInt; }
        }

        public override sbyte Deserialize(ushort protocolVersion, byte[] buffer, IColumnInfo typeInfo)
        {
            return unchecked((sbyte)buffer[0]);
        }

        public override byte[] Serialize(ushort protocolVersion, sbyte value)
        {
            return new[] { unchecked((byte)value) };
        }
    }
}
