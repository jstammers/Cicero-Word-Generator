using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.ModularInstruments.Interop;
using DataStructures;

namespace AtticusServer
{
    public class HSDIOTask
    {
        /// <summary>
        /// This class mimicks some of the functionality of DAQmx for NI-HSDIO cards. The buffer is generated as a digital waveform in a similar way to DAQmx digital tasks.
        /// </summary>
        /// 
       public static void createHSDIOWaveForm(AtticusServerCommunicator sender, string deviceName,DeviceSettings deviceSettings,SequenceData sequence, SettingsData settings, Dictionary<int, HardwareChannel> usedDigitalChannels, ServerSettings serverSettings, out long expectedSamplesGenerated)
        {
            niHSDIO hsdio = niHSDIO.InitGenerationSession(deviceName, true, false, "");
            Dictionary<int, bool[]> digitalValues;
            expectedSamplesGenerated = 0;
            #region NON variable timebase buffer
            if (deviceSettings.UsingVariableTimebase == false)
            {
                double timeStepSize = Common.getPeriodFromFrequency(deviceSettings.SampleClockRate);
                int nSamples = sequence.nSamples(timeStepSize);
                if (nSamples%8!=0)
                {
                    //The HSDIO card expects data as a list of bytes.
                    int remainder = nSamples % 8;
                    nSamples += remainder;
                }
                if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.DerivedFromMaster)
                {
                    //Uses the onboard clock
                    hsdio.ConfigureSampleClock("NIHSDIO_VAL_ON_BOARD_CLOCK_STR", deviceSettings.SampleClockRate);
                }
                else if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.External && deviceSettings.SampleClockExternalSource=="ClkIn")
                {
                    //If set to ClkIn, the clock is configured to use the ClkIn input on the HSDIO card
                    hsdio.ConfigureSampleClock("NIHSDIO_VAL_CLK_IN_STR", deviceSettings.SampleClockRate);
                }
                int hsCount = countHSChannels(usedDigitalChannels);
                if (hsCount != 0)
                {
                    //HSDIO cards typically have 32 channels, but we may want to reserve some for defining clock signals (in which case it may be slightly trickier to generate a digital sequence)
                    //This code goes through every enabled channel and parses the sequence data into a list of booleans for each value
                    if (deviceSettings.DigitalHardwareStructure[0] == 32)
                    {
                        UInt32[,] digitalBuffer;
                        bool[] singleChannelBuffer;
                        List<string> channelNames = new List<string>();
                        foreach(HardwareChannel hc in usedDigitalChannels.Values)
                        {
                            channelNames.Add(hc.ChannelName);
                        }
                        try
                        {
                            digitalBuffer = new UInt32[hsCount, nSamples];
                            singleChannelBuffer = new bool[nSamples];
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Unable to allocate digital buffer for HSDIO device " + deviceName + ". Reason: " + e.Message + "\n" + e.StackTrace);
                        }
                        for (int i = 0; i<hsCount;i++)
                        {
                           // int digNum = usedDigitalChannels[i];
                            UInt32 digitalBitMask = 1;
                            for (int channel = 0; channel < deviceSettings.DigitalHardwareStructure[0];channel++)
                            {
                                string hsChannel = "hs" + channel.ToString();
                                if (channelNames.Contains(hsChannel))
                                {
                                    for(int j = 0; j<nSamples;j++)
                                    {
                                        sequence.computeDigitalBuffer(channel, timeStepSize, singleChannelBuffer);
                                    }
                                }
                                

                            }
                        }
                    }
                 }
               }
            #endregion
        }
        public static int countHSChannels(Dictionary<int, HardwareChannel> digitalChannels)
        {
            int hcCount = 0;
            //returns the number of enabled channels belonging to an HSDIO card
            foreach(int digitalID in digitalChannels.Keys)
            {
                HardwareChannel hc = digitalChannels[digitalID];
                if (hc.ChannelName.Contains("hs"))
                    hcCount += 1;
            }
            return hcCount;
        }
        public static void parseDigitalIDs(AtticusServerCommunicator sender, string deviceName,DeviceSettings deviceSettings,Dictionary<int,HardwareChannel> usedDigitalChannels, out List<int> digitalIDs )
        {
            digitalIDs = new List<int>();
            if (deviceSettings.DeviceDescription.Contains("6541"))
            {

            }
        }

    }
    
}
