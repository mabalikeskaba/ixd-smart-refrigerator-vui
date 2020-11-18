using CSCore.CoreAudioAPI;
using Newtonsoft.Json;
using SmartRefrigeratorVui;
using System;
using System.Linq;
using System.IO;
using System.Speech.Synthesis;
using System.Collections.Generic;

namespace SmartRefrigerator.Vui
{
  public class Dialog
  {
    private readonly SpeechSynthesizer mSpeech;
    private VoiceTranscriptor mVoiceTranscriptor;
    private Refrigerator mFridge;
    private DialogStructure mDialogNodes;

    public Dialog()
    {
      mSpeech = new SpeechSynthesizer();
      mSpeech.SelectVoice("Microsoft Zira Desktop");
      mFridge = new Refrigerator();
      mDialogNodes = JsonConvert.DeserializeObject<DialogStructure>(File.ReadAllText("dialog.json"));
    }

    public void Begin()
    {
      SelectAudioDevice();

      InteractWithFridge();
    }

    private void SelectAudioDevice()
    {
      var audioDevicePool = new AudioDevicePool();

      Console.WriteLine("Please select your preferred audio device:");
      Console.WriteLine("###################################################");
      audioDevicePool.PrintAvailableDevices();
      Console.WriteLine("###################################################");
      MMDevice audioDevice = null;
      while (audioDevice == null)
      {
        Console.Write("Enter device id: ");
        if (audioDevicePool.TryGetDeviceWithId(Console.ReadLine(), out var device))
        {
          audioDevice = device;
        }
      }
      mVoiceTranscriptor = new VoiceTranscriptor(audioDevice);
    }

    private void InteractWithFridge()
    {
      Console.WriteLine("###################################################");
      while (mFridge.HasEnoughItems)
      {
        Console.WriteLine("The fridge contains following items:");
        mFridge.PrintFridgeContent();
        Console.WriteLine("Press any key to remove a random item...");
        Console.ReadKey();
        Console.WriteLine($"Removed item: {mFridge.RemoveRandomItem()}");
      }

      StartDialog();
    }

    private void StartDialog()
    {
      var currentNode = mDialogNodes.DialogNodes.First(x => x.Id == 1);
      var didNotUnderstand = false;
      var didNotUnderstandText = "I'm sorry I did not understand. Could you repeat that again?";

      while (!currentNode.IsEndNode)
      {
        if (!didNotUnderstand)
          Speak(currentNode.SpeechText);
        else
          Speak(didNotUnderstandText);

        if (!currentNode.IsListenOnly)
        {
          Console.WriteLine("Speak for a response and finish with enter");
          mVoiceTranscriptor.GetNextRecording().Wait();
          Console.WriteLine(mVoiceTranscriptor.Transcription);
        }
        var nextNode = GetNextNodeFromAnswer(currentNode, mVoiceTranscriptor.Transcription);
        foreach (var item in currentNode.ItemList)
        {
          Speak(item);
        }

        didNotUnderstand = nextNode == null;
        currentNode = !didNotUnderstand ? nextNode : currentNode;
      }
      Speak(currentNode.SpeechText);
    }

    private DialogNode GetNextNodeFromAnswer(DialogNode currentNode, string answerString)
    {
      // Skip the mic input when answer node is 0
      if (currentNode.IsListenOnly)
        return mDialogNodes.DialogNodes.First(x => x.Id == currentNode.AnswerNodeIds.First().DialogNode);

      foreach (var pathNode in currentNode.AnswerNodeIds)
      {
        var node = mDialogNodes.AnswerNodes.First(x => x.Id == pathNode.AnswerNode);
        if (node.IsListAnswer)
        {
          currentNode.ItemList.Add(answerString);
          break;
        }
        foreach (var answer in answerString.Split(' '))
        {
          if (node.Keywords.Contains(answer.ToLower()))
          {
            return mDialogNodes.DialogNodes.First(x => x.Id == pathNode.DialogNode);
          }
        }
      }

      return null;
    }

    private void Speak(string text)
    {
      mSpeech.Speak(text);
    }
  }
}
