using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private string[] ayuda = {"Joaquin", "Jose Alberto", "Inma", "Ambrosio", "Marcelo", "Fernado"};

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, // x-coordinate of upper-left corner
    int nTopRect, // y-coordinate of upper-left corner
    int nRightRect, // x-coordinate of lower-right corner
    int nBottomRect, // y-coordinate of lower-right corner
    int nWidthEllipse, // height of ellipse
    int nHeightEllipse // width of ellipse
 );

        public frmHorario()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            cargarDefecto();

        }

        
        private void cargarDefecto()
        {
            cmbCurso.SelectedIndex = 0;
            cmbHora.SelectedIndex = 0;
            cmbDia.SelectedIndex = 0;
            lstbCiclo.SelectedIndex = 0;
            lstbModulos.SelectedIndex = 0;
            crearTabla();
            dgvHorario.DataSource = dsDatos.Tables[0];
            //seleccionarCelda();
        }

        private void seleccionarCelda()
        {
            try
            {
                int fila, columna;
                fila = cmbHora.SelectedIndex;
                columna = cmbDia.SelectedIndex + 1;
                dgvHorario.Rows[fila].Cells[columna].Selected = true;
            }
            catch(Exception e) { 
            }
            
        }
        private void crearTabla()
        {
            rellenarColumnas();
            rellenarFilas();
            
        }
        private void rellenarColumnas()
        {
            dsDatos.Tables.Add(new DataTable("horario"));
            dsDatos.Tables[0].Columns.Add("Hora", typeof(string));
            dsDatos.Tables[0].Columns.Add("Lunes", typeof(string));
            dsDatos.Tables[0].Columns.Add("Martes", typeof(string));
            dsDatos.Tables[0].Columns.Add("Miercoles", typeof(string));
            dsDatos.Tables[0].Columns.Add("Jueves", typeof(string));
            dsDatos.Tables[0].Columns.Add("Viernes", typeof(string));
        }
        private void rellenarFilas()
        {
            dsDatos.Tables[0].Rows.Add("8:30-9:25");
            dsDatos.Tables[0].Rows.Add("9:25-10:20");
            dsDatos.Tables[0].Rows.Add("10:20-11:15");
            dsDatos.Tables[0].Rows.Add("11:45-12:40");
            dsDatos.Tables[0].Rows.Add("12:40-13:35");
            dsDatos.Tables[0].Rows.Add("13:35-14:30");
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
                rellenarColumnas();

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
            //Comprobarmos que está completo y si lo está cambiamos el estado del botón guardar.
            if (comprobarRelleno())
            {
                btnGuardar.Enabled = true;
            }
            else
            {
                btnGuardar.Enabled = false;
            }

        }//fin del método

        private void LimpiarDatos()
        {
            dsDatos = new DataSet(); ;
            dgvHorario.DataSource = null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int fila, columna;
            fila = cmbHora.SelectedIndex;
            columna = cmbDia.SelectedIndex + 1;
            dgvHorario.Rows[fila].Cells[columna].Value = lstbModulos.SelectedItem;
            dgvHorario.Rows[fila].Cells[columna].ToolTipText = ayuda[lstbModulos.SelectedIndex];

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int fila, columna;
            fila = cmbHora.SelectedIndex;
            columna = cmbDia.SelectedIndex + 1;
            dgvHorario.Rows[fila].Cells[columna].Value = "";
            dgvHorario.Rows[fila].Cells[columna].ToolTipText = "";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc;
            XmlTextWriter xtw;
            if (sfdGuardar.ShowDialog() == DialogResult.OK)
            {
                xtw = new XmlTextWriter(sfdGuardar.FileName, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                xDoc = new XmlDocument();
                //Construir un nuevo XMLDocument vacío, el cuál se irá rellenando con los datos
                //del Dataset asociado al DataGridView y se le irán añadiendo para cada día
                //una nueva etiqueta con la información del toltiptext
                XmlElement elementoRaiz = xDoc.CreateElement(string.Empty, "horario", string.Empty);
                xDoc.AppendChild(elementoRaiz);
                for (int f = 0; f < dgvHorario.Rows.Count; f++)
                {
                    XmlElement xHora = xDoc.CreateElement(string.Empty, "hora", string.Empty);
                    //El atributo id con la hora se puede sacar de la primera columna del DataGridView
                    xHora.SetAttribute("id", dgvHorario.Rows[f].Cells[0].Value.ToString());
                    for (int c = 1; c < dgvHorario.Columns.Count; c++)
                    {
                        XmlElement xDia = xDoc.CreateElement(string.Empty, "dia", string.Empty);
                        XmlElement xPantalla = xDoc.CreateElement(string.Empty, "pantalla", string.Empty);
                        XmlElement xAyuda = xDoc.CreateElement(string.Empty, "ayuda", string.Empty);
                        XmlText xTxTPantalla = xDoc.CreateTextNode(dgvHorario.Rows[f].Cells[c].Value.ToString());
                        xPantalla.AppendChild(xTxTPantalla);
                        XmlText xTxTAyuda = xDoc.CreateTextNode(dgvHorario.Rows[f].Cells[c].ToolTipText);
                        xAyuda.AppendChild(xTxTAyuda);
                        xDia.AppendChild(xPantalla);
                        xDia.AppendChild(xAyuda);
                        xHora.AppendChild(xDia);
                    }
                    elementoRaiz.AppendChild(xHora);
                }
                //Por último, escribir el contenido del XMLDocument en el archivo y cerrar conexión
                xDoc.Save(xtw);
                xtw.Close();
                MessageBox.Show("Se ha guardado el horario", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }//fin del método guardar.

        private void cmbHora_SelectedIndexChanged(object sender, EventArgs e)
        {
            seleccionarCelda();
        }

        private void cmbDia_SelectedIndexChanged(object sender, EventArgs e)
        {
            seleccionarCelda();
        }

        private void dgvHorario_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Primero tenemos que desactivar el selected index de los combo box,
            //para que no pete el programa
            cmbDia.SelectedIndexChanged -= cmbDia_SelectedIndexChanged;
            cmbHora.SelectedIndexChanged -= cmbHora_SelectedIndexChanged;

            if(dgvHorario.SelectedCells[0].ColumnIndex == 0)
            {
                cmbDia.SelectedIndex = 0;
                cmbHora.SelectedIndex = dgvHorario.SelectedCells[0].RowIndex;
            }
            else
            {
                cmbHora.SelectedIndex = dgvHorario.SelectedCells[0].RowIndex;
                cmbDia.SelectedIndex = dgvHorario.SelectedCells[0].ColumnIndex - 1;
            }

            //Volvemos a activar los eventos de los combos para que funcione correctamente.
            cmbDia.SelectedIndexChanged += cmbDia_SelectedIndexChanged;
            cmbHora.SelectedIndexChanged += cmbHora_SelectedIndexChanged;
        }

        private bool comprobarRelleno()
        {
            foreach (DataGridViewRow fila in dgvHorario.Rows)
            {
                for (int i = 0; i < fila.Cells.Count; i++)
                {
                    if (String.IsNullOrEmpty(fila.Cells[i].Value.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void dgvHorario_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (comprobarRelleno())
            {
                btnGuardar.Enabled = true;
            }
            else
            {
                btnGuardar.Enabled = false;
            }
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
