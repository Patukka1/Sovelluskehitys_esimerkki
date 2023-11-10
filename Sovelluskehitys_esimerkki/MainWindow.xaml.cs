using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Data.SqlClient;
using System.Data;

namespace Sovelluskehitys_esimerkki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string solun_arvo;
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2101810\\Documents\\Uusitesti2.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();
            paivitaComboBox();
            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
        }

        private void painike_hae_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            }
            catch
            {
                Viestirivi.Text = "tietojen haku epäonnistui";
            }
        }
        private void painike_lisaa_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string sql = "INSERT INTO tuotteet (nimi, hinta) VALUES ('" + tuote_nimi.Text + "','" + tuote_hinta.Text + "')";

            SqlCommand komento = new SqlCommand(sql, kanta);
            komento.ExecuteNonQuery();

            kanta.Close();
            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);

        }
        private void paivitaDataGrid(string kysely, string taulu, DataGrid grid)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            /*tehdään sql komento*/
            SqlCommand komento = kanta.CreateCommand();
            komento.CommandText = "SELECT * FROM tuotteet"; // kysely

            /*tehdään data adapteri ja taulu johon tiedot haetaan*/
            SqlDataAdapter adapteri = new SqlDataAdapter(komento);
            DataTable dt = new DataTable("tuotteet");
            adapteri.Fill(dt);

            /*sijoitetaan data-taulun tiedot DataGridiin*/
            grid.ItemsSource = dt.DefaultView;

            kanta.Close();

        }
        private void paivitaComboBox()
        {
            SqlConnection kanta = new SqlConnection(polku);

            kanta.Open();
            SqlCommand komento = new SqlCommand("Select * From tuotteet", kanta);
            SqlDataReader lukija = komento.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Tuote", typeof(string));
            Combo_tuotteet.ItemsSource = dt.DefaultView;
            Combo_tuotteet.DisplayMemberPath = "Tuote";
            Combo_tuotteet.SelectedValuePath = "ID";

            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string tuote = lukija.GetString(1);
                dt.Rows.Add(id, tuote);

            }
            lukija.Close();
            kanta.Close();

        }

        private void Painike_poista_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string id = Combo_tuotteet.SelectedValue.ToString();
            SqlCommand komento = new SqlCommand("DELETE FROM tuotteet Where id = " + id + ";", kanta);
            komento.ExecuteNonQuery();
            kanta.Close();

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            paivitaComboBox();
        }

        private void tuote_lista_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int sarake = tuote_lista.CurrentCell.Column.DisplayIndex;
            solun_arvo = (e.Row.Item as DataRowView).Row[sarake].ToString();

            Viestirivi.Text = "sarake" + sarake + " Arvo " + solun_arvo;

        }

        private void tuote_lista_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int sarake = tuote_lista.CurrentCell.Column.DisplayIndex;
                string uusi_arvo = ((TextBox)e.EditingElement).Text;

                int tuote_id = int.Parse((e.Row.Item as DataRowView).Row[0].ToString());

                if (solun_arvo != uusi_arvo)
                {
                    string kysely = "";
                    string kanta_sarake = "";
                    SqlConnection kanta = new SqlConnection(polku);
                    kanta.Open();
                    if (sarake == 1) kanta_sarake = "nimi";
                    else if (sarake == 2) kanta_sarake = "hinta";

                    kysely = " update tuotteet Set " + kanta_sarake + "='" + uusi_arvo + "'Where id=" + tuote_id;

                    SqlCommand komento = new SqlCommand(kysely, kanta);
                    komento.ExecuteNonQuery();

                    kanta.Close();
                    Viestirivi.Text = "uusi arvo:" + uusi_arvo;

                }
                else
                {
                    Viestirivi.Text = "Arvo ei muuttunut";

                }
            }
            catch
            {
                Viestirivi.Text = "muokkaus ei onnistunut";
            }

        }
}    }   


    

