﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransmissionSimulation.Components;
using TransmissionSimulation.Models;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation
{
    public partial class MainForm : Form
    {
        private readonly ProgramOption progOption;
        private Thread sendThread;
        private Thread receiveThread;
        private Transmitter cable;
        private NumericUpDown[] errorsPositions;
        delegate void UpdateDataDelegate(Frame frameToShow, bool isSent);

        public MainForm(ProgramOption progOption)
        {
            this.progOption = progOption;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            errorsPositions = new[] {
                posError1,
                posError2,
                posError3,
                posError4
            };
            foreach (NumericUpDown errorPosition in errorsPositions)
            {
                errorPosition.Maximum = Constants.FrameSize * 8;
                errorPosition.Minimum = -1;
                errorPosition.Value = -1;
            }

            FileStream fileToCopie = File.Open(progOption.FileToCopie, FileMode.Open, FileAccess.Read);
            FileStream destinationFile = File.Open(progOption.DestinationFile, FileMode.OpenOrCreate, FileAccess.Write);
            //Start the threads
            cable = new Transmitter();
            Station sendStation = new Station(Constants.Station.Source, cable, progOption.BufferSize, progOption.Timeout * 1000, fileToCopie, ShowFrame);
            Station receiveStation = new Station(Constants.Station.Dest, cable, progOption.BufferSize, progOption.Timeout * 1000, destinationFile, ShowFrame);
            sendThread = new Thread(sendStation.Start);
            receiveThread = new Thread(receiveStation.Start);

            sendThread.Start();
            //receiveThread.Start();
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            foreach (NumericUpDown errorPosition in errorsPositions)
            {
                if (errorPosition.Validate() && errorPosition.Value != -1)
                {
                    cable.IndicesInversions.Add(errorPosition.Value);
                }
            }
        }

        private void ShowFrame(Frame frameToShow, bool isSent)
        {
            //based on msdn doc: https://msdn.microsoft.com/en-us/library/ms171728(v=vs.110).aspx
            RichTextBox textBox = isSent ? txtDataSend : txtReception;
            if (textBox.InvokeRequired)
            {
                UpdateDataDelegate d = ShowFrame;
                Invoke(d, frameToShow, isSent);
            }
            else
            {
                textBox.AppendText(frameToShow.ToString());
            }
        }
    }
}
