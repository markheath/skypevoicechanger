using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SKYPE4COMLib;
using JSNet;
using NAudio.Wave;

namespace SkypeFx
{
    class MicInterceptor : IDisposable
    {
        const int Protocol = 8;
        const int MicPort = 3754;
        const int OutPort = 3755;

        Skype skype;
        TcpServer micServer;
        TcpServer outServer;
        NetworkStream outStream;
        NetworkStream micStream;
        Call call;
        //int packetSize;
        ILog log;
        SkypeBufferStream bufferStream;
        public EffectStream OutputStream { get; private set; }
        
        public MicInterceptor(ILog log, EffectChain effects)
        {
            this.log = log;
            InitSockets();
            
            
            skype = new Skype();
            ISkype iSkype = (ISkype)skype;
            if (!iSkype.Client.IsRunning)
            {
                log.Error("Skype is not running");
            }

            _ISkypeEvents_Event events = (_ISkypeEvents_Event)skype;

            events.AttachmentStatus += OnSkypeAttachmentStatus;            
            skype.CallStatus += OnSkypeCallStatus;
            skype.Error += OnSkypeError;
            skype.Attach(Protocol, false);

            bufferStream = new SkypeBufferStream(44100);
            OutputStream = new EffectStream(effects, bufferStream);
        }

        void OnSkypeError(Command command, int errorNumber, string description)
        {
            log.Error("Error {0}:{1}:{2}", command, errorNumber, description);
        }      

        void OnSkypeAttachmentStatus(TAttachmentStatus status)
        {
            log.Info("Attachment Status {0}", status);
            if (status == TAttachmentStatus.apiAttachAvailable)
            {
                skype.Attach(Protocol, false);
            }
            if (status == TAttachmentStatus.apiAttachSuccess)
            {
                log.Info("AudioIn: {0}", skype.Settings.AudioIn);
                log.Info("You can now place a call");
            }            
        }

        void OnSkypeCallStatus(Call call, TCallStatus status)
        {
            log.Info("SkypeCallStatus: {0}", status);
            if (status == TCallStatus.clsInProgress)
            {
                this.call = call;                  
                call.set_CaptureMicDevice(TCallIoDeviceType.callIoDeviceTypePort, MicPort.ToString());
                call.set_InputDevice(TCallIoDeviceType.callIoDeviceTypeSoundcard, "");
                call.set_InputDevice(TCallIoDeviceType.callIoDeviceTypePort, OutPort.ToString());
            }
            else if (status == TCallStatus.clsFinished)
            {
                call = null;
                outStream.Dispose();
                micStream.Dispose();
            }
        }

        void InitSockets()
        {
            micServer = new TcpServer(MicPort);
            outServer = new TcpServer(OutPort);
            micServer.Connect += OnMicServerConnect;
            micServer.Disconnect += new EventHandler(micServer_Disconnect);
            micServer.DataReceived += OnMicServerExecute;
            outServer.Connect += OnOutServerConnect;
            outServer.Disconnect += new EventHandler(outServer_Disconnect);
        }

        void outServer_Disconnect(object sender, EventArgs e)
        {
            log.Info("OutServer Disconnected");
            outStream = null;
        }

        void OnOutServerConnect(object sender, ConnectedEventArgs e)
        {
            log.Info("OutServer Connected");
            outStream = e.Stream;
        }

        void OnMicServerExecute(object sender, DataReceivedEventArgs args)
        {
            // log.Info("Got {0} bytes", args.Buffer.Length);
            if (outStream != null)
            {
                // give the input audio to the beginning of our audio graph
                bufferStream.SetLatestInBuffer(args.Buffer);
                // process it out through the effects
                OutputStream.Read(args.Buffer, 0, args.Buffer.Length);
                // play it back
                outStream.Write(args.Buffer, 0, args.Buffer.Length);
            }
        }

        void micServer_Disconnect(object sender, EventArgs e)
        {
            log.Info("MicServer Disconnected");
        }

        void OnMicServerConnect(object sender, ConnectedEventArgs e)
        {
            log.Info("MicServer Connected");
            micStream = e.Stream;
        }

        public void Dispose()
        {
            OutputStream.Dispose();
            micServer.Dispose();
            outServer.Dispose();
        }
    }
}
