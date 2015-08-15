﻿using System;
using System.Net;
using System.Net.Sockets;
using Thrift.Protocol;
using UnityEngine;
using System.Collections;

public class NetWorkManager : Singleton<NetWorkManager>
{
    public enum SocketStatus
    {
        Idle,
        Connecting,
        Sending,
        Reciving,
        Closing,
    }
    private Socket              m_Socket;
    private SocketStatus        m_Status;
    private MessageBufferTool   m_BufferTool;
    private const int           DEFAULT_RECEIVE_SIZE    = 64 * 1024;
    private const int           DEFAULT_SEND_SIZE       = 32 * 1024;

    #region public interface

    public NetWorkManager()
    {
        Initialize();
    }
    public void Connect(string ip, int port)
    {
        if (null != m_Socket)
        {
            Close();
        }
        m_Socket                    = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Socket.Blocking           = false;
        m_Socket.ReceiveBufferSize  = DEFAULT_RECEIVE_SIZE;
        m_Socket.SendBufferSize     = DEFAULT_SEND_SIZE;
        m_Socket.ReceiveTimeout     = 30000;
        m_Socket.SendTimeout        = 30000;
        IPEndPoint remoteEP         = new IPEndPoint(IPAddress.Parse(ip), port);
        m_Status                    = SocketStatus.Connecting;

        m_Socket.BeginConnect(remoteEP, ConnectEventHandle, m_Socket);
    }
    public void Disconnect()
    {
        Close();
    }
    public SocketStatus GetNetworkStatus
    {
        get
        {
            return m_Status;
        }
    }
    public void SendMsgToServer(object msgValue)
    {
        if (CheckSocketStatus())
        {
            PingTickTask.ResetSendMsgTime();
            m_BufferTool.EncodeGamePackage(msgValue);
            Send();
        }
    }
    public void RegisterMessage(int id, Type handler)
    {
        m_BufferTool.RegisterMessage(id, handler);
    }
    public void RegisterMessage()
    {
        m_BufferTool.RegisterMessage();
    }
    public bool IsConnected()
    {
        return m_Socket != null && m_Socket.Connected;
    }
    public void RestSocketStatus()
    {
        Initialize();
        if (null != m_Socket)
        {
            m_Socket.Close();
        }
        m_Socket = null;
    }
    #endregion

    #region system function
    private void Initialize()
    {  
        m_BufferTool = new MessageBufferTool();
        m_BufferTool.Initialize();
    }
    private void Send()
    {
        if (m_Status == SocketStatus.Idle || m_Status == SocketStatus.Closing)
        {
            return;
        }
        m_Status = SocketStatus.Sending;
        m_Socket.BeginSend(m_BufferTool.GetSendBuffer(), 0, m_BufferTool.GetSendBufferSize(), 0, SendEventHandle, m_Socket);
    }
    private void Receive()
    {
        try
        {
            m_Status = SocketStatus.Reciving;
            m_Socket.BeginReceive(m_BufferTool.GetRecieveBuffer(), 0, MessageBufferTool.MAXLength, SocketFlags.None, ReceiveEventHandle, m_Socket);
        }
        catch (Exception)
        {
            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_SOCKET_CLOSE, null));
        }
       
    }
    private void Close()
    {
        if (null == m_Socket)
        {
            return;
        }
        m_Socket.Close();
        m_Status = SocketStatus.Idle;
    }
    private void ConnectEventHandle(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;
        if (client.Connected)
        {
            Debug.Log("Connected");
            TickTaskManager.Instance.SetTickTaskStatus(TickTaskManager.TickTaskType.PingTickTask, true);
        }
        else
        {
            Debug.Log("Connected error");
        }
        client.EndConnect(ar);
        Receive();
    }
    private void ReceiveEventHandle(IAsyncResult ar)
    {
        try
        {
            int size = m_Socket.EndReceive(ar);
            m_BufferTool.RecieveMsg(size);
            Debuger.Log("size = " + size);

            if (CheckSocketStatus())
            {
                m_Socket.BeginReceive(m_BufferTool.GetRecieveBuffer(), 0, MessageBufferTool.MAXLength, SocketFlags.None, ReceiveEventHandle, m_Socket);
            }
        }
        catch (Exception)
        {
            // 
            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_SOCKET_CLOSE, null));
        }
        
    }
    private void SendEventHandle(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;

        client.EndSend(ar);
        m_Status = SocketStatus.Reciving;
    }
    private bool CheckSocketStatus()
    {
        bool res = m_Socket != null && m_Socket.Connected;
        if (!res)
        {
            // 
            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_SOCKET_CLOSE, null));
        }
        return res;
    }
    #endregion
}
