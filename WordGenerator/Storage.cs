using System;
using DataStructures;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace WordGenerator
{
    /// <summary>
    /// The Storage class is a static container for application-related data. Its main components (henceforth referred to as
    /// the "Storage subclasses") are:
    ///     1) clientStartupSettings;
    ///     2) settingsData;
    ///     3) sequenceData;
    /// For further details on each subclass, please see the associated source file.
    /// </summary>
    public class Storage
    {
        private static string clientStartupSettingsFileName;

        public static ClientStartupSettings clientStartupSettings;
        public static SettingsData settingsData;

        public static SequenceData sequenceData;

        public static Waveform clipboardWaveform;

        private static Storage dummy = new Storage();

        private static event EventHandler<MessageEvent> messageLog;

        public static void registerMessageLogHandler(EventHandler<MessageEvent> handler) {
            messageLog+=handler;
        }

        private static void safeMessageLog(MessageEvent message) {
            if (messageLog != null)
                messageLog(dummy, message);
        }




        /// <summary>
        /// The SaveAndLoad class is a collection of methods that implement (as the title suggests!) saving and loading of
        /// the Storage subclasses. The private Load(...), Save(...), PromptOpenFileDialog(...) are basic methods that implement the
        /// .NET mechanism for serialization. From the application, we indicate that we want to save or load by invoking the
        /// appropriate public intermediary method; for example, LoadSettingsData(string path) to load the SettingsData file.
        /// </summary>
        public static class SaveAndLoad
        {
            /// <summary>
            /// Loads an object from serialized format using .NET's BinaryFormatter. The method internally catches 
            /// (and quiets) FileNotFoundException, SerializationException and ArgumentNullException. In such cases, 
            /// a null object is returned as an error flag.
            /// </summary>
            /// <returns>Deserialized object or null</returns>
            private static object Load(string path)
            {

                if (path == null)
                    return null;

                try
                {
                  
                    return Common.loadBinaryObjectFromFile(path);
                    
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), FileNotFoundException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), FileNotFoundException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), SerializationException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), SerializationException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), IOException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (IOException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), ArgumentNullException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (JsonException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), JsonException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), JsonException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                return null;
            }
            #region Json deserialize methods
            private static SettingsData LoadSettingsJson(string path)
            {
                try
                {
                    return JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText(path), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, ReferenceLoopHandling = ReferenceLoopHandling.Error});
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), FileNotFoundException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), FileNotFoundException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), SerializationException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), SerializationException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), IOException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (IOException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), ArgumentNullException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (JsonException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), JsonException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), JsonException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                return null;
            }

            private static SequenceData LoadSequenceJson(string path)
            {
                try
                {
                    return JsonConvert.DeserializeObject<SequenceData>(File.ReadAllText(path), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, ReferenceLoopHandling = ReferenceLoopHandling.Error});
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), FileNotFoundException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), FileNotFoundException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), SerializationException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), SerializationException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), IOException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (IOException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), ArgumentNullException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (JsonException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), JsonException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), JsonException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                return null;
            }

            private static ClientStartupSettings LoadClientJson(string path)
            {
                try
                {
                    return JsonConvert.DeserializeObject<ClientStartupSettings>(File.ReadAllText(path), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, ReferenceLoopHandling = ReferenceLoopHandling.Error});
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), FileNotFoundException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), FileNotFoundException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), SerializationException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), SerializationException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), IOException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (IOException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), IOException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), ArgumentNullException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                catch (JsonException e)
                {
                    Console.WriteLine("SaveAndLoad.Load(), JsonException: " + e.Message);
                    safeMessageLog(new MessageEvent("SaveAndLoad.Load(), JsonException: " + e.Message, 0, MessageEvent.MessageTypes.Error));
                }
                return null;
            }
            #endregion
            /// <summary>
            /// Saves an object to the specified path using the .NET BinaryFormatter. In contrast to the Load(string path)
            /// method, Save(...) does NOT internally catch any exceptions. Furthermore, If a file already exists at the target 
            /// path, the method will create a backup.
            /// </summary>
            public static void Save(string path, object obj)
            {
                Save(path, obj, true);
            }

            /// <summary>
            /// Saves an object to the specified path using the .NET BinaryFormatter. In contrast to the Load(string path)
            /// method, Save(...) does NOT internally catch any exceptions. If a file already exists at the target 
            /// path, the method will create a backup only if saveOld File is set to true
            /// </summary>
            /// <param name="path"></param>
            /// <param name="obj"></param>
            /// <param name="saveOldFile"></param>
            public static void Save(string path, object obj, bool saveOldFile)
            {
               
                
                

                if( saveOldFile )
                    #region Backup old file
                    if (File.Exists(path)) // Does our target already exist?
                    {
                        // Make sure that the backup folder exists
                        if (!Directory.Exists(FileNameStrings.BackupFolder))
                            Directory.CreateDirectory(FileNameStrings.BackupFolder);

                        // We'll use the current time to generate a unique file name. Unfortunately, the standard
                        // ToString methods associated with the DateTime class generate strings incompatible for filenames.
                        // So we have our own little scheme, below:
                        DateTime dt = DateTime.Now;
                        string timestamp = dt.Year.ToString()   + "-" +
                                           dt.Month.ToString()  + "-" +
                                           dt.Day.ToString()    + "-" +
                                           dt.Hour.ToString()   + "-" +
                                           dt.Minute.ToString() + "-" +
                                           dt.Second.ToString();

                        // It is possible that the given path is absolute, in which case our naming scheme below will not work. So, 
                        // with the following code, we fetch only the name of the file.
                        string fileNameOnly;
                        #region Get the file name from (possibly) the absolute path
                    char pathDelimiter = '\\';
                    string[] split = null;

                    split = path.Split(pathDelimiter);
                    fileNameOnly = split[split.Length - 1];
                    #endregion

                        string backupFileName = FileNameStrings.BackupFolder + fileNameOnly + "." + timestamp + "." + "backup";

                        File.Copy(path, backupFileName, true); // We allow overwrite, in the off chance that the user requests
                                                               // two consecutive saves within the same second, and generates a 
                                                               // duplicate filename!
                    
                        // Acknowledge
                        Console.WriteLine("SaveAndLoad.Save(): Created backup file at " + backupFileName);
                    }
                    #endregion

                // Since we have created a backup, it ought to be okay to clobber the old!
                if (path.EndsWith(".json"))
                {
                    JsonSerializer json = new JsonSerializer();
                        string json_obj = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, ReferenceLoopHandling = ReferenceLoopHandling.Error, NullValueHandling = NullValueHandling.Ignore});
                    File.WriteAllText(path, json_obj);
                    
                }
                else if (path != null)
                {
                    //otherwise, it saves as a binary
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, obj);
                    }
                }
                
            }


            
            /// <summary>
            /// The LoadAllSubclasses(string clientStartupSettingsFileNameParam) method will set ALL subclasses of 
            /// the Storage container based on the information given in the clientStartupSettings file.
            /// </summary>
            /// <param name="clientStartupSettingsFileNameParam">Target ClientStartupSettings filename</param>
            public static void LoadAllSubclasses(string clientStartupSettingsFileNameParam)
            {
                clientStartupSettingsFileName = clientStartupSettingsFileNameParam;

                LoadClientStartupSettings(clientStartupSettingsFileName);
                LoadSettingsData(clientStartupSettings.settingsDataFileName);                
                LoadSequenceDataToStorage(clientStartupSettings.sequenceDataFileName);

                if (Storage.settingsData == null)
                    Storage.settingsData = new SettingsData();

                if (Storage.sequenceData == null)
                    Storage.sequenceData = new SequenceData();
            }

            public static void LoadClientStartupSettings(string path)
            {
                //string targetFile;
                if (path.EndsWith(".json"))
                clientStartupSettings = LoadClientJson(path);
                else if (path != null)
                clientStartupSettings = Load(path) as ClientStartupSettings;
                
                if (clientStartupSettings == null) // Failed to load ClientStartupSettings in the first pass. Probably file doesn't exist.
                {
                   /* targetFile =
                        PromptOpenFileDialog("ClientStartupSettings", DefaultNames.Extensions.ClientStartupSettings);

                    //if (targetFile == null) // Null return indicates that user has opted for 'Cancel' i.e. default state
                    //{
                      MessageBox.Show("Proceeding with default ClientStartupSettings!"); */
                        clientStartupSettings = new ClientStartupSettings();

                    // Also, give it the default name
                    clientStartupSettingsFileName = FileNameStrings.DefaultClientStartupSettingsFile;
                    /*}
                    else
                    {
                        clientStartupSettings = Load(targetFile) as ClientStartupSettings;
                        clientStartupSettingsFileName = targetFile;
                    }*/
                }
            }



            public static bool LoadSettingsData(string path)
            {
                SettingsData loadedSettings = null;

                if (path != null)
                {
                    if (path.EndsWith(".json"))
                        loadedSettings = LoadSettingsJson(path);
                    else
                        loadedSettings = Load(path) as SettingsData;                    
                }
                else
                {
                    string ext;
                    if (settingsData.SaveDataAsJson)
                        ext = FileNameStrings.Extensions.JsonData;
                    else
                        ext = FileNameStrings.Extensions.ClientSettingsData;
                    path =
                        SharedForms.PromptOpenFileDialog(FileNameStrings.FriendlyNames.ClientSettingsData,ext);

                    SettingsData loadedObject;
                    if (path.EndsWith(".json") && path != null)
                        loadedObject = LoadSettingsJson(path);
                    else
                        loadedObject = Load(path) as SettingsData;

#if DEBUG
                    if (!(loadedSettings is SettingsData))
                    {
                        WordGenerator.MainClientForm.instance.handleMessageEvent(
                            WordGenerator.MainClientForm.instance, new MessageEvent("Loaded settings object is non settings data object.", 0, MessageEvent.MessageTypes.Debug,
                                 MessageEvent.MessageCategories.Unspecified)
                                 );
                    }
#endif
                    if (path.EndsWith(".json"))
                        loadedSettings = LoadSettingsJson(path);
                    else
                        loadedSettings = Load(path) as SettingsData;
                }

                if (loadedSettings != null)
                {
                    Storage.settingsData = loadedSettings;
                    WordGenerator.MainClientForm.instance.OpenSettingsFileName = path;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static SequenceData LoadSequenceWithFileDialog()
            {
                string ext;
                if (settingsData.SaveDataAsJson)
                    ext = FileNameStrings.Extensions.JsonData;
                else
                    ext = FileNameStrings.Extensions.SequenceData;
                string path =
                        SharedForms.PromptOpenFileDialog(FileNameStrings.FriendlyNames.SequenceData, ext);
                    if (path.EndsWith(".json") && path != null)
                    return LoadSequenceJson(path);
                    else
                    return  Load(path) as SequenceData;
            }

            public static bool LoadSequenceDataToStorage(string path)
            {

                SequenceData loadMe;

                if (path != null)
                {
                    if (path.EndsWith(".json"))
                        loadMe = LoadSequenceJson(path);
                    else
                    loadMe = Load(path) as SequenceData;
                }
                else
                {
                    string ext;
                    if (settingsData.SaveDataAsJson)
                        ext = FileNameStrings.Extensions.JsonData;
                    else
                        ext = FileNameStrings.Extensions.SequenceData;
                    loadMe = new SequenceData();
                    path =
                        SharedForms.PromptOpenFileDialog(FileNameStrings.FriendlyNames.SequenceData, ext);
                    if (path != null && path.EndsWith(".json"))
                        loadMe = LoadSequenceJson(path);
                    else
                        loadMe = Load(path) as SequenceData;
                }

                if (loadMe != null)
                {
                    Storage.sequenceData = loadMe;
                    clientStartupSettings.AddNewFile(path);
                    WordGenerator.MainClientForm.instance.OpenSequenceFileName = path;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            public static void SaveAllSubclasses()
            {
                SaveClientStartupSettings(clientStartupSettingsFileName);
                SaveSettingsData(clientStartupSettings.settingsDataFileName);
                SaveSequenceData(clientStartupSettings.sequenceDataFileName);
            }

            public static void SaveClientStartupSettings()
            {
                Save(AppDomain.CurrentDomain.BaseDirectory + clientStartupSettingsFileName, clientStartupSettings);
            }
            public static void SaveClientStartupSettings(string path)
            {      
                Save(path, clientStartupSettings);
            }


            public static void SaveSettingsData(string path)
            {
                string ext;
                if (settingsData.SaveDataAsJson)
                    ext = FileNameStrings.Extensions.JsonData;
                else
                    ext = FileNameStrings.Extensions.ClientSettingsData;
                if (path == null)
                {
                    path = SharedForms.PromptSaveFile(FileNameStrings.FriendlyNames.ClientSettingsData, ext);
                    if (path != null)
                    {
                        Save(path, settingsData);
                        WordGenerator.MainClientForm.instance.OpenSettingsFileName = path;
                    }
                }
                if (path != null)
                {
                    Save(path, settingsData);
                    WordGenerator.MainClientForm.instance.OpenSettingsFileName = path;
                }
            }



            public static void SaveSequenceData(string path) {
                SaveSequenceData(path, Storage.sequenceData);
            }

            public static void SaveSequenceData(string path, SequenceData sequence)
            {
                string ext;
                if (settingsData.SaveDataAsJson)
                    ext = FileNameStrings.Extensions.JsonData;
                else
                    ext = FileNameStrings.Extensions.SequenceData;
                if (path == null)
                {
                    path = SharedForms.PromptSaveFile(FileNameStrings.FriendlyNames.SequenceData, ext);
                }

                if (path != null)
                {
                    Save(path, sequence);
                    if (sequence == sequenceData)
                        WordGenerator.MainClientForm.instance.OpenSequenceFileName = path;
                    clientStartupSettings.AddNewFile(path);
                }
            }
        }
    }
}
