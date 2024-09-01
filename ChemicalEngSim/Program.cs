using System.Numerics;
using System.Threading.Tasks;

namespace ChemicalEngSim
{
    internal static class Program
    {
        

        [STAThread]
        static void Main()
        {
            


            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}