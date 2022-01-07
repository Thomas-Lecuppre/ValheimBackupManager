using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ValheimBackupManager.ViewModel;

namespace ValheimBackupManager.Model
{
    public class MainWindowModel
    {
        MainWindowViewModel _viewModel;

        private string localConfFolder;
        private string localConfFile;
        private List<string> logEvents;

        public MainWindowModel(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;

            logEvents = new List<string>();

            localConfFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ValheimBackupManager";
            localConfFile = $"{localConfFolder}\\{Environment.UserName}.conf";

            //Verifying local information.
            if (!Directory.Exists(localConfFolder))
            {
                Directory.CreateDirectory(localConfFolder);
                Logger("Création du dossier de configuration local réussi.");
                Logger(localConfFolder);
            }

            if (!File.Exists(localConfFile))
            {
                File.WriteAllText(localConfFile, $"C:\\Users\\{Environment.UserName}\\AppData\\LocalLow\\IronGate\\Valheim\\worlds\n" +
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ValheimBackupManager\n" +
                    $"5\n" +
                    $"Minutes");
                Logger("Création du fichier de configuration local réussi.");
                Logger(localConfFolder);
            }

            //GetUserConf is called after end of initialisation in viewmodel.
        }

        public void GetUserConf()
        {
            List<string> content = File.ReadAllLines(localConfFile).ToList();

            if(content.Count >= 3)
            {
                _viewModel.GameFolder = content[0];
                _viewModel.UserFolder = content[1];
                _viewModel.Number = int.Parse(content[2]);
                _viewModel.TimeUnit = content[3];
            }
        }

        public void SetUserConf()
        {
            List<string> content = new List<string>()
            {
                _viewModel.GameFolder,
                _viewModel.UserFolder,
                _viewModel.Number.ToString(),
                _viewModel.TimeUnit
            };

            File.WriteAllLines(localConfFile, content);
        }

        public void StartTheJob(int timeToWait)
        {
            Task.Run(() =>
            {
                while (_viewModel.Working)
                {
                    string saveFolderPath = _viewModel.UserFolder + $"\\{DateTime.Now.ToString("yyyyMMdd_HH-mm-ss")}_ValheimSaveBackup";
                    if (!Directory.Exists(saveFolderPath))
                    {
                        Directory.CreateDirectory(saveFolderPath);
                        Logger($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Création d'un dossier de sauvegarde.");
                    }

                    try
                    {
                        FileInfo[] files = new DirectoryInfo(_viewModel.GameFolder).GetFiles();

                        foreach (FileInfo file in files)
                        {
                            file.CopyTo($"{saveFolderPath}\\{file.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger(ex.Message);
                    }

                    Logger($"Prochaine sauvegarde dans {_viewModel.Number} {_viewModel.TimeUnit.ToLower()}");

                    switch (_viewModel.TimeUnit)
                    {
                        case "Secondes":
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(timeToWait));
                                break;
                            }
                        case "Minutes":
                            {
                                Thread.Sleep(TimeSpan.FromMinutes(timeToWait));
                                break;
                            }
                        case "Heures":
                            {
                                Thread.Sleep(TimeSpan.FromHours(timeToWait));
                                break;
                            }
                        default:
                            {
                                Thread.Sleep(TimeSpan.FromMinutes(5));
                                break;
                            }
                    }
                }
            });
        }

        public void Logger(string text)
        {
            logEvents.Insert(0, text);

            List<string> temp = logEvents.Take(30).ToList();
            temp.Reverse();

            string finalLog = "";

            foreach(string item in temp)
            {
                finalLog += $"{item}\n";
            }

            _viewModel.Log = finalLog;

            logEvents = logEvents.Take(30).ToList();
        }
    }
}
