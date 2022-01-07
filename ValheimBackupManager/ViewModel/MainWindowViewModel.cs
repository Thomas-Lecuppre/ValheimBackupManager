using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimBackupManager.Model;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace ValheimBackupManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        MainWindowModel _model;

        public MainWindowViewModel()
        {
            Log = "";

            _model = new MainWindowModel(this);

            //Initialising commands
            BrowseGameFolder = new DelegateCommand(E_BrowseGameFolder, CanE_BrowseGameFolder);
            BrowseUserFolder = new DelegateCommand(E_BrowseUserFolder, CanE_BrowseUserFolder);
            DecreaseTime = new DelegateCommand(E_DecreaseTime, CanE_DecreaseTime);
            Decrease10Time = new DelegateCommand(E_Decrease10Time, CanE_Decrease10Time);
            Decrease60Time = new DelegateCommand(E_Decrease60Time, CanE_Decrease60Time);
            IncreaseTime = new DelegateCommand(E_IncreaseTime, CanE_IncreaseTime);
            Increase10Time = new DelegateCommand(E_Increase10Time, CanE_Increase10Time);
            Increase60Time = new DelegateCommand(E_Increase60Time, CanE_Increase60Time);
            SwitchWorkingMode = new DelegateCommand(E_SwitchWorkingMode, CanE_SwitchWorkingMode);

            _model.GetUserConf();

            WindowTitle = "Valheim Backup Manager v1.0.0.0";

            Working = false;
            WorkingTag = "";
        }

        // Window name
        private string windowTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            set 
            { 
                windowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }


        //Path of game save.
        private string gameFolder;
        public string GameFolder
        {
            get { return gameFolder; }
            set 
            { 
                gameFolder = value;
                OnPropertyChanged("GameFolder");
                _model.SetUserConf();
            }
        }

        #region Browse Game Folder

        public DelegateCommand BrowseGameFolder { get; set; }

        void E_BrowseGameFolder(object parameter)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Chemin vers l'emplacement des sauvegardes de parties du jeu Valheim";
            fbd.ShowDialog();

            if (Directory.Exists(fbd.SelectedPath))
            {
                GameFolder = fbd.SelectedPath;
            }
        }

        bool CanE_BrowseGameFolder(object parameter)
        {
            return true;
        }

        #endregion

        //Path where user want to make the save of the save (saveception)
        private string userFolder;
        public string UserFolder
        {
            get { return userFolder; }
            set 
            { 
                userFolder = value;
                OnPropertyChanged("UserFolder");
                _model.SetUserConf();
            }
        }

        #region Browse User Folder

        public DelegateCommand BrowseUserFolder { get; set; }

        void E_BrowseUserFolder(object parameter)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Chemin ou vous souhaitez enregistrer les sauvegarde de Valheim";
            fbd.ShowDialog();

            if (Directory.Exists(fbd.SelectedPath))
            {
                UserFolder = fbd.SelectedPath;
            }
        }

        bool CanE_BrowseUserFolder(object parameter)
        {
            return true;
        }

        #endregion

        //Time between save of save (saveception again)
        private int number;
        public int Number
        {
            get { return number; }
            set 
            { 
                number = value; 
                OnPropertyChanged("Number");
                _model.SetUserConf();
            }
        }

        //Time unit between save of save (realy ? saveception, again ?)
        private string timeUnit;
        public string TimeUnit
        {
            get { return timeUnit; }
            set 
            { 
                timeUnit = value;
                OnPropertyChanged("TimeUnit");
                _model.SetUserConf();
            }
        }

        #region Time Changer

        #region Decrease Time

        public DelegateCommand DecreaseTime { get; set; }

        void E_DecreaseTime(object parameter)
        {
            Number--;
        }

        bool CanE_DecreaseTime(object parameter)
        {
            return Number - 1 > 0;
        }

        #endregion

        #region Decrease 10 Time

        public DelegateCommand Decrease10Time { get; set; }

        void E_Decrease10Time(object parameter)
        {
            Number = number -10;
        }

        bool CanE_Decrease10Time(object parameter)
        {
            return Number - 10 > 0;
        }

        #endregion

        #region Decrease 60 Time

        public DelegateCommand Decrease60Time { get; set; }

        void E_Decrease60Time(object parameter)
        {
            Number = number - 60;
        }

        bool CanE_Decrease60Time(object parameter)
        {
            return Number - 60 > 0;
        }

        #endregion

        #region Increase Time

        public DelegateCommand IncreaseTime { get; set; }

        void E_IncreaseTime(object parameter)
        {
            Number++;
        }

        bool CanE_IncreaseTime(object parameter)
        {
            return Number + 1 < 999;
        }

        #endregion

        #region Increase 10 Time

        public DelegateCommand Increase10Time { get; set; }

        void E_Increase10Time(object parameter)
        {
            Number = number + 10;
        }

        bool CanE_Increase10Time(object parameter)
        {
            return Number + 10 < 999;
        }

        #endregion

        #region Increase 60 Time

        public DelegateCommand Increase60Time { get; set; }

        void E_Increase60Time(object parameter)
        {
            Number = number + 60;
        }

        bool CanE_Increase60Time(object parameter)
        {
            return Number + 60 < 999;
        }

        #endregion

        #endregion

        //State work the application
        private bool working;
        public bool Working
        {
            get { return working; }
            set 
            { 
                working = value;
                OnPropertyChanged("Working");
            }
        }

        //Update UI without blocking button
        private string workingTag;
        public string WorkingTag
        {
            get { return workingTag; }
            set 
            { 
                workingTag = value;
                OnPropertyChanged("WorkingTag");
            }
        }

        #region Switch Working Mode

        public DelegateCommand SwitchWorkingMode { get; set; }

        void E_SwitchWorkingMode(object parameter)
        {
            if(Working)
            {
                Working = false;
                WorkingTag = "";
            }
            else
            {
                Working = true;
                WorkingTag = "Working";
                _model.StartTheJob(Number);
            }
        }

        bool CanE_SwitchWorkingMode(object parameter)
        {
            return true;
        }

        #endregion

        //All the log of the current session.
        private string log;
        public string Log
        {
            get { return log; }
            set 
            { 
                log = value;
                OnPropertyChanged("Log");
            }
        }

    }
}
