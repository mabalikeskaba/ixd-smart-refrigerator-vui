using System.Speech.Synthesis;

namespace SmartRefrigerator.Vui
{
  public class VoiceSynthesizer
  {
    private readonly SpeechSynthesizer mSpeech;
    
    private static VoiceSynthesizer mInstance;

    public static VoiceSynthesizer Instance()
    {
      if(mInstance == null)
      {
        mInstance = new VoiceSynthesizer();
      }
      return mInstance;
    }

    public VoiceSynthesizer()
    {
      mSpeech = new SpeechSynthesizer();
      mSpeech.SelectVoice("Microsoft Zira Desktop");
    }

    public void Speak(string text)
    {
      mSpeech.Speak(text);
    }
  }
}
