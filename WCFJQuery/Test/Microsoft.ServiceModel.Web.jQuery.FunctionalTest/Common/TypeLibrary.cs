using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Silverlight.Cdf.Test.Common.Utility;

namespace Microsoft.ServiceModel.Web.Test.Common
{
    [DataContract]
    public enum EnumType_17
    {
        [EnumMember]
        member_0,
        [EnumMember]
        member_1 = 2,
        [EnumMember]
        member_2,
        [EnumMember]
        member_3,
        [EnumMember]
        member_4,
        [EnumMember]
        member_5,
        [EnumMember]
        member_6,
        [EnumMember]
        member_7,
        [EnumMember]
        member_8 = 9,
        [EnumMember]
        member_9,
        [EnumMember]
        member_10,
        [EnumMember]
        member_11 = 12,
        [EnumMember]
        member_12 = 13,
        [EnumMember]
        member_13,
        [EnumMember]
        member_14,
        [EnumMember]
        member_15,
        [EnumMember]
        member_16,
        [EnumMember]
        member_17,
    }

    [DataContract]
    public enum EnumType_35
    {
        [EnumMember]
        member_0,
        [EnumMember]
        member_1,
        [EnumMember]
        member_2,
        [EnumMember]
        member_3,
        [EnumMember]
        member_4,
        [EnumMember]
        member_5,
        [EnumMember]
        member_6,
        [EnumMember]
        member_7,
        [EnumMember]
        member_8,
    }

    public interface IEmptyInterface
    {
    }

    [DataContract]
    public struct StructInt16
    {
        [DataMember]
        public short Int16Member;
    }

    [DataContract]
    public struct StructGuid
    {
        [DataMember]
        public Guid GuidMember;
    }

    [DataContract]
    public class DCType_1
    {
        [DataMember]
        public byte Member0 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_1 other = obj as DCType_1;
            return (other != null) && this.Member0.Equals(other.Member0);
        }

        public override int GetHashCode()
        {
            int result = this.Member0.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_3
    {
        [DataMember]
        public ulong Member2 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_3 other = obj as DCType_3;
            return (other != null) && this.Member2.Equals(other.Member2);
        }

        public override int GetHashCode()
        {
            int result = this.Member2.GetHashCode();
            return result;
        }
    }

    [Serializable]
    public class SerType_4
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public char Member0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public short? Member1;

