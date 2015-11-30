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
        niHSDIO hsdio;
        string deviceName;
        string channelList;
        public long TotalSamplesGeneratedPerChannel;


        public HSDIOTask (string deviceName, string channelList)
        {
            //initialises an instance of HSDIOTask
            hsdio = niHSDIO.InitGenerationSession(deviceName, true, false, "");
            this.deviceName = deviceName;
            this.channelList = channelList;
            this.TotalSamplesGeneratedPerChannel = 0;
            hsdio.AssignDynamicChannels(channelList);
        }
        /// <summary>
        /// This class mimicks some of the functionality of DAQmx for NI-HSDIO cards. The buffer is generated as a digital waveform in a similar way to DAQmx digital tasks.
        /// </summary>
        /// 
        public void createHSDIOWaveForm(AtticusServerCommunicator sender, string deviceName, DeviceSettings deviceSettings, SequenceData sequence, SettingsData settings, Dictionary<int, HardwareChannel> usedDigitalChannels, ServerSettings serverSettings, out long expectedSamplesGenerated)
        {
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
                    //Uses the onboard clock and exports the start trigger to the PXI back plane.
                    hsdio.ConfigureSampleClock(niHSDIOConstants.OnBoardClockStr, deviceSettings.SampleClockRate);
                    hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.PxiTrig0Str);
                    //hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.PxiTrig1Str);
                    
                }
                else if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.External && deviceSettings.SampleClockExternalSource == "ClkIn")
                {
                    //If set to ClkIn, the clock is configured to use the ClkIn input on the HSDIO card
                    hsdio.ConfigureSampleClock(niHSDIOConstants.ClkInStr, deviceSettings.SampleClockRate);
                }
                if (deviceSettings.StartTriggerType == DeviceSettings.TriggerType.SoftwareTrigger)
                {
                    hsdio.ConfigureSoftwareStartTrigger();
                }
                else if(deviceSettings.StartTriggerType == DeviceSettings.TriggerType.TriggerIn)
                {
                    string trigger = deviceSettings.TriggerInPort;
                    hsdio.ConfigureDigitalEdgeStartTrigger(trigger, niHSDIOConstants.RisingEdge);
                }
                hsdio.ExportSignal(niHSDIOConstants.SampleClock, "", niHSDIOConstants.ClkOutStr);
                hsdio.ExportSignal(niHSDIOConstants.SampleClock, "", niHSDIOConstants.DdcClkOutStr);
                hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.PxiTrig0Str);

                Dictionary<int, string> hsChannels = countHSChannels(usedDigitalChannels);
                if (hsChannels.Count != 0)
                {
                    //HSDIO cards typically have 32 channels, but we may want to reserve some for defining clock signals (in which case it may be slightly trickier to generate a digital sequence)
                    //This code goes through every enabled channel and parses the sequence data into a list of booleans for each value
                    if (deviceSettings.DigitalHardwareStructure[0] == 32)
                    {
                        bool[] singleChannelBuffer;
                        uint[] hsdioBuffer;
                        List<byte> hsdioChannelData = new List<byte>();
                        List<string> channelNames = new List<string>();
                        foreach (string hc in hsChannels.Values)
                        {
                            channelNames.Add(hc);
                        }
                        try
                        {
                            singleChannelBuffer = new bool[nSamples];
                            hsdioBuffer = new uint[nSamples];
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Unable to allocate digital buffer for HSDIO device " + deviceName + ". Reason: " + e.Message + "\n" + e.StackTrace);
                        }
                        for (int channel = 0; channel < deviceSettings.DigitalHardwareStructure[0]; channel++)
                        {
                            string hsChannel = "hs" + channel.ToString();
                            if (channelNames.Contains(hsChannel))
                            {
                                int digitalID = hsChannels.FirstOrDefault(x => x.Value == hsChannel).Key;
                                sequence.computeDigitalBuffer(digitalID, timeStepSize, singleChannelBuffer);
                               hsdioBuffer = addBooltoUint(hsdioBuffer,singleChannelBuffer,channel);
                            }
                        }
                        try
                        {
                            hsdio.AllocateNamedWaveform("waveform", nSamples);
                            hsdio.WriteNamedWaveformU32("waveform", nSamples, hsdioBuffer);
                        }
                        catch (System.AccessViolationException e)
                        {
                            throw new Exception("Couldn't access memory on HSDIO card.");
                        }
                        expectedSamplesGenerated = nSamples;
                        TotalSamplesGeneratedPerChannel = expectedSamplesGenerated;

                    }
                }
            }
            if (deviceSettings.StartTriggerType == DeviceSettings.TriggerType.TriggerIn)
            {
                hsdio.ConfigureDigitalEdgeStartTrigger(deviceSettings.TriggerInPort, niHSDIOConstants.RisingEdge);
            }
            #endregion
        }

        private static Dictionary<int, string> countHSChannels(Dictionary<int, HardwareChannel> digitalChannels)
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
        private static uint[] addBooltoUint(uint[]uintList,bool[] boolList,int Channel)
        {
            //Adds each element of a bool list to the corresponding element in a 32-bit integer list
            int boolLength = boolList.Length;
            int uintLength = uintList.Length;
            if (boolLength != uintLength)
                throw new Exception("Number of samples across channels is not equal to the number of samples on hs" + Channel.ToString());
            int uintIndex = 0;
            for (int i = 0; i < uintLength; i++)
            {
                if (boolList[i])
                   uintList[i] |= (uint)(((uint)1) << Channel);
            }
            uintIndex++;
            return uintList;
        }
        public void Dispose()
        {
            hsdio.Dispose();
        }
        public void Abort()
        {
            hsdio.Abort();
        }

        public int Initiate()
        {
            int initiate = hsdio.Initiate();
            return initiate;
        }

        public void ExportSignal(int signal, string signal_identifier,string output_terminal)
        {
            hsdio.ExportSignal(signal, signal_identifier, output_terminal);
        }
    
    }
}
