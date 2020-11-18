using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRefrigerator.Vui
{
  public class AudioDevicePool
  {
    private List<MMDevice> mDeviceList = new List<MMDevice>();

    public AudioDevicePool()
    {
      var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.All, DeviceState.Active);
      for (var i = 0; i < devices.GetCount(); i++)
      {
        mDeviceList.Add(devices.ItemAt(i));
      }
    }

    public MMDevice GetDeviceWithId(int id)
    {
      if (mDeviceList.Count > 0 && id < mDeviceList.Count)
        return mDeviceList.ElementAt(id);

      return null;
    }

    public bool TryGetDeviceWithId(int id, out MMDevice device)
    {
      device = GetDeviceWithId(id);
      return device != null;
    }

    public bool TryGetDeviceWithId(string id, out MMDevice device)
    {
      device = null;

      if(int.TryParse(id, out var intId))
      {
        return TryGetDeviceWithId(intId, out device);
      }

      return false;
    }

    public void PrintAvailableDevices()
    {
      var i = 1;
      foreach(var device in mDeviceList)
      {
        Console.WriteLine($"{i}. {device}");
        i++;
      }
    }
  }
}
