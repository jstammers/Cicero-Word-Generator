using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.ModularInstruments.Interop;
using DataStructures;

namespace AtticusServer
{
    public class HSDIOTask : MarshalByRefObject
    {
        /// <summary>
        /// This class mimicks some of the functionality of DAQmx for NI-HSDIO cards. The buffer is generated as a digital waveform in a similar way to DAQmx digital tasks.
        /// </summary>
        /// 
        public static void createHSDIOWaveForm(AtticusServerCommunicator sender, string deviceName, DeviceSettings deviceSettings, SequenceData sequence, SettingsData settings, Dictionary<int, HardwareChannel> usedDigitalChannels, ServerSettings serverSettings, out long expectedSamplesGenerated)
        {
            niHSDIO hsdio = niHSDIO.InitGenerationSession(deviceName, true, false, "");
            hsdio.AssignDynamicChannels("0");
            hsdio.ConfigureGenerationMode(niHSDIOConstants.Waveform);
            hsdio.ConfigureScriptToGenerate("waveform");
            Dictionary<int, bool[]> digitalValues;
            expectedSamplesGenerated = 0;
            #region NON variable timebase buffer
            if (deviceSettings.UsingVariableTimebase == false)
            {
                double timeStepSize = Common.getPeriodFromFrequency(deviceSettings.SampleClockRate);
                int nSamples = sequence.nSamples(timeStepSize);
                if (nSamples % 8 != 0)
                {
                    //The HSDIO card expects data as a list of bytes.
                    int remainder = nSamples % 8;
                    nSamples += (8-remainder);
                }
                if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.DerivedFromMaster)
                {
                    //Uses the onboard clock
                    hsdio.ConfigureSampleClock(niHSDIOConstants.OnBoardClockStr, deviceSettings.SampleClockRate);
                }
                else if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.External && deviceSettings.SampleClockExternalSource == "ClkIn")
                {
                    //If set to ClkIn, the clock is configured to use the ClkIn input on the HSDIO card
                    hsdio.ConfigureSampleClock(niHSDIOConstants.ClkInStr, deviceSettings.SampleClockRate);
                }
                Dictionary<int, string> hsChannels = countHSChannels(usedDigitalChannels);
                if (hsChannels.Count != 0)
                {
                    //HSDIO cards typically have 32 channels, but we may want to reserve some for defining clock signals (in which case it may be slightly trickier to generate a digital sequence)
                    //This code goes through every enabled channel and parses the sequence data into a list of booleans for each value
                    if (deviceSettings.DigitalHardwareStructure[0] == 32)
                    {
                        bool[] singleChannelBuffer;
                        byte[] byteBuffer;
                        List<byte> hsdioChannelData = new List<byte>();
                        List<string> channelNames = new List<string>();
                        foreach (string hc in hsChannels.Values)
                        {
                            channelNames.Add(hc);
                        }
                        try
                        {
                            singleChannelBuffer = new bool[nSamples];
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Unable to allocate digital buffer for HSDIO device " + deviceName + ". Reason: " + e.Message + "\n" + e.StackTrace);
                        }
                        for (int channel = 0; channel < deviceSettings.DigitalHardwareStructure[0]; channel++)
                        {
                            byteBuffer = new byte[nSamples / 8];
                            string hsChannel = "hs" + channel.ToString();
                            if (channelNames.Contains(hsChannel))
                            {
                                int digitalID = hsChannels.FirstOrDefault(x => x.Value == hsChannel).Key;
                                sequence.computeDigitalBuffer(digitalID, timeStepSize, singleChannelBuffer);
                                byteBuffer = convertToByteList(singleChannelBuffer);
                            }
                            hsdioChannelData.AddRange(byteBuffer);
                        }
                        byte[] hsdioWaveform = hsdioChannelData.ToArray();
                        if(nSamples * 32 != hsdioWaveform.Length*8)
                        {
                            throw new Exception(deviceName + ": Total number of samples is not equal to the Waveform length");
                        }
                        try
                        {
                            hsdio.WriteNamedWaveformWDT("waveform", nSamples * 32, niHSDIOConstants.GroupByChannel, hsdioWaveform);
                        }
                        catch (System.AccessViolationException e)
                        {
                            throw new Exception("Couldn't access memory on HSDIO card.");
                        }
                        expectedSamplesGenerated = nSamples;
                    }
                }
            }
            if (deviceSettings.StartTriggerType == DeviceSettings.TriggerType.TriggerIn)
            {
                hsdio.ConfigureDigitalEdgeStartTrigger(deviceSettings.TriggerInPort, niHSDIOConstants.RisingEdge);
            }
            #endregion
        }

        public static Dictionary<int, string> countHSChannels(Dictionary<int, HardwareChannel> digitalChannels)
        {
            Dictionary<int, string> hcIDs = new Dictionary<int, string>();
            //returns a list of the digitalIDs for the channels belonging to an HSDIO card
            foreach (int digitalID in digitalChannels.Keys)
            {
                HardwareChannel hc = digitalChannels[digitalID];
                if (hc.ChannelName.Contains("hs"))
                    hcIDs.Add(digitalID, hc.ChannelName);
            }
            return hcIDs;
        }
        public static byte[] convertToByteList(bool[] boolList)
        {
            //Packs a list of bools into a list of bytes
            int boolLength = boolList.Length;
            if (boolLength % 8 != 0)
                throw new System.ArgumentException("sequence is not of a suitable length to convert to bytes");
            int byteLength = boolLength / 8;
            byte[] byteList = new byte[byteLength];
            int bitIndex = 0; int byteIndex = 0;
            for (int i = 0; i < boolLength; i++)
            {
                if (boolList[i])
                    byteList[byteIndex] |= (byte)(((byte)1) << bitIndex);
                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }
            return byteList;
        }
    }
}
