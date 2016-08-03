using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiagramInteraccionColumnas
{
    public partial class Form1 : Form
    {

        double[] d;
        public Form1()
        {
            d = new double[12];
            InitializeComponent();

            //lineChart.ChartAreas[0].AxisY.ScaleView.Zoom(-150, 700); // -15<= y <=15
            //lineChart.ChartAreas[0].AxisX.ScaleView.Zoom(0, 100); // -15 <= x <= 2
            lineChart.ChartAreas[0].AxisX.Title = "Momento M (tn.m)";
            lineChart.ChartAreas[0].AxisY.Title = "Carga Axial P (tn)";
            lineChart.ChartAreas[0].AxisX.Name = "X";
            lineChart.ChartAreas[0].AxisX.LabelStyle.Interval = 10;
            lineChart.ChartAreas[0].AxisX.RoundAxisValues();
            lineChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            lineChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            lineChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            //MessageBox.Show((|se(b.Text)*0.10).ToString());
        }

        double fuction(double x)
        {
            //return (Math.Pow(x, 2) + 2 * Math.Sin(2 * x));
            return (Math.Pow(2, x) * Math.Sin(Math.Pow(2, x)));
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private double SIGNO(double number) {
            if (number > 0)
            {
                return 1;
            }
            else if (number == 0)
            {
                return 0;
            }
            else {
                return -1;
            }
        }

        private void btnGraficar_Click(object sender, EventArgs e)
        {
            try
            {
                lineChart.Series.Clear();
                d = new double[13];
                double Ast = 0; //Suma total de asx
                                //Calcular d array (Area de acero)
                for (int i = 1; i < 13; i++)
                {
                    TextBox asx = (TextBox)this.Controls.Find("as" + i.ToString(), true).FirstOrDefault();
                    TextBox dcx = (TextBox)this.Controls.Find("dc" + i.ToString(), true).FirstOrDefault();
                    if (double.Parse(asx.Text, CultureInfo.InvariantCulture) != 0)
                    {
                        d[i] = double.Parse(h.Text, CultureInfo.InvariantCulture) - double.Parse(dcx.Text, CultureInfo.InvariantCulture);
                    }
                    else {
                        d[i] = 0;
                    }
                    Ast += double.Parse(asx.Text, CultureInfo.InvariantCulture);
                    //MessageBox.Show("d[" + i + "] = " + d[i].ToString());
                }
                //MessageBox.Show(Ast.ToString());


                //Calculo de Compresion Pura
                double Po = ((0.85 * double.Parse(fc.Text, CultureInfo.InvariantCulture) * ((double.Parse(b.Text, CultureInfo.InvariantCulture) * double.Parse(h.Text, CultureInfo.InvariantCulture) * 10000) - Ast)) + (double.Parse(fy.Text, CultureInfo.InvariantCulture) * Ast)) / 1000;
                double Pn = 0.8 * Po;
                double phiPo = Po * 0.7;
                double phiPn = Pn * 0.7;
                double Mo = 0;
                double Mn = 0;


                //Calculo de Tracción pura
                double To = -double.Parse(fy.Text, CultureInfo.InvariantCulture) * Ast / 1000;
                double Tn = 0.9 * To;
                double TMo = 0;
                double TMn = 0;

                //Calculo de falla balanceada;
                double Cb = 0.59 * d[1] * 100;
                double ab = 0.85 * Cb;

                //Variables ocultas
                //Llenado de Es (Columna N)
                double[] Es = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    if (d[i + 1] != 0)
                    {
                        Es[i] = 0.003 * ((d[i + 1] * 100 / Cb) - 1);
                    }
                    else {
                        Es[i] = 0;
                    }
                }

                //Llenado de T (Columna O)
                double[] T = new double[7];
                for (int i = 0; i < 6; i++)
                {
                    TextBox asx = (TextBox)this.Controls.Find("as" + (i + 1).ToString(), true).FirstOrDefault();
                    if (Math.Abs(Es[i]) > 0.0021)
                    {
                        T[i] = -SIGNO(Es[i]) * 4200 * double.Parse(asx.Text, CultureInfo.InvariantCulture) / 1000;
                    }
                    else {
                        T[i] = Es[i] * 2000 * double.Parse(asx.Text, CultureInfo.InvariantCulture);
                    }
                }
                T[6] = 0.85 * (double.Parse(fc.Text, CultureInfo.InvariantCulture)) * (ab) * (double.Parse(b.Text, CultureInfo.InvariantCulture) * 100) / 1000;

                //Llenado de M (Columna P)
                double[] M = new double[7];
                for (int i = 0; i < 6; i++)
                {
                    M[i] = -T[i] * (d[i + 1] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2);
                }
                M[6] = -T[6] * (ab / 200 - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2);

                //Calculo de cargas actuales
                double D56 = 1.4 * double.Parse(txtMcm.Text, CultureInfo.InvariantCulture) + 1.7 * double.Parse(txtMcv.Text, CultureInfo.InvariantCulture);
                double D57 = 1.4 * double.Parse(txtPcm.Text, CultureInfo.InvariantCulture) + 1.7 * double.Parse(txtPcv.Text, CultureInfo.InvariantCulture);

                double E56 = 1.25 * (double.Parse(txtMcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtMcv.Text, CultureInfo.InvariantCulture)) + double.Parse(txtMcs.Text, CultureInfo.InvariantCulture);
                double E57 = 1.25 * (double.Parse(txtPcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtPcv.Text, CultureInfo.InvariantCulture)) + double.Parse(txtPcs.Text, CultureInfo.InvariantCulture);

                double F56 = 1.25 * (double.Parse(txtMcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtMcv.Text, CultureInfo.InvariantCulture)) - double.Parse(txtMcs.Text, CultureInfo.InvariantCulture);
                double F57 = 1.25 * (double.Parse(txtPcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtPcv.Text, CultureInfo.InvariantCulture)) - double.Parse(txtPcs.Text, CultureInfo.InvariantCulture);

                double G56 = 0.9 * double.Parse(txtMcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtMcs.Text, CultureInfo.InvariantCulture);
                double G57 = 0.9 * double.Parse(txtPcm.Text, CultureInfo.InvariantCulture) + double.Parse(txtPcs.Text, CultureInfo.InvariantCulture);

                double H56 = 0.9 * double.Parse(txtMcm.Text, CultureInfo.InvariantCulture) - double.Parse(txtMcs.Text, CultureInfo.InvariantCulture);
                double H57 = 0.9 * double.Parse(txtPcm.Text, CultureInfo.InvariantCulture) - double.Parse(txtPcs.Text, CultureInfo.InvariantCulture);

                //Calculo de Falla Balanceada
                double Pnb = T.Sum();
                double Mnb = M.Sum();

                //Variables Ocultas separadas
                double M56 = Math.Abs(D56);
                double M57 = Math.Abs(D57);
                double N56 = Math.Abs(E56);
                double N57 = Math.Abs(E57);
                double O56 = Math.Abs(F56);
                double O57 = Math.Abs(F57);
                double P56 = Math.Abs(G56);
                double P57 = Math.Abs(G57);
                double Q56 = Math.Abs(H56);
                double Q57 = Math.Abs(H57);

                //Tabla de Pn y Mn - Diagrama de interacción.
                double[] tablaC = new double[39];
                for (int i = 0; i < 39; i++)
                {
                    tablaC[i] = double.Parse(h.Text, CultureInfo.InvariantCulture) * (100 * (i + 1)) / 40;
                    //MessageBox.Show(tablaC[i].ToString());
                }

                double[] tablaA = new double[39];
                for (int i = 0; i < 39; i++)
                {
                    tablaA[i] = 0.85 * tablaC[i];
                    //MessageBox.Show(tablaA[i].ToString());
                }

                double[] tablaPn = new double[41];
                tablaPn[0] = To;
                //MessageBox.Show(tablaPn[0].ToString());
                TextBox as6 = (TextBox)this.Controls.Find("as" + (6).ToString(), true).FirstOrDefault();
                double B32 = double.Parse(as6.Text, CultureInfo.InvariantCulture);
                TextBox as5 = (TextBox)this.Controls.Find("as" + (5).ToString(), true).FirstOrDefault();
                double B30 = double.Parse(as5.Text, CultureInfo.InvariantCulture);
                TextBox as4 = (TextBox)this.Controls.Find("as" + (4).ToString(), true).FirstOrDefault();
                double B28 = double.Parse(as4.Text, CultureInfo.InvariantCulture);
                TextBox as3 = (TextBox)this.Controls.Find("as" + (3).ToString(), true).FirstOrDefault();
                double B26 = double.Parse(as3.Text, CultureInfo.InvariantCulture);
                TextBox as2 = (TextBox)this.Controls.Find("as" + (2).ToString(), true).FirstOrDefault();
                double B24 = double.Parse(as2.Text, CultureInfo.InvariantCulture);
                TextBox as1 = (TextBox)this.Controls.Find("as" + (1).ToString(), true).FirstOrDefault();
                double B22 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as7 = (TextBox)this.Controls.Find("as" + (7).ToString(), true).FirstOrDefault();
                double D22 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as8 = (TextBox)this.Controls.Find("as" + (8).ToString(), true).FirstOrDefault();
                double D24 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as9 = (TextBox)this.Controls.Find("as" + (9).ToString(), true).FirstOrDefault();
                double D26 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as10 = (TextBox)this.Controls.Find("as" + (10).ToString(), true).FirstOrDefault();
                double D28 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as11 = (TextBox)this.Controls.Find("as" + (11).ToString(), true).FirstOrDefault();
                double D30 = double.Parse(as1.Text, CultureInfo.InvariantCulture);
                TextBox as12 = (TextBox)this.Controls.Find("as" + (12).ToString(), true).FirstOrDefault();
                double D32 = double.Parse(as1.Text, CultureInfo.InvariantCulture);

                for (int i = 1; i < 40; i++)
                {
                    tablaPn[i] = (Math.Abs((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B22 / 1000 : -((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B22) + (Math.Abs((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B24 / 1000 : -((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B24) + (Math.Abs((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B26 / 1000 : -((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B26) + (Math.Abs((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B28 / 1000 : -((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B28) + (Math.Abs((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B30 / 1000 : -((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B30) + (Math.Abs(d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0) > 0.0021 ? -SIGNO(d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0) * 4200 * B32 / 1000 : -(d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0) * 2000 * B32) + (Math.Abs((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i-1])-1) : 0))> 0.0021 ? -SIGNO((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i-1])-1) : 0))*4200 *D22 / 1000 : -((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i-1])-1) : 0))*2000 *D22) + (Math.Abs((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D24 / 1000 : -((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D24) + (Math.Abs((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D26 / 1000 : -((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D26) + (Math.Abs((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D28 / 1000 : -((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D28) + (Math.Abs((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D30 / 1000 : -((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D30) + (Math.Abs((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D32 / 1000 : -((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D32) + (0.85 * (double.Parse(fc.Text, CultureInfo.InvariantCulture)) * (tablaA[i - 1]) * (double.Parse(b.Text, CultureInfo.InvariantCulture) * 100) / 1000);
                }
                //MessageBox.Show(tablaPn[0].ToString());
                tablaPn[40] = Po;

                //Calculo de Mn Correcto
                double[] tablaMn = new double[41];
                tablaMn[0] = Mo;
                //MessageBox.Show(tablaMn[0].ToString());
                for (int i = 1; i < 40; i++)
                {
                    tablaMn[i] = -((Math.Abs((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B22 / 1000 : -((d[1] != 0 ? 0.003 * ((d[1] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B22)) * (d[1] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B24 / 1000 : -((d[2] != 0 ? 0.003 * ((d[2] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B24)) * (d[2] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B26 / 1000 : -((d[3] != 0 ? 0.003 * ((d[3] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B26)) * (d[3] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B28 / 1000 : -((d[4] != 0 ? 0.003 * ((d[4] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B28)) * (d[4] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B30 / 1000 : -((d[5] != 0 ? 0.003 * ((d[5] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B30)) * (d[5] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * B32 / 1000 : -((d[6] != 0 ? 0.003 * ((d[6] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * B32)) * (d[6] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) - ((Math.Abs((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D22 / 1000 : -((d[7] != 0 ? 0.003 * ((d[7] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D22)) * (d[7] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -
-((Math.Abs((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D24 / 1000 : -((d[8] != 0 ? 0.003 * ((d[8] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D24)) * (d[8] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -
-((Math.Abs((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D26 / 1000 : -((d[9] != 0 ? 0.003 * ((d[9] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D26)) * (d[9] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -
-((Math.Abs((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D28 / 1000 : -((d[10] != 0 ? 0.003 * ((d[10] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D28)) * (d[10] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -
-((Math.Abs((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D30 / 1000 : -((d[11] != 0 ? 0.003 * ((d[11] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D30)) * (d[11] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -
-((Math.Abs((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) > 0.0021 ? -SIGNO((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) * 4200 * D32 / 1000 : -((d[12] != 0 ? 0.003 * ((d[12] * 100 / tablaC[i - 1]) - 1) : 0)) * 2000 * D32)) * (d[12] - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2) -(0.85 * (double.Parse(fc.Text, CultureInfo.InvariantCulture)) * (tablaA[i - 1]) * (double.Parse(b.Text, CultureInfo.InvariantCulture) * 100) / 1000) * (tablaA[i - 1] / 200 - double.Parse(h.Text, CultureInfo.InvariantCulture) / 2);
                    //MessageBox.Show(tablaMn[i].ToString());
                }
                //MessageBox.Show(tablaMn[1].ToString());
                tablaMn[40] = Mo;

                double[] tablaphi = new double[41];
                for (int i = 0; i < 41; i++)
                {
                    tablaphi[i] = (tablaPn[i] < 0 ? 0.9 : 0.7);
                }

                double[] tablaphiPn = new double[41];
                for (int i = 0; i < 41; i++)
                {
                    tablaphiPn[i] = tablaphi[i] * tablaPn[i];
                }

                double[] tablaphiMn = new double[41];
                for (int i = 0; i < 41; i++)
                {
                    tablaphiMn[i] = tablaMn[i] * tablaphi[i];
                }


                lineChart.Series.Add("Curva Diagrama Nominal");
                for (int i = 0; i < 41; i++)
                {
                    lineChart.Series[0].Points.AddXY(tablaMn[i], tablaPn[i]);
                    lineChart.Series[0].BorderWidth = 2;
                    lineChart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                }

                lineChart.Series.Add("Curva de Diseño");
                for (int i = 0; i < 41; i++)
                {
                    lineChart.Series[1].Points.AddXY(tablaphiMn[i], tablaphiPn[i]);
                    lineChart.Series[1].BorderWidth = 2;
                    lineChart.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                }

                //=SERIES("Curva de diseño";Hoja1!$H$65:$H$68;Hoja1!$L$65:$L$68;3)

                /*
                lineChart.Series.Add("Curva de Diseño2");
                for (int i = 0; i < 51; i++)
                {
                    lineChart.Series[2].Points.AddXY(tablaphiMn[i], phiPn);
                    lineChart.Series[2].BorderWidth = 2;
                    lineChart.Series[2].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                }*/


                lineChart.Series.Add("Curva de Diseño2");
                for (int i = 0; i < tablaMn.Max(); i++)
                {
                    lineChart.Series[2].Points.AddXY(i, phiPn);
                    lineChart.Series[2].BorderWidth = 2;
                    lineChart.Series[2].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                }
                lineChart.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void numCapas_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 1; i < (int)numCapas.Value+1; i++)
            {
                TextBox asx = (TextBox)this.Controls.Find("as" + (i).ToString(), true).FirstOrDefault();
                asx.Visible = true;
                Label lblasx = (Label)this.Controls.Find("lblAs" + (i).ToString(), true).FirstOrDefault();
                lblasx.Visible = true;
                Label lblunitasx = (Label)this.Controls.Find("lblunitas" + (i).ToString(), true).FirstOrDefault();
                lblunitasx.Visible = true;
                //asx.Text = "0";
                TextBox dcx = (TextBox)this.Controls.Find("dc" + (i).ToString(), true).FirstOrDefault();
                dcx.Visible = true;
                //dcx.Text = "0";
                Label lbldcx = (Label)this.Controls.Find("lblDc" + (i).ToString(), true).FirstOrDefault();
                lbldcx.Visible = true;
                Label lblunitdcx = (Label)this.Controls.Find("lblunitdc" + (i).ToString(), true).FirstOrDefault();
                lblunitdcx.Visible = true;
            }
            for (int i = (int)numCapas.Value+1; i < 13; i++)
            {
                TextBox asx = (TextBox)this.Controls.Find("as" + (i).ToString(), true).FirstOrDefault();
                asx.Visible = false;
                asx.Text = "0";
                Label lblasx = (Label)this.Controls.Find("lblAs" + (i).ToString(), true).FirstOrDefault();
                lblasx.Visible = false;
                Label lblunitasx = (Label)this.Controls.Find("lblunitas" + (i).ToString(), true).FirstOrDefault();
                lblunitasx.Visible = false;

                TextBox dcx = (TextBox)this.Controls.Find("dc" + (i).ToString(), true).FirstOrDefault();
                dcx.Visible = false;
                dcx.Text = "0";
                Label lbldcx = (Label)this.Controls.Find("lblDc" + (i).ToString(), true).FirstOrDefault();
                lbldcx.Visible = false;
                Label lblunitdcx = (Label)this.Controls.Find("lblunitdc" + (i).ToString(), true).FirstOrDefault();
                lblunitdcx.Visible = false;
            }
        }

        private void btnResetZoom_Click(object sender, EventArgs e)
        {
            lineChart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            lineChart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "diagrama.png";
            // set filters - this can be done in properties as well
            savefile.Filter = "Image Files (*.png,*.bmp, *.jpg)|*.png;*.bmp;*.jpg";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                lineChart.SaveImage(savefile.FileName, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
            }
            
        }

        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();

        private void lineChart_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = lineChart.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around the point)
                        if (Math.Abs(pos.X - pointXPixel) < 4 &&
                            Math.Abs(pos.Y - pointYPixel) < 4)
                        {
                            tooltip.Show("X=" + prop.XValue + ", Y=" + prop.YValues[0], this.lineChart,
                                            pos.X, pos.Y - 15);
                        }
                    }
                }
            }
        }
    }
}
