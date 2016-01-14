﻿using System;
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


        public HSDIOTask (string deviceName, string channelList, DeviceSettings deviceSettings)
        {
            //initialises an instance of HSDIOTask
            hsdio = niHSDIO.InitGenerationSession(deviceName, true, false, "");
            this.deviceName = deviceName;
            this.channelList = channelList;
            this.TotalSamplesGeneratedPerChannel = 0;
            ConfigureVoltageLevels(deviceSettings);
            hsdio.AssignDynamicChannels(channelList);
        }
        /// <summary>
        /// This class mimicks some of the functionality of DAQmx for NI-HSDIO cards. The buffer is generated as a digital waveform in a similar way to DAQmx digital tasks.
        /// </summary>
        /// 
        public void createHSDIOWaveForm(AtticusServerCommunicator sender, string deviceName, DeviceSettings deviceSettings, SequenceData sequence, SettingsData settings, Dictionary<int, HardwareChannel> usedDigitalChannels, ServerSettings serverSettings, out long expectedSamplesGenerated)
        {

            bool softTrigger = false;
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
                    hsdio.ConfigureRefClock(niHSDIOConstants.PxiClk10Str, 10000000);
                    hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.PxiTrig1Str);
                    //hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.PxiTrig1Str);
                    
                }
                else if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.External)
                {
                    //If set to ClkIn, the clock is configured to use the ClkIn input on the HSDIO card
                    hsdio.ConfigureSampleClock(deviceSettings.SampleClockExternalSource, deviceSettings.SampleClockRate);
                }
                if (deviceSettings.StartTriggerType == DeviceSettings.TriggerType.SoftwareTrigger)
                {
                    softTrigger = true;
                    hsdio.ConfigureSoftwareStartTrigger();
                }
                else if(deviceSettings.StartTriggerType == DeviceSettings.TriggerType.TriggerIn)
                {
                    softTrigger = false;
                    string trigger = deviceSettings.TriggerInPort;
                    hsdio.ConfigureDigitalEdgeStartTrigger(niHSDIOConstants.PxiTrig0Str, niHSDIOConstants.RisingEdge);
                }
                hsdio.ExportSignal(niHSDIOConstants.SampleClock, "", niHSDIOConstants.ClkOutStr);
                hsdio.ExportSignal(niHSDIOConstants.SampleClock, "", niHSDIOConstants.DdcClkOutStr);
                //hsdio.ExportSignal(niHSDIOConstants.StartTrigger, "", niHSDIOConstants.Pfi1Str);


                Dictionary<int, string> hsChannels = countHSChannels(usedDigitalChannels);
                if (hsChannels.Count != 0)
                {
                    //HSDIO cards typically have 32 channels, but we may want to reserve some for defining clock signals (in which case it may be slightly trickier to generate a digital sequence)
                    ////This code goes through every enabled channel and parses the sequence data into a list of booleans for each value
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
                                hsdioBuffer = addBooltoUint(hsdioBuffer,singleChannelBuffer,channel,softTrigger);
                            }
                        }
                        try
                        {
                            hsdio.AllocateNamedWaveform("waveform", nSamples);
                            hsdio.WriteNamedWaveformU32("waveform", nSamples, hsdioBuffer);
                       
                            hsdio.CommitDynamic();
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
            //if (deviceSettings.StartTriggerType == DeviceSettings.TriggerType.TriggerIn)
            //{
            //    hsdio.ConfigureDigitalEdgeStartTrigger(deviceSettings.TriggerInPort, niHSDIOConstants.RisingEdge);
            //}
            #endregion
            
            else //cariable timebase buffer creation
            {
                double timeStepSize = Common.getPeriodFromFrequency(deviceSettings.SampleClockRate);
                TimestepTimebaseSegmentCollection timebaseSegments = sequence.generateVariableTimebaseSegments(serverSettings.VariableTimebaseType, timeStepSize);

                int nBaseSamples = timebaseSegments.nSegmentSamples();
                //The HSDIO cards require samples to be written in bytes
                int nFillerSamples = 8 - nBaseSamples % 4;
                if (nFillerSamples == 8)
                {
                    nFillerSamples = 0;
                }
                int nSamples = nBaseSamples + nFillerSamples;

                if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.DerivedFromMaster)
                {
                    throw new Exception("Attempt to use a uniform sample clock with a variable timebase enabled device. This will not work. To use a variable timebase for this device, you must specify an external sample clock source.");
                }
                else
                {
                    hsdio.ConfigureSampleClock(deviceSettings.SampleClockExternalSource, deviceSettings.SampleClockRate);
                }
            }
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
        private static uint[] addBooltoUint(uint[]uintList,bool[] boolList,int Channel, bool softTrigger)
        {
            //Adds each element of a bool list to the corresponding element in a 32-bit integer list
            //The HSDIO card starts outputting it's sequence 32 samples after the start trigger is sent. The simplest work-around is to remove the first 32 samples if Atticus is configured to trigger using the HSDIO card.
            int boolLength = boolList.Length;
            int uintLength = uintList.Length;
            int sampleShift = 0;
            if (boolLength != uintLength)
                throw new Exception("Number of samples across channels is not equal to the number of samples on hs" + Channel.ToString());
            if (softTrigger)
            {
                sampleShift = 29;
            }
            for (int i = 0; i < uintLength-sampleShift; i++)
            {
                if (boolList[i+sampleShift])
                   uintList[i] |= (uint)(((uint)1) << Channel);
            }

            return uintList;
        }
        private void ConfigureVoltageLevels(DeviceSettings deviceSettings)
        {
            int voltageLevel = new int();
            //A bit messy, and would probably be better with a dictionary, but this does the job
           DeviceSettings.VoltageLevel voltageFamily = deviceSettings.DigitalVoltage;
            if (voltageFamily == DeviceSettings.VoltageLevel._5V)
            {
                voltageLevel = niHSDIOConstants._50vLogic;
            }
            else if (voltageFamily == DeviceSettings.VoltageLevel._33V)
            {
                voltageLevel = niHSDIOConstants._33vLogic;

            }
            else if (voltageFamily == DeviceSettings.VoltageLevel._18V)
            {
                voltageLevel = niHSDIOConstants._18vLogic;
            }
            hsdio.ConfigureEventVoltageLogicFamily(voltageLevel);
            hsdio.ConfigureDataVoltageLogicFamily("0-31",voltageLevel);
            hsdio.ConfigureTriggerVoltageLogicFamily(voltageLevel);
        }
        public void Dispose()
        {
            hsdio.Dispose();
        }
        public void Abort()
        {
            hsdio.Abort();
            hsdio.reset();
        }

        public int Initiate()
        {
            int initiate = hsdio.Initiate();
            return initiate;
        }
        public void SendStartTrigger()
        {
            hsdio.SendSoftwareEdgeTrigger(niHSDIOConstants.StartTrigger, "");
        }
        public void ExportSignal(int signal, string signal_identifier,string output_terminal)
        {
            hsdio.ExportSignal(signal, signal_identifier, output_terminal);
        }

    }
}
