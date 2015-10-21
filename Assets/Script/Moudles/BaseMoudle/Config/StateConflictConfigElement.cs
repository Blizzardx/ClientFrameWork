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

namespace Common.Config
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class StateConflictConfigElement : TBase
  {
    private int _stateId;
    private bool _isConflict;

    public int StateId
    {
      get
      {
        return _stateId;
      }
      set
      {
        __isset.stateId = true;
        this._stateId = value;
      }
    }

    public bool IsConflict
    {
      get
      {
        return _isConflict;
      }
      set
      {
        __isset.isConflict = true;
        this._isConflict = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool stateId;
      public bool isConflict;
    }

    public StateConflictConfigElement() {
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
            if (field.Type == TType.I32) {
              StateId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 20:
            if (field.Type == TType.Bool) {
              IsConflict = iprot.ReadBool();
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
      TStruct struc = new TStruct("StateConflictConfigElement");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.stateId) {
        field.Name = "stateId";
        field.Type = TType.I32;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(StateId);
        oprot.WriteFieldEnd();
      }
      if (__isset.isConflict) {
        field.Name = "isConflict";
        field.Type = TType.Bool;
        field.ID = 20;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(IsConflict);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("StateConflictConfigElement(");
      sb.Append("StateId: ");
      sb.Append(StateId);
      sb.Append(",IsConflict: ");
      sb.Append(IsConflict);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
