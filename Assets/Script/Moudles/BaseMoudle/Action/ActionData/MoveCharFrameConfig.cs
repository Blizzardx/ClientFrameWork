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
  public partial class MoveCharFrameConfig : TBase
  {
    private ECharType _charType;
    private List<CharSpeedMove> _lstSpeedMove;

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

    public List<CharSpeedMove> LstSpeedMove
    {
      get
      {
        return _lstSpeedMove;
      }
      set
      {
        __isset.lstSpeedMove = true;
        this._lstSpeedMove = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool charType;
      public bool lstSpeedMove;
    }

    public MoveCharFrameConfig() {
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
            if (field.Type == TType.List) {
              {
                LstSpeedMove = new List<CharSpeedMove>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  CharSpeedMove _elem6 = new CharSpeedMove();
                  _elem6 = new CharSpeedMove();
                  _elem6.Read(iprot);
                  LstSpeedMove.Add(_elem6);
                }
                iprot.ReadListEnd();
              }
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
      TStruct struc = new TStruct("MoveCharFrameConfig");
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
      if (LstSpeedMove != null && __isset.lstSpeedMove) {
        field.Name = "lstSpeedMove";
        field.Type = TType.List;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, LstSpeedMove.Count));
          foreach (CharSpeedMove _iter7 in LstSpeedMove)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MoveCharFrameConfig(");
      sb.Append("CharType: ");
      sb.Append(CharType);
      sb.Append(",LstSpeedMove: ");
      sb.Append(LstSpeedMove);
      sb.Append(")");
      return sb.ToString();
    }

  }

}