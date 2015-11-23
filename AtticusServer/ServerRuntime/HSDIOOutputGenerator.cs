using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.ModularInstruments.Interop;
using DataStructures;

namespace AtticusServer.ServerRuntime
{
    public class HSDIOOutputGenerator
    {
        /// <summary>
        ///This class is designed to output a waveform to an NI-HSDIO card based on the digital sequence written in Cicero. 
        /// At the moment, it only works with a single timebase and it will perhaps be a lot trickier to work with a variable timebase.
        /// With all 32 channels enabled, the HSDIO card outputs a waveform as a 32bit integer at each clock cycle, so the digital "Tasks" are created on each channel
        /// simultaenously. 
        /// The HSDIO wrapper has a method WriteNamedWaveformWDT which takes in waveform data grouped by channel or grouped by sample (i.e. all channels are defined for the first time step, then second and so on).
        /// The enabled channels have their corresponding digital sequences written in this form. Those not defined by the user have a blank array written to them.
        /// </summary>
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
                if (usedDigitalChannels.Count != 0)
                {
                    //HSDIO cards typically have 32 channels, but we may want to reserve some for defining clock signals (in which case it may be slightly trickier to generate a digital sequence)
                    if (deviceSettings.DigitalHardwareStructure[0] == 32)
                    {
                        UInt32[,] digitalBuffer;
                        bool[] singleChannelBuffer;
                        try
                        {
                            digitalBuffer = new UInt32[usedDigitalChannels.Count, nSamples];
                            singleChannelBuffer = new bool[nSamples];
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Unable to allocate digital buffer for HSDIO device " + deviceName + ". Reason: " + e.Message + "\n" + e.StackTrace);
                        }
                        for (int i = 0; i<usedDigitalChannels.Count;i++)
                        {
                            int digNum = usedDigitalChannels[i];
                            UInt32 digitalBitMask = 1;
                            for (int channel = 0; channel < deviceSettings.DigitalHardwareStructure[digNum];channel++)
                        }
                    }
                 }
               }
            #endregion
        }
    }
}
