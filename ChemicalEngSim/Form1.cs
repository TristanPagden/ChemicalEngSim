namespace ChemicalEngSim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            double ProcessTime = 150;

            FinalTank ProductTank = new(50000);
            Reactor Reactor1 = new(20000, 0.5);
            Pipe HClPipe = new(0.5, 0, 0, 5);
            Pipe NaOHPipe = new(0.5, 0, 0, 5);
            Tank HClTank = new(100, 100);
            Tank NaOHTank = new(100, 100);
            var ProductTankRef = new RefValue<FinalTank>(ProductTank);
            var Reactor1Ref = new RefValue<Reactor>(Reactor1);
            var HClPipeRef = new RefValue<Pipe>(HClPipe);
            var NaOHPipeRef = new RefValue<Pipe>(NaOHPipe);
            var HClTankRef = new RefValue<Tank>(HClTank);
            var NaOHTankRef = new RefValue<Tank>(NaOHTank);



            Pump Pump1 = new(100);


            dynamic[] Flow1 = { HClTankRef, HClPipeRef, Reactor1Ref };
            dynamic[] Flow2 = { NaOHTankRef, NaOHPipeRef, Reactor1Ref };
            dynamic[] Flow3 = { Reactor1Ref, ProductTankRef };
            dynamic[] Objects = { HClTankRef, HClPipeRef, Reactor1Ref, NaOHTankRef, NaOHPipeRef, Reactor1Ref, ProductTankRef };
            var Flow1ListRef = new FlowListRef<dynamic>(Flow1);
            var Flow2ListRef = new FlowListRef<dynamic>(Flow2);
            var Flow3ListRef = new FlowListRef<dynamic>(Flow3);



            Pump1.CalculateMassFlowRate(Flow1);
            Pump1.CalculateMassFlowRate(Flow2);
            Pump1.CalculateMassFlowRate(Flow3);


            TimeStep TimeStep1 = new TimeStep();

            while (TimeStep1.t < ProcessTime)
            {
                TimeStep1.TimeSteps(Flow1, Flow3, true, false, Objects, Reactor1Ref);
                TimeStep1.TimeSteps(Flow2, Flow3, false, true,  Objects, Reactor1Ref);
                TimeStep1.TimeSteps(Flow3, Flow3, false, false, Objects, Reactor1Ref);

            }



            formsPlot1.Plot.Add.Scatter(TimeStep1.Time.ToArray(), Reactor1Ref.Value.HClConcentrationValues.ToArray());
            formsPlot1.Refresh();
            formsPlot2.Plot.Add.Scatter(TimeStep1.Time.ToArray(), Reactor1Ref.Value.NaClConcentrationValues.ToArray());
            formsPlot2.Refresh();

        }
    }
}
