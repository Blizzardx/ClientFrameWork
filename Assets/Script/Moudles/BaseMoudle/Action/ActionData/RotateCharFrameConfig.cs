/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace ActionEditor
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class RotateCharFrameConfig : TBase
  {
    private ECharType _charType;
    private double _rotation;

    /// <summary>
    /// 
    /// <seealso cref="ECharType"/>
    /// </summary>
    public ECharType CharType
    {
      get
      {
        return _charType;
      }
      set
      {
        __isset.charType = true;
        this._charType = value;
      }
    }

    public double Rotation
    {
      get
      {
        return _rotation;
      }
      set
      {
        __isset.rotation = true;
        this._rotation = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool charType;
      public bool rotation;
    }

    public RotateCharFrameConfig() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I32) {
              CharType = (ECharType)iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.Double) {
              Rotation = iprot.ReadDouble();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("RotateCharFrameConfig");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.charType) {
        field.Name = "charType";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32((int)CharType);
        oprot.WriteFieldEnd();
      }
      if (__isset.rotation) {
        field.Name = "rotation";
        field.Type = TType.Double;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(Rotation);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("RotateCharFrameConfig(");
      sb.Append("CharType: ");
      sb.Append(CharType);
      sb.Append(",Rotation: ");
      sb.Append(Rotation);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
