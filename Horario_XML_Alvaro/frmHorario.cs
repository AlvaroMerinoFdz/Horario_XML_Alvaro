using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Horario_XML_Alvaro
{
    public partial class frmHorario : Form
    {
        String ruta;
        bool cargado;
        public frmHorario()
        {
            InitializeComponent();
            cmbCurso.SelectedIndex = 0;
            cmbHora.SelectedIndex = 0;
            cmbDia.SelectedIndex = 0;
            lstbCiclo.SelectedIndex = 0;
            lstbModulos.SelectedIndex = 0;
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc;
            if(ofdAbrir.ShowDialog() == DialogResult.OK)
            {
                ruta = ofdAbrir.FileName;
                LimpiarDatos();

                //Se lee el XML con un XMLDocument
                xDoc = new XmlDocument();
                xDoc.Load(ruta);

                //Añadimos la tabla al horario directamente, con las columnas fijas
                dsDatos.Tables.Add(new DataTable("horario"));
                dsDatos.Tables[0].Columns.Add("Hora", typeof(string));
                dsDatos.Tables[0].Columns.Add("Lunes", typeof(string));
                dsDatos.Tables[0].Columns.Add("Martes", typeof(string));
                dsDatos.Tables[0].Columns.Add("Miercoles", typeof(string));
                dsDatos.Tables[0].Columns.Add("Jueves", typeof(string));
                dsDatos.Tables[0].Columns.Add("Viernes", typeof(string));

                //Recorremos el XMLDocument y vamos rellenando el DataSet junto con los ToolTipText
                XmlNodeList horario = xDoc.GetElementsByTagName("horario");
                XmlNodeList horas = ((XmlElement)horario[0]).GetElementsByTagName("hora");
                int contHora = 0;

                foreach(XmlElement hora in horas)
                {
                    //Construimos 2 arrays de strings, uno para los textos y otro con las ayudas.
                    string[] filaPantalla = new string[6];
                    string[] filaAyuda = new string[6];

                    //La primera columna de cada fila será la hora: primera, segunda, tercera, etc...
                    filaPantalla[0] = hora.GetAttribute("id").ToString();
                    filaAyuda[0] = "";
                    int col = 1;

                    XmlNodeList dias = hora.GetElementsByTagName("dia");
                    foreach(XmlElement dia in dias)
                    {
                        //Cargamos los datos de pantalla
                        XmlNodeList entradaPantalla = dia.GetElementsByTagName("pantalla");
                        filaPantalla[col] = ((XmlElement)entradaPantalla[0]).InnerText.ToString();

                        //Cargamos los datos de ayuda
                        XmlNodeList entradaAyuda = dia.GetElementsByTagName("ayuda");
                        filaAyuda[col] = ((XmlElement)entradaAyuda[0]).InnerText.ToString();

                        col++;
                    }
                    //Pero solo mostramos por pantalla los de la filaPantalla;
                    dsDatos.Tables[0].Rows.Add(filaPantalla);
                    dgvHorario.DataSource = dsDatos.Tables[0];

                    //Con esto metemos la ayuda a el ToolTipText
                    for (int c = 1;  c < filaAyuda.Length ;   c++)
                    {
                        dgvHorario.Rows[contHora].Cells[c].ToolTipText = filaAyuda[c];
                    }
                    contHora++;
                }
                cargado = true;
            }//fin del if
            else
            {
                cargado = false;
            }
        }//fin del método

        private void LimpiarDatos()
        {
            dsDatos = new DataSet(); ;
            dgvHorario.DataSource = null;
            dgvHorario.Rows.Clear();
            dgvHorario.Columns.Clear();
            dgvHorario.Refresh();
        }
    }
}