        public override bool Equals(object obj)
        {
            SerType_4 other = obj as SerType_4;
            return (other != null) && this.Member0.Equals(other.Member0) && Util.CompareNullable<short>(this.Member1, other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [Serializable]
    public class SerType_5
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public char Member0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public byte? Member1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public char Member2;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public bool Member3;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public sbyte Member4;

        public override bool Equals(object obj)
        {
            SerType_5 other = obj as SerType_5;
            return (other != null) &&
                this.Member0.Equals(other.Member0) &&
                Util.CompareNullable<byte>(this.Member1, other.Member1) &&
                this.Member2.Equals(other.Member2) &&
                this.Member3.Equals(other.Member3) &&
                this.Member4.Equals(other.Member4);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            result ^= this.Member2.GetHashCode();
            result ^= this.Member3.GetHashCode();
            result ^= this.Member4.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_7
    {
        [DataMember]
        public long? Member1 { get; set; }

        [DataMember]
        public sbyte Member2 { get; set; }

        [DataMember]
        public byte[] Member3 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_7 other = obj as DCType_7;
            return (other != null) &&
                Util.CompareNullable<long>(this.Member1, other.Member1) &&
                this.Member2.Equals(other.Member2) &&
                Util.CompareArrays(this.Member3, other.Member3);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            result ^= this.Member2.GetHashCode();
            result ^= Util.ComputeArrayHashCode(this.Member3);
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_9
    {
        [DataMember]
        public sbyte? Member0 { get; set; }

        [DataMember]
        public Guid Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_9 other = obj as DCType_9;
            return (other != null) &&
                Util.CompareNullable<sbyte>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            return result;
        }
    }

    [Serializable]
    public class SerType_11
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public float Member0;

        public override bool Equals(object obj)
        {
            SerType_11 other = obj as SerType_11;
            return (other != null) && this.Member0.Equals(other.Member0);
        }

        public override int GetHashCode()
        {
            int result = this.Member0.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_15
    {
        public byte[] Member0 { get; set; }

        public ushort Member1 { get; set; }

        [DataMember]
        public Guid Member2 { get; set; }

        public DCType_1 Member3 { get; set; }

        [DataMember]
        public DCType_7 Member5 { get; set; }

        [DataMember]
        public int Member6 { get; set; }

        [DataMember]
        public DCType_9 Member7 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_15 other = obj as DCType_15;
            return (other != null) &&
                this.Member2.Equals(other.Member2) &&
                Util.CompareObjects<DCType_7>(this.Member5, other.Member5) &&
                this.Member6.Equals(other.Member6) &&
                Util.CompareObjects<DCType_9>(this.Member7, other.Member7);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member2.GetHashCode();
            result ^= (this.Member5 == null) ? 0 : this.Member5.GetHashCode();
            result ^= this.Member6.GetHashCode();
            result ^= (this.Member7 == null) ? 0 : this.Member7.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_16
    {
        [DataMember]
        public decimal Member0 { get; set; }

        [DataMember]
        public SerType_11 Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_16 other = obj as DCType_16;
            return (other != null) &&
                this.Member0.Equals(other.Member0) &&
                Util.CompareObjects<SerType_11>(this.Member1, other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_18
    {
        [DataMember]
        public SerType_5 Member0 { get; set; }

        [DataMember]
        public short Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_18 other = obj as DCType_18;
            return (other != null) &&
                Util.CompareObjects<SerType_5>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_19
    {
        [DataMember]
        public uint? Member0 { get; set; }

        [DataMember]
        public byte? Member1 { get; set; }

        [DataMember]
        public long? Member2 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_19 other = obj as DCType_19;
            return (other != null) &&
                Util.CompareNullable<uint>(this.Member0, other.Member0) &&
                Util.CompareNullable<byte>(this.Member1, other.Member1) &&
                Util.CompareNullable<long>(this.Member2, other.Member2);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            result ^= (this.Member2 == null) ? 0 : this.Member2.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_20
    {
        [DataMember]
        public DCType_9 Member0 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_20 other = obj as DCType_20;
            return (other != null) &&
                Util.CompareObjects<DCType_9>(this.Member0, other.Member0);
        }

        public override int GetHashCode()
        {
            int result = (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            return result;
        }
    }

    [Serializable]
    public class SerType_22
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public byte Member0;

        public override bool Equals(object obj)
        {
            SerType_22 other = obj as SerType_22;
            return (other != null) &&
                this.Member0.Equals(other.Member0);
        }

        public override int GetHashCode()
        {
            int result = this.Member0.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_25
    {
        [DataMember]
        public SerType_22 Member0 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_25 other = obj as DCType_25;
            return (other != null) &&
                Util.CompareObjects<SerType_22>(this.Member0, other.Member0);
        }

        public override int GetHashCode()
        {
            int result = (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [Serializable]
    public class SerType_26
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public char Member0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public short? Member1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public SerType_4 Member2;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public decimal Member3;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2235", Justification = "The type is serializable (it contains a [DataContract] attribute).")]
        public DCType_3 Member4;

        public override bool Equals(object obj)
        {
            SerType_26 other = obj as SerType_26;
            return (other != null) &&
                this.Member0.Equals(other.Member0) &&
                Util.CompareNullable<short>(this.Member1, other.Member1) &&
                Util.CompareObjects<SerType_4>(this.Member2, other.Member2) &&
                this.Member3.Equals(other.Member3) &&
                Util.CompareObjects<DCType_3>(this.Member4, other.Member4);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            result ^= (this.Member2 == null) ? 0 : this.Member2.GetHashCode();
            result ^= this.Member3.GetHashCode();
            result ^= (this.Member4 == null) ? 0 : this.Member4.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_31
    {
        [DataMember]
        public SerType_22 Member0 { get; set; }

        [DataMember]
        public byte Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_31 other = obj as DCType_31;
            return (other != null) &&
                Util.CompareObjects<SerType_22>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_32
    {
        [DataMember]
        public int Member0 { get; set; }

        [DataMember]
        public DCType_20 Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_32 other = obj as DCType_32;
            return (other != null) &&
                this.Member0.Equals(other.Member0) &&
                Util.CompareObjects<DCType_20>(this.Member1, other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [Serializable]
    public class SerType_33
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        public long? Member0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Testing serialization of [Serializable] types, which needs public fields.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2235", Justification = "The type is serializable (it contains a [DataContract] attribute).")]
        public DCType_20 Member1;

        public override bool Equals(object obj)
        {
            SerType_33 other = obj as SerType_33;
            return (other != null) &&
                this.Member0.Equals(other.Member0) &&
                Util.CompareObjects<DCType_20>(this.Member1, other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_34
    {
        [DataMember]
        public short? Member0 { get; set; }

        [DataMember]
        public float Member1 { get; set; }

        [DataMember]
        public EnumType_17 Member2 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_34 other = obj as DCType_34;
            return (other != null) &&
                Util.CompareNullable<short>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1) &&
                this.Member2.Equals(other.Member2);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            result ^= this.Member2.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_36
    {
        [DataMember]
        public bool? Member0 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_36 other = obj as DCType_36;
            return (other != null) &&
                Util.CompareNullable<bool>(this.Member0, other.Member0);
        }

        public override int GetHashCode()
        {
            int result = (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_38
    {
        [DataMember]
        public ulong Member0 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_38 other = obj as DCType_38;
            return (other != null) &&
                this.Member0.Equals(other.Member0);
        }

        public override int GetHashCode()
        {
            int result = this.Member0.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class DCType_40
    {
        [DataMember]
        public SerType_22 Member0 { get; set; }

        [DataMember]
        public short Member1 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_40 other = obj as DCType_40;
            return (other != null) &&
                Util.CompareObjects<SerType_22>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_42
    {
        [DataMember]
        public SerType_11 Member0 { get; set; }

        [DataMember]
        public EnumType_35 Member1 { get; set; }

        [DataMember]
        public SerType_5 Member2 { get; set; }

        [DataMember]
        public DCType_3 Member3 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_42 other = obj as DCType_42;
            return (other != null) &&
                Util.CompareObjects<SerType_11>(this.Member0, other.Member0) &&
                this.Member1.Equals(other.Member1) &&
                Util.CompareObjects<SerType_5>(this.Member2, other.Member2) &&
                Util.CompareObjects<DCType_3>(this.Member3, other.Member3);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1.GetHashCode();
            result ^= (this.Member2 == null) ? 0 : this.Member2.GetHashCode();
            result ^= (this.Member3 == null) ? 0 : this.Member3.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class DCType_65
    {
        [DataMember]
        public ulong? Member0 { get; set; }

        [DataMember]
        public DCType_7 Member1 { get; set; }

        [DataMember]
        public DCType_36 Member4 { get; set; }

        [DataMember]
        public uint Member5 { get; set; }

        public override bool Equals(object obj)
        {
            DCType_65 other = obj as DCType_65;
            return (other != null) &&
                Util.CompareNullable<ulong>(this.Member0, other.Member0) &&
                Util.CompareObjects<DCType_7>(this.Member1, other.Member1) &&
                Util.CompareObjects<DCType_36>(this.Member4, other.Member4) &&
                this.Member5.Equals(other.Member5);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= (this.Member0 == null) ? 0 : this.Member0.GetHashCode();
            result ^= (this.Member1 == null) ? 0 : this.Member1.GetHashCode();
            result ^= (this.Member4 == null) ? 0 : this.Member4.GetHashCode();
            result ^= this.Member5.GetHashCode();
            return result;
        }
    }

    [CLSCompliant(false)]
    public class ListType_1
    {
        public List<DCType_15> Member0 { get; set; }

        public List<DCType_34> Member1 { get; set; }

        public List<SerType_33> Member2 { get; set; }

        public override bool Equals(object obj)
        {
            ListType_1 other = obj as ListType_1;
            return (other != null) &&
                Util.CompareLists<DCType_15>(this.Member0, other.Member0) &&
                Util.CompareLists<DCType_34>(this.Member1, other.Member1) &&
                Util.CompareLists<SerType_33>(this.Member2, other.Member2);
        }

        public override int GetHashCode()
        {
            int result = 0;
            if (this.Member0 != null)
            {
                result ^= Util.ComputeArrayHashCode(this.Member0.ToArray());
            }

            if (this.Member1 != null)
            {
                result ^= Util.ComputeArrayHashCode(this.Member0.ToArray());
            }

            if (this.Member2 != null)
            {
                result ^= Util.ComputeArrayHashCode(this.Member0.ToArray());
            }

            return result;
        }
    }

    [CLSCompliant(false)]
    [DataContract]
    public class ListType_2
    {
        [DataMember]
        public SerType_4[] Member0 { get; set; }

        [DataMember]
        public DCType_32[] Member1 { get; set; }

        public override bool Equals(object obj)
        {
            ListType_2 other = obj as ListType_2;
            return (other != null) &&
                Util.CompareArrays(this.Member0, other.Member0) &&
                Util.CompareArrays(this.Member1, other.Member1);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= Util.ComputeArrayHashCode(this.Member0);
            result ^= Util.ComputeArrayHashCode(this.Member0);
            return result;
        }
    }

    [DataContract]
    [KnownType(typeof(DerivedType))]
    public class BaseType
    {
        [DataMember]
        public string Member0 { get; set; }
        [DataMember]
        public DCType_1 Member1 { get; set; }

        public static BaseType CreateInstance(Random rndGen)
        {
            return new DerivedType(rndGen);
        }
    }

    [DataContract]
    public class DerivedType : BaseType, IEmptyInterface
    {
        public DerivedType(Random rndGen)
        {
            this.Member0 = InstanceCreator.CreateInstanceOf<string>(rndGen);
            this.Member1 = InstanceCreator.CreateInstanceOf<DCType_1>(rndGen);
            this.Member2 = InstanceCreator.CreateInstanceOf<SerType_4>(rndGen);
            this.Member3 = InstanceCreator.CreateInstanceOf<decimal>(rndGen);
        }

        [DataMember]
        public SerType_4 Member2 { get; set; }
        [DataMember]
        public decimal Member3 { get; set; }

        public override bool Equals(object obj)
        {
            DerivedType other = obj as DerivedType;
            return (other != null) &&
                Util.CompareObjects<string>(this.Member0, other.Member0) &&
                Util.CompareObjects<DCType_1>(this.Member1, other.Member1) &&
                Util.CompareObjects<SerType_4>(this.Member2, other.Member2) &&
                this.Member3.Equals(other.Member3);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.Member0 == null ? 0 : this.Member0.GetHashCode();
            result ^= this.Member1 == null ? 0 : this.Member1.GetHashCode();
            result ^= this.Member2 == null ? 0 : this.Member2.GetHashCode();
            result ^= this.Member3.GetHashCode();
            return result;
        }
    }

    [DataContract]
    public class PolymorphicMember
    {
        [DataMember]
        public BaseType Member_0 { get; set; }

        public override bool Equals(object obj)
        {
            PolymorphicMember other = obj as PolymorphicMember;
            return (other != null) &&
                Util.CompareObjects<BaseType>(this.Member_0, other.Member_0);
        }

        public override int GetHashCode()
        {
            return this.Member_0 == null ? 0 : this.Member_0.GetHashCode();
        }
    }

    [DataContract, KnownType(typeof(DerivedType))]
    public class PolymorphicAsInterfaceMember
    {
        [DataMember]
        public IEmptyInterface Member_0 { get; set; }

        public override bool Equals(object obj)
        {
            PolymorphicAsInterfaceMember other = obj as PolymorphicAsInterfaceMember;
            return (other != null) &&
                Util.CompareObjects<IEmptyInterface>(this.Member_0, other.Member_0);
        }

        public override int GetHashCode()
        {
            return this.Member_0 == null ? 0 : this.Member_0.GetHashCode();
        }
    }

    [DataContract]
    [KnownType(typeof(DerivedType))]
    public class CollectionsWithPolymorphicMember
    {
        [DataMember]
        public List<BaseType> ListOfBase { get; set; }
        [DataMember]
        public List<IEmptyInterface> ListOfInterface { get; set; }
        [DataMember]
        public Dictionary<string, BaseType> DictionaryOfBase { get; set; }
        [DataMember]
        public Dictionary<string, IEmptyInterface> DictionaryOfInterface { get; set; }

        public override bool Equals(object obj)
        {
            CollectionsWithPolymorphicMember other = obj as CollectionsWithPolymorphicMember;
            return (other != null) &&
                Util.CompareLists<BaseType>(this.ListOfBase, other.ListOfBase) &&
                Util.CompareLists<IEmptyInterface>(this.ListOfInterface, other.ListOfInterface) &&
                Util.CompareDictionaries<string, BaseType>(this.DictionaryOfBase, other.DictionaryOfBase) &&
                Util.CompareDictionaries<string, IEmptyInterface>(this.DictionaryOfInterface, other.DictionaryOfInterface);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result ^= this.ListOfBase == null ? 0 : Util.ComputeArrayHashCode(this.ListOfBase.ToArray());
            result ^= this.ListOfInterface == null ? 0 : Util.ComputeArrayHashCode(this.ListOfInterface.ToArray());
            result ^= this.DictionaryOfBase == null ? 0 : Util.ComputeArrayHashCode(new List<string>(this.DictionaryOfBase.Keys).ToArray());
            result ^= this.DictionaryOfBase == null ? 0 : Util.ComputeArrayHashCode(new List<BaseType>(this.DictionaryOfBase.Values).ToArray());
            result ^= this.DictionaryOfInterface == null ? 0 : Util.ComputeArrayHashCode(new List<string>(this.DictionaryOfInterface.Keys).ToArray());
            result ^= this.DictionaryOfInterface == null ? 0 : Util.ComputeArrayHashCode(new List<IEmptyInterface>(this.DictionaryOfInterface.Values).ToArray());
            return result;
        }
    }

    public class Person
    {
        internal const string Letters = "abcdefghijklmnopqrstuvwxyz";

        public Person()
        {
            this.Friends = new List<Person>();
        }

        public Person(Random rndGen)
        {
            this.Name = PrimitiveCreator.CreateInstanceOfString(rndGen, rndGen.Next(5, 15), Letters);
            this.Age = PrimitiveCreator.CreateInstanceOfInt32(rndGen);
            this.Address = new Address(rndGen);
            this.Friends = new List<Person>();
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public Address Address { get; set; }

        public List<Person> Friends { get; set; }

        public void AddFriends(int count, Random rndGen)
        {
            for (int i = 0; i < count; i++)
            {
                this.Friends.Add(new Person(rndGen));
            }
        }

        public string FriendsToString()
        {
            string s = "";

            foreach (Person p in this.Friends)
            {
                s += p + ",";
            }

            return s;
        }

        public override string ToString()
        {
            return string.Format("Person{{{0}, {1}, [{2}], Friends=[{3}]}}", this.Name, this.Age, this.Address, this.FriendsToString());
        }
    }

    public class Address
    {
        public Address()
        {
        }

        public Address(Random rndGen)
        {
            this.Street = PrimitiveCreator.CreateInstanceOfString(rndGen, rndGen.Next(5, 15), Person.Letters);
            this.City = PrimitiveCreator.CreateInstanceOfString(rndGen, rndGen.Next(5, 15), Person.Letters);
            this.State = PrimitiveCreator.CreateInstanceOfString(rndGen, rndGen.Next(5, 15), Person.Letters);
        }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public override string ToString()
        {
            return string.Format("Address{{{0}, {1}, {2}}}", this.Street, this.City, this.State);
        }
    }
}

