namespace HD5toPcapConverter.FileReaders
{
    partial class Hdf5FileReader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            reader = new System.ComponentModel.BackgroundWorker();
            reader.WorkerSupportsCancellation = true;

            reader.DoWork += new System.ComponentModel.DoWorkEventHandler(reader_DoWork);
            reader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(reader_RunWorkerCompleted);
        }

        private System.ComponentModel.BackgroundWorker reader;

        #endregion
    }
}
