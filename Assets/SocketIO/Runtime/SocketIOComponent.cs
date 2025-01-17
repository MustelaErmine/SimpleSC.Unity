#region License

/*
 * SocketIO.cs
 *
 * The MIT License
 *
 * (origin) Copyright (c) 2014 Fabio Panettieri
 * Copyright (c) 2023 Sergej Görzen
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using WebSocketSharp;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace SocketIO
{
    public class SocketIOComponent : MonoBehaviour
    {
        #region Public Properties

        public string url = "ws://127.0.0.1:4567/socket.io/?EIO=3&transport=websocket";
        public bool autoConnect;
        public int reconnectDelay = 5;
        public float ackExpirationTime = 30f;
        public float pingInterval = 25f;
        public float pingTimeout = 60f;

        public float onFailRetryInSeconds = 30.0f;
        
        private WebSocket Socket { get; set; }

        private string sid { get; set; }

        public bool IsConnected => _connected;
        #endregion

        #region Failure Management
        public event Action<string> OnFailed;
        public event Action OnReconnect;
        public string FailMessage { get; private set; } = null;
        public bool IsFailedToConnect => FailMessage != null;
        private float _failCooldown;
        private bool _wasFailed;
        #endregion

        #region Private Properties

        private volatile bool _connected;
        private volatile bool _thPinging;
        private volatile bool _thPong;
        private volatile bool _wsConnected;

        private Thread _socketThread;
        private Thread _pingThread;

        private readonly Dictionary<string, List<Action<SocketIOEvent>>> _handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
        private readonly List<Ack> _ackList = new List<Ack>();

        private int _packetId;

        private readonly Queue<SocketIOEvent> _eventQueue = new Queue<SocketIOEvent>();

        private readonly Queue<Packet> _ackQueue = new Queue<Packet>();

        private bool _isOpen;

        private Dictionary<string, string> _customHeaders = new Dictionary<string, string>();
        
        #endregion

        #region Unity interface

        private void Init()
        {
            lock (_ackList) _ackList.Clear();

                sid = null;
            _packetId = 0;

            Socket = new WebSocket(url);
            Socket.OnOpen += (s, e) => OnOpen();
            Socket.OnMessage += OnMessage;
            Socket.OnError += OnError;
            Socket.OnClose += (s, e) => OnClose();
            _wsConnected = false;

            lock (_eventQueue) _eventQueue.Clear();
            lock (_ackQueue) _ackQueue.Clear();

            _connected = false;
        }

        private void Awake()
        {
            Init();
        }

        public void Start()
        {
            if (autoConnect)
            {
                Connect();
            }
        }

        private void EmitQueued()
        {
            lock (_eventQueue)
            {
                while (_eventQueue.Count > 0)
                {
                    EmitEvent(_eventQueue.Dequeue());
                }
            }
        }

        private void Fail(string message)
        {
            FailMessage = message;
            _failCooldown = onFailRetryInSeconds;
        }

        private void InvokeAckQueue()
        {
            lock (_ackQueue)
            {
                while (_ackQueue.Count > 0)
                {
                    InvokeAck(_ackQueue.Dequeue());
                }
            }
        }

        public void Update()
        {
            if (!_wasFailed && IsFailedToConnect)
            {
                _wasFailed = true;
                // thread safe call
                OnFailed?.Invoke(FailMessage);
            }
            
            if (_failCooldown > 0.0f)
            {
                _failCooldown -= Time.deltaTime;
                if (_failCooldown <= 0.0f)
                {
                    // reset failure, start reconnection
                    FailMessage = null;
                    _wasFailed = false;
                    Reconnect();
                }

                return;
            }
            
            if (Socket == null)
                return;

            EmitQueued();
            InvokeAckQueue();

            if (_wsConnected != Socket.IsAlive)
            {
                _wsConnected = Socket.IsAlive;
                EmitEvent(_wsConnected ? "connect" : "disconnect");
            }

            // GC expired acks
            if (_ackList.Count == 0)
                return;

            if (DateTime.Now.Subtract(_ackList[0].Time).TotalSeconds < ackExpirationTime)
                return;

            _ackList.RemoveAt(0);
        }

        public void OnDestroy()
        {
            if (_socketThread != null && _socketThread.IsAlive)
            {
                _socketThread.Abort();
            }

            if (_pingThread != null && _pingThread.IsAlive)
            {
                _pingThread.Abort();
            }
        }

        public void OnApplicationQuit() => Close();

        #endregion

        #region Public Interface

        public void SetHeader(string header, string value)
        {
            _customHeaders = (Dictionary<string, string>)Socket.CustomHeaders ?? new Dictionary<string, string>();
            _customHeaders[header] = value;
            Socket.CustomHeaders = _customHeaders;
        }

        public void Reconnect()
        {
            Init();
            Socket.CustomHeaders = _customHeaders;
            Connect();
            OnReconnect?.Invoke();
        }
        public void Connect()
        {
            _connected = true;

            _socketThread = new Thread(RunSocketThread);
            _socketThread.Start(Socket);

            _pingThread = new Thread(RunPingThread);
            _pingThread.Start(Socket);
        }

        public void Close()
        {
            EmitClose();
            _connected = false;

            if (_socketThread != null && _socketThread.IsAlive)
            {
                _socketThread.Abort();
            }

            if (_pingThread != null && _pingThread.IsAlive)
            {
                _pingThread.Abort();
            }

            _socketThread = null;
            _pingThread = null;

            Socket?.Close();
            Socket = null;
        }

        public void On(string ev, Action<SocketIOEvent> callback)
        {
            if (!_handlers.ContainsKey(ev))
            {
                _handlers[ev] = new List<Action<SocketIOEvent>>();
            }

            _handlers[ev].Add(callback);
        }

        public void Off(string ev, Action<SocketIOEvent> callback)
        {
            if (!_handlers.ContainsKey(ev))
            {
                return;
            }

            var l = _handlers[ev];
            if (!l.Contains(callback))
            {
                return;
            }

            l.Remove(callback);
            if (l.Count == 0)
            {
                _handlers.Remove(ev);
            }
        }

        public void Emit(string ev, JToken data = null) => EmitMessage(-1, data == null ? $"[\"{ev}\"]" : $"[\"{ev}\",{data}]");
        public void Emit(string ev, Action<JToken> action)
        {
            EmitMessage(++_packetId, $"[\"{ev}\"]");
            _ackList.Add(new Ack(_packetId, action));
        }

        public void Emit(string ev, JObject data, Action<JToken> action)
        {
            EmitMessage(++_packetId, $"[\"{ev}\",{data}]");
            _ackList.Add(new Ack(_packetId, action));
        }

        #endregion

        #region Private Methods

        private void RunSocketThread(object obj)
        {
            var webSocket = (WebSocket) obj;
            while (_connected)
            {
                if (webSocket.IsAlive)
                {
                    Thread.Sleep(reconnectDelay);
                }
                else
                {
                    try
                    {
                        webSocket.Connect();
                    }
                    catch (Exception ex)
                    {
                        Fail(ex.Message);
                        Close();
                        return;
                    }
                }
            }

            Close();
        }

        private void RunPingThread(object obj)
        {
            var webSocket = (WebSocket) obj;

            var timeoutMs = Mathf.FloorToInt(pingTimeout * 1000);
            var intervalMs = Mathf.FloorToInt(pingInterval * 1000);
            
            while (_connected)
            {
                if (!_wsConnected)
                {
                    Thread.Sleep(reconnectDelay);
                }
                else
                {
                    _thPinging = true;
                    _thPong = false;

                    EmitPacket(new Packet(EnginePacketType.Ping));
                    var pingStart = DateTime.Now;

                    while (webSocket.IsAlive && _thPinging &&
                           (DateTime.Now.Subtract(pingStart).TotalSeconds < timeoutMs))
                    {
                        Thread.Sleep(200);
                    }

                    if (!_thPong)
                    {
                        webSocket.Close();
                    }

                    Thread.Sleep(intervalMs);
                }
            }
        }

        private void EmitMessage(int id, string raw)
            => EmitPacket(new Packet(EnginePacketType.Message, SocketPacketType.Event, 0, "/", id, JToken.Parse(raw)));

        private void EmitClose()
        {
            EmitPacket(new Packet(EnginePacketType.Message, SocketPacketType.Disconnect, 0, "/", -1, null));
            EmitPacket(new Packet(EnginePacketType.Close));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void EmitPacket(Packet packet)
        {
            if (Socket == null || !_isOpen)
                return;
            try
            {
                Socket.Send(Encoder.Encode(packet));
            }
            catch (SocketIOException ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        
        private void OnMessage(object sender, MessageEventArgs e)
        {
            var packet = Decoder.Decode(e);

            switch (packet.enginePacketType)
            {
                case EnginePacketType.Open:
                    HandleOpen(packet);
                    break;
                case EnginePacketType.Close:
                    EmitEvent("close");
                    break;
                case EnginePacketType.Ping:
                    HandlePing();
                    break;
                case EnginePacketType.Pong:
                    HandlePong();
                    break;
                case EnginePacketType.Message:
                    HandleMessage(packet);
                    break;
                case EnginePacketType.Unknown:
                    break;
                case EnginePacketType.Upgrade:
                    break;
                case EnginePacketType.Noop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleOpen(Packet packet)
        {
            sid = packet.json["sid"]?.ToString();
            OnOpen();
        }

        private void OnOpen()
        {
            if (_isOpen)
                return;
            EmitEvent("open");
            _isOpen = true;
        }

        private void OnClose()
        {
            if (!_isOpen)
                return;
            _isOpen = false;
            EmitEvent("close");
        }

        private void HandlePing()
        {
            EmitPacket(new Packet(EnginePacketType.Pong));
        }

        private void HandlePong()
        {
            _thPong = true;
            _thPinging = false;
        }

        private void HandleMessage(Packet packet)
        {
            if (packet.json == null)
                return;

            if (packet.socketPacketType == SocketPacketType.Ack)
            {
                if (_ackList.Any(t => t.PacketId == packet.id))
                {
                    lock (_ackQueue)
                    {
                        _ackQueue.Enqueue(packet);
                    }
                    return;
                }
            }

            if (packet.socketPacketType != SocketPacketType.Event) return;
            var e = SocketIOEvent.Parse(packet.json);
            lock (_eventQueue) {
                _eventQueue.Enqueue(e);
            }
        }

        private void OnError(object sender, ErrorEventArgs e) => EmitEvent("error");
        private void EmitEvent(string type) => EmitEvent(new SocketIOEvent(type));

        private void EmitEvent(SocketIOEvent ev)
        {
            if (!_handlers.ContainsKey(ev.name))
                return;
            _handlers[ev.name].ForEach(handler => handler(ev));
        }

        private void InvokeAck(Packet packet)
        {
            for (var i = 0; i < _ackList.Count; i++)
            {
                if (_ackList[i].PacketId != packet.id)
                {
                    continue;
                }

                var ack = _ackList[i];
                _ackList.RemoveAt(i);
                ack.Invoke(packet.json);
                return;
            }
        }

        #endregion
    }
}