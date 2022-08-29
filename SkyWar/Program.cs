namespace EleCho.ScratchGame
{
    internal static class Program
    {
        static MainForm? mainForm;
        public static string Title { get => mainForm!.Text; set => mainForm!.Text = value; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}