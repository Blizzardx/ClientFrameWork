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

namespace NetWork.Auto
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CreateNewCharRequest : TBase
  {
    private string _charName;
    private sbyte _gender;
    private sbyte _age;
    private int _modelId;

    public string CharName
    {
      get
      {
        return _charName;
      }
      set
      {
        __isset.charName = true;
        this._charName = value;
      }
    }

    public sbyte Gender
    {
      get
      {
        return _gender;
      }
      set
      {
        __isset.gender = true;
        this._gender = value;
      }
    }

    public sbyte Age
    {
      get
      {
        return _age;
      }
      set
      {
        __isset.age = true;
        this._age = value;
      }
    }

    public int ModelId
    {
      get
      {
        return _modelId;
      }
      set
      {
        __isset.modelId = true;
        this._modelId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool charName;
      public bool gender;
      public bool age;
      public bool modelId;
    }

    public CreateNewCharRequest() {
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
            if (field.Type == TType.String) {
              CharName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 20:
            if (field.Type == TType.Byte) {
              Gender = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 30:
            if (field.Type == TType.Byte) {
              Age = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 40:
            if (field.Type == TType.I32) {
              ModelId = iprot.ReadI32();
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
      TStruct struc = new TStruct("CreateNewCharRequest");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (CharName != null && __isset.charName) {
        field.Name = "charName";
        field.Type = TType.String;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(CharName);
        oprot.WriteFieldEnd();
      }
      if (__isset.gender) {
        field.Name = "gender";
        field.Type = TType.Byte;
        field.ID = 20;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(Gender);
        oprot.WriteFieldEnd();
      }
      if (__isset.age) {
        field.Name = "age";
        field.Type = TType.Byte;
        field.ID = 30;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(Age);
        oprot.WriteFieldEnd();
      }
      if (__isset.modelId) {
        field.Name = "modelId";
        field.Type = TType.I32;
        field.ID = 40;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ModelId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CreateNewCharRequest(");
      sb.Append("CharName: ");
      sb.Append(CharName);
      sb.Append(",Gender: ");
      sb.Append(Gender);
      sb.Append(",Age: ");
      sb.Append(Age);
      sb.Append(",ModelId: ");
      sb.Append(ModelId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
