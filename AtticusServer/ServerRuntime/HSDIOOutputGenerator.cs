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
                int nBaseSamples = sequence.nSamples(timeStepSize);

                if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.DerivedFromMaster)
                {
                    //Uses the onboard clock
                    hsdio.ConfigureSampleClock("NIHSDIO_VAL_ON_BOARD_CLOCK_STR", deviceSettings.SampleClockRate);
                }
                else if (deviceSettings.MySampleClockSource == DeviceSettings.SampleClockSource.External && deviceSettings.
                    == "ClkIn")
                {
                    //If set to ClkIn, the clock is configured to use the ClkIn input on the HSDIO card
                    hsdio.ConfigureSampleClock("NIHSDIO_VAL_CLK_IN_STR", deviceSettings.SampleClockRate);
                }
               

            }
            #endregion
        }
    }
}
