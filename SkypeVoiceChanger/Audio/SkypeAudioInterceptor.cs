using System;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;
using SKYPE4COMLib;

namespace SkypeVoiceChanger.Audio
{
    class SkypeAudioInterceptor : IDisposable
    {
        private const int Protocol = 8;
        private const int MicDevicePort = 3754;
        private const int InputDevicePort = 3755;
        private const int OutputDevicePort = 3756;

        private readonly ILog log;
        private readonly ISkype skype;
        private readonly IAudioProcessor audioProcessor;

        private TcpServer micServer;
        private TcpServer inputDeviceServer;
        private TcpServer outputDeviceServer;
        private NetworkStream inputDeviceStream;
        private NetworkStream micStream;
        private NetworkStream outputDeviceStream;
        private Call currentCall;
        private string originalSoundCard;
        private SkypeStatus skypeStatus;
        private readonly Timer timer;

        public event EventHandler SkypeStatusChanged;

        public SkypeAudioInterceptor(ISkype skype, _ISkypeEvents_Event skypeEvents, ILog log, IAudioProcessor audioProcessor)
        {
            this.log = log;
            this.audioProcessor = audioProcessor;

            this.timer = new Timer();
            timer.Interval = 500; // TimeSpan.FromMilliseconds(500);
            timer.Tick += TimerOnTick;
            timer.Start();

            
            InitSockets();

            this.skype = skype;
            if (!skype.Client.IsRunning)
            {
                log.Error("Skype is not running - check you have installed and started the desktop version of Skype");
            }

            skypeEvents.AttachmentStatus += OnSkypeAttachmentStatus;
            skypeEvents.CallStatus += OnSkypeCallStatus;
            skypeEvents.Error += OnSkypeError;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (currentCall == null && skype.AttachmentStatus == TAttachmentStatus.apiAttachSuccess && skype.ActiveCalls.Count > 0)
            {
                log.Warning("In progress call without API notification, attempting to attach...");
                foreach (Call call in skype.ActiveCalls)
                {
                    // indexer issue, so using enumeration
                    OnCallStarted(call);
                    break;
                }

                //skype.ActiveCalls[0]
            }
            else if (currentCall != null && skype.ActiveCalls.Count == 0)
            {
                log.Info("Call has ended without API notification");
                // missed a call ending
                OnCallFinished(false);
            }
        }

        public void Attach()
        {
            skype.Attach(Protocol, false);
        }

        public SkypeStatus SkypeStatus
        {
            get { return skypeStatus; }
            set
            {
                if (skypeStatus != value)
                {
                    skypeStatus = value;
                    RaiseSkypeStatusChanged();
                }
            }
        }

        private void RaiseSkypeStatusChanged()
        {
            var handler = SkypeStatusChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
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
            else if (status == TAttachmentStatus.apiAttachPendingAuthorization)
            {
                SkypeStatus = SkypeStatus.PendingAuthorisation;
            }
            else if (status == TAttachmentStatus.apiAttachSuccess)
            {
                log.Info("AudioIn: {0}", skype.Settings.AudioIn);
                log.Info("You can now place a currentCall");
                SkypeStatus = SkypeStatus.WaitingForCall;
            }
            else if (status == TAttachmentStatus.apiAttachNotAvailable)
            {
                log.Warning("No longer connected to Skype");
                SkypeStatus = SkypeStatus.SkypeNotRunning;
            }
            else if (status == TAttachmentStatus.apiAttachRefused)
            {
                log.Error("Attach refused");
                SkypeStatus = SkypeStatus.SkypeAttachRefused;
            }
        }

        void OnSkypeCallStatus(Call call, TCallStatus status)
        {
            log.Info("SkypeCallStatus: {0}", status);
            if (status == TCallStatus.clsInProgress)
            {
                OnCallStarted(call);
            }
            else if (status == TCallStatus.clsFinished)
            {
                OnCallFinished(false);
            }
            else
            {
                log.Info("SkypeCallStatus: {0}", status);
            }
        }

