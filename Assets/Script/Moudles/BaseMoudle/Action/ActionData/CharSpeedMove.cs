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
  public partial class CharSpeedMove : TBase
  {
    private double _speed;
    private Common.Auto.ThriftVector3 _target;

    public double Speed
    {
      get
      {
        return _speed;
      }
      set
      {
        __isset.speed = true;
        this._speed = value;
      }
    }

    public Common.Auto.ThriftVector3 Target
    {
      get
      {
        return _target;
      }
      set
      {
        __isset.target = true;
        this._target = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool speed;
      public bool target;
    }

    public CharSpeedMove() {
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
          case 10:
            if (field.Type == TType.Double) {
              Speed = iprot.ReadDouble();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 20:
            if (field.Type == TType.Struct) {
              Target = new Common.Auto.ThriftVector3();
              Target.Read(iprot);
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
      TStruct struc = new TStruct("CharSpeedMove");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.speed) {
        field.Name = "speed";
        field.Type = TType.Double;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(Speed);
        oprot.WriteFieldEnd();
      }
      if (Target != null && __isset.target) {
        field.Name = "target";
        field.Type = TType.Struct;
        field.ID = 20;
        oprot.WriteFieldBegin(field);
        Target.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CharSpeedMove(");
      sb.Append("Speed: ");
      sb.Append(Speed);
      sb.Append(",Target: ");
      sb.Append(Target== null ? "<null>" : Target.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
