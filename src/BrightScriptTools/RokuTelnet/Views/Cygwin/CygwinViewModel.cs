﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Prism.Commands;

namespace RokuTelnet.Views.Cygwin
{
    public class CygwinViewModel : Prism.Mvvm.BindableBase, ICygwinViewModel
    {
        private const int LENGHT = 10000;
        private const string FILE_NAME = "commands.json";
        private const string LAST_FOLDER_NAME = "lastFolder.json";

        private string _logs;

        private string _commands;
        private int _cmdIndex = 0;
        private bool _enable = true;

        private Process _process;

        public CygwinViewModel(ICygwinView view)
        {
            View = view;
            View.DataContext = this;

            LastCommands = new ObservableCollection<string>(LoadCommandList());
            Output = string.Empty;

            var psi = new ProcessStartInfo("Cygwin.bat");
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            _process = new Process();
            _process.StartInfo = psi;
            _process.OutputDataReceived += _processDataReceived;
            _process.ErrorDataReceived += _processDataReceived;
            _process.Start();

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            
            EnterCommand = new DelegateCommand(() =>
            {
                _process.StandardInput.WriteLine(Command);
                LastCommands.Add(Command);
                if (LastCommands.Count > 100)
                    LastCommands.RemoveAt(0);
                _cmdIndex = LastCommands.Count;
                Command = string.Empty;
                UpdateCommandList();
            });

            UpCommand = new DelegateCommand(() =>
            {
                if (_cmdIndex > 0)
                {
                    _cmdIndex--;
                    Command = LastCommands[_cmdIndex];
                }
            });

            DownCommand = new DelegateCommand(() =>
            {
                if (_cmdIndex < LastCommands.Count - 1)
                {
                    _cmdIndex++;
                    Command = LastCommands[_cmdIndex];
                }
                else
                {
                    _cmdIndex = LastCommands.Count;
                    Command = String.Empty;
                }
            });

            CoffeeCommand = new DelegateCommand(() =>
            {
                Command = "coffee build.coffee";
                EnterCommand.Execute().Wait();
            });

            ChangeDirectory();
        }

        private void ChangeDirectory()
        {
            var dir = GetLastFolder().Substring(2).Replace("\\", "/");
            _process.StandardInput.WriteLine("cd /cygdrive/c{0}", dir);
        }

        private void _processDataReceived(object sender, DataReceivedEventArgs e)
        {
            Output += e.Data + Environment.NewLine;

            if (Output.Length > LENGHT)
                Output = Output.Substring(Output.Length - LENGHT);
        }

        public ICygwinView View { get; set; }

        public string Output
        {
            get { return _logs; }
            set { _logs = value; OnPropertyChanged(() => Output); }
        }

        public ObservableCollection<string> LastCommands { get; set; }

        public string Command
        {
            get { return _commands; }
            set { _commands = value; OnPropertyChanged(() => Command); }
        }

        public DelegateCommand EnterCommand { get; set; }
        public DelegateCommand UpCommand { get; set; }
        public DelegateCommand DownCommand { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                OnPropertyChanged(() => Enable);
            }
        }

        public DelegateCommand CoffeeCommand { get; set; }

        private string GetLastFolder()
        {
            if (File.Exists(LAST_FOLDER_NAME))
            {
                using (var sr = new StreamReader(LAST_FOLDER_NAME))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<string>(content);
                }
            }

            return null;
        }

        private void UpdateCommandList()
        {
            var content = JsonConvert.SerializeObject(LastCommands.ToList());

            if (File.Exists(FILE_NAME))
                File.Delete(FILE_NAME);

            using (var sw = new StreamWriter(FILE_NAME))
            {
                sw.Write(content);
            }
        }

        private List<string> LoadCommandList()
        {
            if (File.Exists(FILE_NAME))
            {
                using (var sr = new StreamReader(FILE_NAME))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<string>>(content);
                }
            }

            return new List<string>();
        }
    }
}