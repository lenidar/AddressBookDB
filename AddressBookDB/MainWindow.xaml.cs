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

namespace AddressBookDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AddressBookDBSampleDataContext db_con 
            = new AddressBookDBSampleDataContext(
                Properties.Settings.Default.BasicAddressBookConnectionString);
        Dictionary<int, string> countryList = new Dictionary<int, string>();
        Dictionary<int, string> cityList = new Dictionary<int, string>();
        Dictionary<int, string> contactList = new Dictionary<int, string>();

        public MainWindow()
        {
            InitializeComponent();

            selectCountry();
            selectCity();
            selectContact();
        }

        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            if(tbCountry.Text.Length > 0)
            {
                db_con.uspAddCountry(tbCountry.Text);
                tbCountry.Text = "";
                selectCountry();
            }
        }

        private void selectCountry()
        {
            List<uspSelectCountryResult> result = new List<uspSelectCountryResult>();

            result = db_con.uspSelectCountry().ToList();

            foreach(uspSelectCountryResult r in result)
                countryList[r.CountryID] = r.CountryName;

            cbCountry.ItemsSource = null;
            cbCountry.ItemsSource = countryList;
        }

        private void selectCity()
        {
            List<uspSelectCityResult> results = new List<uspSelectCityResult>();

            results = db_con.uspSelectCity().ToList();

            foreach (uspSelectCityResult r in results)
                cityList[r.CityID] = r.City;

            cbCityName.ItemsSource = null;
            cbCityName.ItemsSource = cityList;
        }

        private void selectContact()
        {
            List<uspSelectContactResult> results = new List<uspSelectContactResult>();

            results = db_con.uspSelectContact().ToList();

            foreach (uspSelectContactResult r in results)
                contactList[r.ContactID] = r.ContName;

            cbContactList.ItemsSource = null;
            cbContactList.ItemsSource = contactList;
        }

        private void btnRefreshCountryList_Click(object sender, RoutedEventArgs e)
        {
            selectCountry();
        }

        private void cbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(((KeyValuePair<int,string>)cbCountry.SelectedItem).Key.ToString());
        }

        private void btnAddCity_Click(object sender, RoutedEventArgs e)
        {
            if(cbCountry.SelectedIndex >= 0 && tbCity.Text.Length > 0)
            {
                db_con.uspAddCity(tbCity.Text
                    , ((KeyValuePair<int, string>)cbCountry.SelectedItem).Key);

                cbCountry.SelectedIndex = -1;
                tbCity.Text = "";
            }
        }

        private void btnRefresh2_Click(object sender, RoutedEventArgs e)
        {
            selectCity();
        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            if (cbContactList.SelectedIndex == -1) // add new contact
            {
                db_con.uspAddContact(tbLN.Text, tbFN.Text, tbMN.Text, DOBPicker.DisplayDate, tbAdd.Text
                    , ((KeyValuePair<int, string>)cbCityName.SelectedItem).Key, tbEadd.Text, tbCont.Text);
            }
            else // update contact
            {
                db_con.uspUpdateContact(((KeyValuePair<int, string>)cbContactList.SelectedItem).Key
                    , tbLN.Text, tbFN.Text, tbMN.Text, DOBPicker.DisplayDate, tbAdd.Text
                    , ((KeyValuePair<int, string>)cbCityName.SelectedItem).Key, tbEadd.Text, tbCont.Text);
            }

            tbLN.Text = "";
            tbFN.Text = "";
            tbMN.Text = "";
            DOBPicker.DisplayDate = DateTime.Now;
            tbAdd.Text = "";
            cbCityName.SelectedIndex = -1;
            tbEadd.Text = "";
            tbCont.Text = "";
            cbContactList.SelectedIndex = -1;

        }

        private void btnRefresh3_Click(object sender, RoutedEventArgs e)
        {
            selectContact();
            cbContactList.SelectedIndex = -1;
        }

        private void cbContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbContactList.SelectedIndex >= 0) 
            {
                List<uspSelectedContactResult> results = new List<uspSelectedContactResult>();

                results = db_con.uspSelectedContact(((KeyValuePair<int, string>)cbContactList.SelectedItem).Key).ToList();

                foreach(uspSelectedContactResult r in results)
                { 
                    tbLN.Text = r.ContactLN;
                    tbFN.Text = r.ContactFN;
                    tbMN.Text = r.ContactMN;
                    //MessageBox.Show(r.ContactDOB.ToString());
                    DOBPicker.Text = r.ContactDOB.ToString();
                    tbAdd.Text = r.ContactAdd;
                    // address city logic here
                    cbCityName.SelectedIndex = selectFromCityList(r.CityID);
                    tbEadd.Text = r.ContactEmailAdd;
                    tbCont.Text = r.ContactNum;
                }
            }
        }

        private int selectFromCityList(int cityID)
        {
            List<int> keys = cityList.Keys.ToList();            

            return keys.IndexOf(cityID);
        }
    }
}
