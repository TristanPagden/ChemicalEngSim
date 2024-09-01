using System;
using System.Runtime.CompilerServices;

namespace ChemicalEngSim
{

    public class RefValue<T>
    {
        public T Value;
        public RefValue(T value)
        {
            Value = value;
        }
    }

    public class FlowListRef<T>
    {
        public List<T> list;
        public FlowListRef(T[] array)
        {
            list = new List<T>(array);
        }
    }

    public class Pump
    {
        public int Power;

        public Pump(int power)
        {

            this.Power = power;

        }

        //tex: Assumptions:

        //tex: Steady state system so $\dot{m}$ is constant

        //tex:$\rho$ is constant so $\dot{V}$ is constant

        //tex: Laminar flow so $f=0.02$

        //tex: $\alpha = 1$

        //tex: Finding equation for $\dot{m}$:

        //tex: $w_s = \frac{1}{2}(\alpha_2v_2^2 - \alpha_1v_1^2)+g(z_2-z_1)+w_f$

        //tex: $w_f = h_fg$

        //tex: $h_f = \frac{fLv_{avg}^2}{4r_{avg}g}$

        //tex: $w_f = \frac{fLv_{avg}^2}{4r_{avg}}$

        //tex: $w_s = \frac{1}{2}(\alpha_2v_2^2 - \alpha_1v_1^2)+g(z_2-z_1)+\frac{fLv_{avg}^2}{4r_{avg}}$

        //tex:$\dot{m}=\rho v_x\pi r^2$

        //tex:$ v_x = \frac{\dot{m}}{\rho\pi r^2}$

        //tex:$w_s=\dot{m}^2(\frac{\alpha_2}{2\rho^2\pi^2r_2^4}-\frac{\alpha_1}{2\rho^2\pi^2r_1^4})+\dot{m}(\frac{fL}{\rho^24\pi^2r_{avg}^5})+g(z_2-z_1)$

        //tex:$P_{pump}=\dot{m}w_s$

        //tex:$P_{pump}=\dot{m}^3(\frac{\alpha_2}{2\rho^2\pi^2r_2^4}-\frac{\alpha_1}{2\rho^2\pi^2r_1^4})+\dot{m}^2(\frac{fL}{\rho^24\pi^2r_{avg}^5})+\dot{m}(g(z_2-z_1))$

        //tex:$\dot{m}^3(\frac{\alpha_2}{2\rho^2\pi^2r_2^4}-\frac{\alpha_1}{2\rho^2\pi^2r_1^4})+\dot{m}^2(\frac{fL}{\rho^24\pi^2r_{avg}^5})+\dot{m}(g(z_2-z_1))-P_{pump}=0$

        public void CalculateMassFlowRate(dynamic[] obj)
        {
            double MassFlowRate;
            bool ContainsPipe = false;

            for (int i = 0; i < (obj.Length); i++)
            {
                if (obj[i].Value is Pipe)
                {
                    ContainsPipe = true;
                }
            }
            if (ContainsPipe)
            {
                double TotalDeltaFriction = 0;
                double TotalDeltaGravitational = 0;

                for (int i = 0; i < (obj.Length); i++)
                {
                    if (obj[i].Value is Pipe)
                    {
                        double DeltaGravitational = Pipe.G * (obj[i].Value.Elavation2 - obj[i].Value.Elavation1);
                        double DeltaFriction = (Pipe.FrictionFactor * obj[i].Value.Length) / (4 * Math.Pow(Reactor.FluidDensity, 2) * Math.Pow(Math.PI, 2) * Math.Pow((obj[i].Value.Radius), 5));
                        TotalDeltaFriction += DeltaFriction;
                        TotalDeltaGravitational += DeltaGravitational;
                    }
                }


                double Discriminant = Math.Pow(TotalDeltaGravitational, 2) - (4 * TotalDeltaFriction * -this.Power);
                if (Discriminant >= 0)
                {
                    double Root1 = (-TotalDeltaGravitational + Math.Sqrt(Discriminant)) / (2 * TotalDeltaFriction);
                    double Root2 = (-TotalDeltaGravitational - Math.Sqrt(Discriminant)) / (2 * TotalDeltaFriction);
                    if (Root1 > Root2)
                    {
                        MassFlowRate = Root1;
                    }
                    else
                    {
                        MassFlowRate = Root2;
                    }
                }
                else
                {
                    Console.Write("No real solutions");
                    MassFlowRate = 0;
                }

                for (int i = 0; i < (obj.Length); i++)
                {
                    obj[i].Value.MassFlowRate += MassFlowRate;
                }
            }
            else
            {
                for (int i = 0; i < (obj.Length); i++)
                {
                    if (i != (obj.Length - 1) && (obj[i + 1].Value is not null))
                    {
                        obj[i + 1].Value.MassFlowRate = obj[i].Value.MassFlowRate;
                    }
                }
            }


        }

        
    }

