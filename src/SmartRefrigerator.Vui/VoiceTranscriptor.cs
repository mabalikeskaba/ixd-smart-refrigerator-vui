// VoiceTranscriptor code is based on https://github.com/mozilla/DeepSpeech-examples

using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using DeepSpeechClient;
using DeepSpeechClient.Interfaces;
using DeepSpeechClient.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SmartRefrigerator.Vui
{
  public class VoiceTranscriptor
  {
    private const string SPEECH_MODEL = "deepspeech-0.9.1-models.pbmm";
    private const int SampleRate = 16000;

    private WasapiCapture mAudioCapture;
    private SoundInSource mSoundInSource;
    private IWaveSource mConvertedSource;
    private IDeepSpeech mSttClient;
    private DeepSpeechStream mSttStream;

    private readonly ConcurrentQueue<short[]> _bufferQueue = new ConcurrentQueue<short[]>();

    public bool StreamingIsBusy { get; private set; }
    public string Transcription { get; private set; }

    public VoiceTranscriptor(MMDevice selectedDevice)
    {
      //Register instance of DeepSpeech
      mSttClient = new DeepSpeech(SPEECH_MODEL);

      InitializeAudioCapture(selectedDevice);
    }

    /// <summary>
    /// Stops the recording and sets the transcription of the closed stream.
    /// </summary>
    /// <returns>A Task to await.</returns>
    public async Task StopRecordingAsync()
    {
      try
      {
        mAudioCapture.Stop();
        while (!_bufferQueue.IsEmpty && StreamingIsBusy)
        {
          await Task.Delay(90);
        }
        Transcription = mSttClient.FinishStream(mSttStream);
      }
      catch(Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    /// <summary>
    /// Creates a new stream and starts the recording.
    /// </summary>
    public async Task GetNextRecording()
    {
      mSttStream = mSttClient.CreateStream();
      mAudioCapture.Start();
      Console.ReadLine();
      await StopRecordingAsync();
    }

    private void InitializeAudioCapture(MMDevice selectedDevice)
    {
      if (selectedDevice != null)
      {
        mAudioCapture = selectedDevice.DataFlow == DataFlow.Capture ?
            new WasapiCapture() : new WasapiLoopbackCapture();
        mAudioCapture.Device = selectedDevice;
        mAudioCapture.Initialize();
        mAudioCapture.DataAvailable += Capture_DataAvailable;
        mSoundInSource = new SoundInSource(mAudioCapture) { FillWithZeros = false };
        //create a source, that converts the data provided by the
        //soundInSource to required format
        mConvertedSource = mSoundInSource
           .ChangeSampleRate(SampleRate) // sample rate
           .ToSampleSource()
           .ToWaveSource(16); //bits per sample

        mConvertedSource = mConvertedSource.ToMono();
      }
    }

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
    {
      //read data from the converedSource
      //important: don't use the e.Data here
      //the e.Data contains the raw data provided by the 
      //soundInSource which won't have the deepspeech required audio format
      byte[] buffer = new byte[mConvertedSource.WaveFormat.BytesPerSecond / 2];

      int read;
      //keep reading as long as we still get some data
      while ((read = mConvertedSource.Read(buffer, 0, buffer.Length)) > 0)
      {
        short[] sdata = new short[(int)Math.Ceiling(Convert.ToDecimal(read / 2))];
        Buffer.BlockCopy(buffer, 0, sdata, 0, read);
        _bufferQueue.Enqueue(sdata);
        Task.Run(() => OnNewData());
      }
    }

    /// <summary>
    /// Starts processing data from the queue.
    /// </summary>
    private void OnNewData()
    {
      try
      {
        while (!StreamingIsBusy && !_bufferQueue.IsEmpty)
        {
          if (_bufferQueue.TryDequeue(out short[] buffer))
          {
            StreamingIsBusy = true;
            mSttClient.FeedAudioContent(mSttStream, buffer, Convert.ToUInt32(buffer.Length));
            StreamingIsBusy = false;
          }
        }
      }
      catch(Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}
