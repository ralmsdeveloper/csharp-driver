﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassandra.Serialization
{
    internal class TupleSerializer : TypeSerializer<IStructuralEquatable>
    {
        public override ColumnTypeCode CqlType
        {
            get { return ColumnTypeCode.Tuple; }
        }

        public override IStructuralEquatable Deserialize(ushort protocolVersion, byte[] buffer, IColumnInfo typeInfo)
        {
            var tupleInfo = (TupleColumnInfo)typeInfo;
            var tupleType = GetClrType(ColumnTypeCode.Tuple, tupleInfo);
            var tupleValues = new object[tupleInfo.Elements.Count];
            var offset = 0;
            for (var i = 0; i < tupleInfo.Elements.Count; i++)
            {
                var element = tupleInfo.Elements[i];
                if (offset >= buffer.Length)
                {
                    break;
                }
                var length = BeConverter.ToInt32(buffer, offset);
                offset += 4;
                if (length < 0)
                {
                    continue;
                }
                var itemBuffer = Utils.SliceBuffer(buffer, offset, length);
                offset += length;
                tupleValues[i] = DeserializeChild(itemBuffer, element.TypeCode, element.TypeInfo);
            }

            return (IStructuralEquatable)Activator.CreateInstance(tupleType, tupleValues);
        }

        internal Type GetClrType(IColumnInfo typeInfo)
        {
            var tupleInfo = (TupleColumnInfo)typeInfo;
            Type genericTupleType;
            switch (tupleInfo.Elements.Count)
            {
                case 1:
                    genericTupleType = typeof(Tuple<>);
                    break;
                case 2:
                    genericTupleType = typeof(Tuple<,>);
                    break;
                case 3:
                    genericTupleType = typeof(Tuple<,,>);
                    break;
                case 4:
                    genericTupleType = typeof(Tuple<,,,>);
                    break;
                case 5:
                    genericTupleType = typeof(Tuple<,,,,>);
                    break;
                case 6:
                    genericTupleType = typeof(Tuple<,,,,,>);
                    break;
                case 7:
                    genericTupleType = typeof(Tuple<,,,,,,>);
                    break;
                default:
                    return typeof(byte[]);
            }
            return genericTupleType.MakeGenericType(
                tupleInfo.Elements.Select(s => GetClrType(s.TypeCode, s.TypeInfo)).ToArray());
        }

        public override byte[] Serialize(ushort protocolVersion, IStructuralEquatable value)
        {
            var tupleType = value.GetType();
            var subtypes = tupleType.GetGenericArguments();
            var bufferList = new List<byte[]>();
            var bufferLength = 0;
            for (var i = 1; i <= subtypes.Length; i++)
            {
                var prop = tupleType.GetProperty("Item" + i);
                if (prop != null)
                {
                    var buffer = SerializeChild(prop.GetValue(value, null));
                    bufferList.Add(buffer);
                    if (buffer != null)
                    {
                        bufferLength += buffer.Length;
                    }
                }
            }
            return EncodeBufferList(bufferList, bufferLength);
        }
    }
}