        private void OnCallFinished(bool detach)
        {
            if (currentCall == null) return;
            log.Info("Call finished");

            // we are shutting down so just put the soundcard back the way it was
            if (detach)
            {
                currentCall.InputDevice[TCallIoDeviceType.callIoDeviceTypePort] = "";
                currentCall.InputDevice[TCallIoDeviceType.callIoDeviceTypeSoundcard] = originalSoundCard;
                currentCall.CaptureMicDevice[TCallIoDeviceType.callIoDeviceTypePort] = "";
                currentCall.OutputDevice[TCallIoDeviceType.callIoDeviceTypePort] = "";
            }

            SkypeStatus = SkypeStatus.WaitingForCall;
            currentCall = null;
            if (inputDeviceStream != null)
            {
                inputDeviceStream.Dispose();
                inputDeviceStream = null;
            }
            if (micStream != null)
            {
                micStream.Dispose();
                micStream = null;
            }
        }

        private void OnCallStarted(Call call)
        {
            if (currentCall != null)
            {
                log.Warning("conference calls not supported");
                return;
            }

            currentCall = call;
            call.CaptureMicDevice[TCallIoDeviceType.callIoDeviceTypePort] = MicDevicePort.ToString(CultureInfo.InvariantCulture);
            originalSoundCard = call.InputDevice[TCallIoDeviceType.callIoDeviceTypeSoundcard];
            call.InputDevice[TCallIoDeviceType.callIoDeviceTypeSoundcard] = "";
            call.InputDevice[TCallIoDeviceType.callIoDeviceTypePort] = InputDevicePort.ToString(CultureInfo.InvariantCulture);
            call.OutputDevice[TCallIoDeviceType.callIoDeviceTypePort] = OutputDevicePort.ToString(CultureInfo.InvariantCulture);
            SkypeStatus = SkypeStatus.CallInProgress;
        }

        private void InitSockets()
        {
            micServer = new TcpServer(MicDevicePort);
            micServer.Connect += OnMicServerConnect;
            micServer.Disconnect += OnMicServerDisconnect;
            micServer.DataReceived += OnMicServerDataReceived;

            inputDeviceServer = new TcpServer(InputDevicePort);
            inputDeviceServer.Connect += OnInputDeviceServerConnect;
            inputDeviceServer.Disconnect += OnInputDeviceServerDisconnect;
            inputDeviceServer.DataReceived += OnInputDeviceServerDataReceived;

            outputDeviceServer = new TcpServer(OutputDevicePort);
            outputDeviceServer.Connect += OnOutputDeviceServerConnect;
            outputDeviceServer.Disconnect += OnOutputDeviceServerDisconnect;
            outputDeviceServer.DataReceived += OnOutputDeviceServerDataReceived;
        }

        void OnInputDeviceServerDisconnect(object sender, EventArgs e)
        {
            log.Info("Input Device Server Disconnected");
            inputDeviceStream = null;
        }

        void OnInputDeviceServerConnect(object sender, ConnectedEventArgs e)
        {
            log.Info("Input Device Server Connected");
            inputDeviceStream = e.Stream;
        }

        void OnInputDeviceServerDataReceived(object sender, DataReceivedEventArgs args)
        {
            // for recording : this the outgoing audio (possibly not needed since we are writing to this stream)
        }

        void OnOutputDeviceServerDisconnect(object sender, EventArgs e)
        {
            log.Info("Output Device Server Disconnected");
            outputDeviceStream = null;
        }

        void OnOutputDeviceServerConnect(object sender, ConnectedEventArgs e)
        {
            log.Info("Output Device Server Connected");
            outputDeviceStream = e.Stream;
        }

        void OnOutputDeviceServerDataReceived(object sender, DataReceivedEventArgs args)
        {
            // for recording : this the incoming audio
            audioProcessor.ProcessIncoming(args.Buffer, args.Buffer.Length);
        }


        void OnMicServerDataReceived(object sender, DataReceivedEventArgs args)
        {
            // log.Info("Got {0} bytes", args.Buffer.Length);
            if (inputDeviceStream != null)
            {
                // process the input audio through audio pipeline
                audioProcessor.ProcessOutgoing(args.Buffer, args.Buffer.Length);
                // play it back
                inputDeviceStream.Write(args.Buffer, 0, args.Buffer.Length);
            }
        }

        void OnMicServerDisconnect(object sender, EventArgs e)
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
            timer.Stop();
            OnCallFinished(true); // clean up a current call
            micServer.Dispose();
            inputDeviceServer.Dispose();
            outputDeviceServer.Dispose();
        }
    }
}