    public class TimeStep()
    {
        public double t = 0;
        public List<double> Time = new List<double>();
        public void TimeSteps(dynamic[] obj, dynamic[] nonSim, bool sim1, bool sim2, dynamic[] objects)
        {
            
            for (int i = 0; i < (obj.Length); i++)
            {

                if (obj[i].Value is not null)
                {

                    if (!sim1 && !sim2)
                    {
                        for (int j = 0; j < (objects.Length); j++)
                        {
                            objects[j].Value.VolumeFilledValues.Add(objects[j].Value.VolumeFilled);
                        }
                        Time.Add(t);
                        t += 1;
                    }
                    if (sim1)
                    {
                        for (int j = 0; j < (obj.Length); j++)
                        {
                            obj[j].Value.VolumeFilledValues.Add(obj[j].Value.VolumeFilled);
                            //non-sim obj
                        }
                        for (int k = 0; k < (nonSim.Length); k++)
                        {
                            nonSim[k].Value.VolumeFilledValues.Add(nonSim[k].Value.VolumeFilled);
                            //non-sim obj
                        }
                        Time.Add(t);
                        t += 1;
                    }
                    if (sim2)
                    {
                        for (int j = 0; j < (obj.Length); j++)
                        {
                            obj[j].Value.VolumeFilledValues.Add(obj[j].Value.VolumeFilled);
                           
                        }
                    }
                    if (i != (obj.Length - 1))
                    {
                        if ((obj[i + 1].Value.VolumeFilled < obj[i + 1].Value.Volume) && (obj[i].Value.VolumeFilled > 0))
                        {
                            if ((obj[i+1].Value.VolumeFilled + (obj[i].Value.MassFlowRate / Reactor.FluidDensity)) > (obj[i+1].Value.Volume))
                            {
                                if ((obj[i].Value.VolumeFilled - (obj[i + 1].Value.Volume - obj[i + 1].Value.VolumeFilled)) <0)
                                {
                                    obj[i + 1].Value.VolumeFilled += obj[i].Value.VolumeFilled;
                                    obj[i].Value.VolumeFilled = 0;
                                } else
                                {
                                    obj[i].Value.VolumeFilled -= (obj[i + 1].Value.Volume - obj[i + 1].Value.VolumeFilled);
                                    obj[i + 1].Value.VolumeFilled += (obj[i + 1].Value.Volume - obj[i + 1].Value.VolumeFilled);
                                }
                            } else
                            {
                                if ((obj[i].Value.VolumeFilled - (obj[i].Value.MassFlowRate / Reactor.FluidDensity)) <0)
                                {
                                    obj[i + 1].Value.VolumeFilled += obj[i].Value.VolumeFilled;
                                    obj[i].Value.VolumeFilled = 0;
                                } else
                                {
                                    obj[i].Value.VolumeFilled -= (obj[i].Value.MassFlowRate / Reactor.FluidDensity);
                                    obj[i + 1].Value.VolumeFilled += (obj[i].Value.MassFlowRate / Reactor.FluidDensity);
                                }

                            }
                            
                          

                        }
                    }
                    
                    

                }
                
            }
        }
    }

    public class Pipe
    {
        public double MassFlowRate = 0;
        public double VolumeFilled;
        public double Volume;
        public static readonly double CorrectionFactor = 1;
        public static readonly double G = 9.81;
        public static readonly double FrictionFactor = 0.02;
        public double Radius;
        public double Elavation2;
        public double Elavation1;
        public double Length;
        public List<double> VolumeFilledValues = new List<double>();

        public Pipe(double radius, double elavation2, double elavation1, double length)
        {
            this.Radius = radius;
            this.Elavation2 = elavation2;
            this.Elavation1 = elavation1;
            this.Length = length;
            this.Volume = length * Math.PI * Math.Pow(radius, 2);
            this.VolumeFilled = 0;
        }
    }
    public class Tank
    {
        public double MassFlowRate = 0;
        public double VolumeFilled;
        public double Volume;
        public List<double> VolumeFilledValues = new List<double>();
        public Tank(double volume, double volumeFilled)
        {
            this.Volume = volume;
            this.VolumeFilled = volumeFilled;
        }
    }

    public class FinalTank
    {
        public double MassFlowRate = 0;
        public double VolumeFilled;
        public double Volume;
        public List<double> VolumeFilledValues = new List<double>();
        public FinalTank(double volume)
        {
            this.Volume = volume;
            this.VolumeFilled = 0;
        }
    }

    public class Reactor
    {
        public double MassFlowRate = 0;
        public double VolumeFilled;
        public double Volume;
        public List<double> VolumeFilledValues = new List<double>();
        public static readonly double FluidDensity = 998;

        public Reactor(double volume)
        {
            Volume = volume;
            VolumeFilled = 0;

        }


        //tex: Assumptions:

        //tex: $k_0=1$

        //tex: $r_{net}$ is constant with time

        //tex:Using CSTR so concentration within reactor is same as at outlet

        //tex:For reaction: $aA + bB \rightleftharpoons cC + dD$

        //tex:$k_r = k_0e^{\frac{-Ea}{RT}}$

        //tex:$r_f = k_f[A]^m[B]^n$

        //tex:$r_r = k_r[C]^p[D]^q$

        //tex:$r_{net} = r_f - r_r$

        //tex:$[A_0] - atr_{net} = [A]$

        //tex:$[B_0] - btr_{net} = [B]$

        //tex:$[C_0] + ctr_{net} = [C]$

        //tex:$[D_0] + dtr_{net} = [D]$

        public double CalculateChemicalConcentrations()
        {
            return 0;
        }
    }


}
