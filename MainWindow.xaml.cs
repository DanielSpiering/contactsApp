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
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;

namespace sqlWpfContacts {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        //set up connection strin and database helper
        static private string sqlConnectionString = @"Data Source=DESKTOP-3TU21FQ\SQLEXPRESS;Integrated Security=True";
        DatabaseHelper dbContacts = new DatabaseHelper(sqlConnectionString);
        //set up lists for each of the list boxes i have
        private List<Contact> _activeContacts;
        private List<Contact> _favoriteContacts;
        private List<Contact> _trashContacts;
        private List<Contact> _searchContacts;

        public MainWindow() {
            GetContacts();
            InitializeComponent();
            ListFavorites();
            ListContacts();
            ListTrash();
        }//end main

        private void btnAddContact_Click(object sender, RoutedEventArgs e) {            
            
            //sets up and executes query to add the contact to the database
            string queryContact = $"INSERT INTO Contacts (firstName,lastName,nickName,photo,website,company,street,city,state,zipCode,country,notes,favorite,active,[delete]) VALUES('{tbxFirstName.Text}','{tbxLastName.Text}','{tbxNickName.Text}','{tbxPhoto.Text}','{tbxWebsite.Text}','{tbxCompany.Text}','{tbxStreet.Text}','{tbxCity.Text}','{tbxState.Text}','{tbxZipCode.Text}','{tbxCountry.Text}','{tbxNotes.Text}','0','1','0')";
            //constructs a query to add numbers and emails to database depending on how many were entered
            if (tbxPhoneNumber1.Text != "") {
                CheckIfPhoneNumberInDatabase(tbxPhoneNumber1.Text);         
            }//end if
            if (tbxPhoneNumber2.Text != "") {
                CheckIfPhoneNumberInDatabase(tbxPhoneNumber2.Text);
            }//end if
            if (tbxPhoneNumber3.Text != "") {
                CheckIfPhoneNumberInDatabase(tbxPhoneNumber3.Text);
            }//end if
            if (tbxEmail1.Text != "") {
                CheckIfEmailInDatabase(tbxEmail1.Text);
            }//end if
            if (tbxEmail2.Text != "") {
                CheckIfEmailInDatabase(tbxEmail1.Text);
            }//end if
            if (tbxEmail3.Text != "") {
                CheckIfEmailInDatabase(tbxEmail1.Text);
            }//end if

            using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                thisConnection.Open();
                using (SqlCommand cmd = new SqlCommand(queryContact, thisConnection)) {                   
                    cmd.ExecuteNonQuery();                    
                }//end command
               
                thisConnection.Close();
            }//end connection
            //get ids of contacts, numbers, and emails so they can be added to child tables
            object[][] resultsContacts = dbContacts.ExecuteReader($"SELECT Id FROM Contacts WHERE firstName='{tbxFirstName.Text}' AND lastName='{tbxLastName.Text}' AND nickName='{tbxNickName.Text}' AND website='{tbxWebsite.Text}' AND company='{tbxCompany.Text}' AND street='{tbxStreet.Text}' AND city='{tbxCity.Text}' AND state='{tbxState.Text}' AND zipCode='{tbxZipCode.Text}' AND country='{tbxCountry.Text}' AND notes='{tbxNotes.Text}'");

            object[][] resultsPhoneNumbers1 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber1.Text}'");
            object[][] resultsPhoneNumbers2 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber2.Text}'");
            object[][] resultsPhoneNumbers3 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber3.Text}'");
            
            object[][] resultsEmails1 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail1.Text}'");
            object[][] resultsEmails2 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail2.Text}'");
            object[][] resultsEmails3 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail3.Text}'");
            
            //inserts ids into child tables
            using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                thisConnection.Open();
                if (tbxPhoneNumber1.Text!="") {
                    using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{resultsContacts[0][0]}','{resultsPhoneNumbers1[0][0]}')", thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    if (tbxPhoneNumber2.Text != "") {
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{resultsContacts[0][0]}','{resultsPhoneNumbers2[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }//end if
                    if (tbxPhoneNumber3.Text != "") {
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{resultsContacts[0][0]}','{resultsPhoneNumbers3[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }//end if
                }//end if
                
                if (tbxEmail1.Text != "") {
                    using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{resultsContacts[0][0]}','{resultsEmails1[0][0]}')", thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    if (tbxEmail2.Text != "") {
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{resultsContacts[0][0]}','{resultsEmails2[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }//end if
                    if (tbxEmail3.Text!="") {
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{resultsContacts[0][0]}','{resultsEmails3[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }//end if

                }//end if               
                               
                thisConnection.Close();
            }//end connection
            _activeContacts.Add(new Contact((int)resultsContacts[0][0], (string)tbxFirstName.Text, (string)tbxLastName.Text, (string)tbxNickName.Text, (string)tbxPhoto.Text, (string)tbxWebsite.Text, (string)tbxCompany.Text, (string)tbxStreet.Text, (string)tbxCity.Text, (string)tbxState.Text, (string)tbxZipCode.Text, (string)tbxCountry.Text, (string)tbxNotes.Text, (bool)false, (bool)true, (bool)false));
            ListContacts();
        }//end event
        private void btnListContacts_Click(object sender, RoutedEventArgs e) {
            lsbSearch.Visibility = Visibility.Hidden;
            ListContacts();
        }//end event
        private void btnDeleteContact_Click(object sender, RoutedEventArgs e) {
            int Id;
            if (lsbContacts.SelectedItem != null || lsbFavorites.SelectedItem != null) {
                if (lsbContacts.SelectedItem != null) {
                    Id = _activeContacts.ElementAt(lsbContacts.SelectedIndex).id;
                } else {
                    Id = _favoriteContacts.ElementAt(lsbFavorites.SelectedIndex).id;
                }

                //set contact to be deleted later but isnt immediately removed from database
                string query = $"UPDATE Contacts SET [delete]=1 WHERE Id='{Id}'";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {

                        cmd.ExecuteNonQuery();
                    }//end command

                    thisConnection.Close();
                }//end conection

                //update all listboxes
                GetContacts();
                ListTrash();
                ListContacts();
                ListFavorites();
            }//end if                     
        }//end event      
        private void btnEditContact_Click(object sender, RoutedEventArgs e) {
            if (lsbContacts.SelectedItem != null || lsbFavorites.SelectedItem != null) {
                //navigates to the add contacts tab and clears the form
                ClearForm();
                tcTabs.SelectedIndex = 1;
                btnConfirmEdit.Visibility=Visibility.Visible;
                btnAddContact.Visibility = Visibility.Hidden;
                //determines which listbox contains the contact being selected
                int Id = 0;
                if (lsbContacts.SelectedItem != null) {
                    Id = _activeContacts.ElementAt(lsbContacts.SelectedIndex).id;
                }//end if
                if (lsbFavorites.SelectedItem != null) {
                    Id = _favoriteContacts.ElementAt(lsbFavorites.SelectedIndex).id;
                }//end if
                object[][] phoneNumb1;
                object[][] phoneNumb2;
                object[][] phoneNumb3;

                object[][] email1;
                object[][] email2;
                object[][] email3;

                //get all contact info from contact table
                object[][] results = dbContacts.ExecuteReader($"SELECT * FROM Contacts WHERE Id={Id}");

                //get phonenumber ids from child table using contact id
                object[][] resultPhoneNumbers = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE userId={Id}");

                //get email ids from child table using contact id
                object[][] resultEmails = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE userId={Id}");

                //adds contact info to the add contact form
                //if any numbers are available they are shown
                if (resultPhoneNumbers.Length != 0) {
                    phoneNumb1 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[0][2].ToString()}");
                    tbxPhoneNumber1.Text = phoneNumb1[0][1].ToString();

                    if (resultPhoneNumbers.Length == 2 || resultPhoneNumbers.Length == 3) {
                        phoneNumb2 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[1][2].ToString()}");
                        tbxPhoneNumber2.Text = phoneNumb2[0][1].ToString();
                    }//end if
                    if (resultPhoneNumbers.Length == 3) {
                        phoneNumb3 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[2][2].ToString()}");
                        tbxPhoneNumber3.Text = phoneNumb3[0][1].ToString();
                    }//end if
                }//end if

                //if any emails are available they are shown
                if (resultEmails.Length != 0) {
                    email1 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[0][2].ToString()}");
                    tbxEmail1.Text = email1[0][1].ToString();

                    if (resultEmails.Length == 2 || resultEmails.Length == 3) {
                        email2 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[1][2].ToString()}");
                        tbxEmail2.Text = email2[0][1].ToString();
                    }//end if
                    if (resultEmails.Length == 3) {
                        email3 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[2][2].ToString()}");
                        tbxEmail3.Text = email3[0][1].ToString();
                    }//end if
                }//end if

                tbxFirstName.Text = results[0][1].ToString();
                tbxLastName.Text = results[0][2].ToString();
                tbxNickName.Text = results[0][3].ToString();
                tbxPhoto.Text = results[0][4].ToString();
                tbxWebsite.Text = results[0][5].ToString();
                tbxCompany.Text = results[0][6].ToString();
                tbxStreet.Text = results[0][7].ToString();
                tbxCity.Text = results[0][8].ToString();
                tbxState.Text = results[0][9].ToString();
                tbxZipCode.Text = results[0][10].ToString();
                tbxCountry.Text = results[0][11].ToString();
                tbxNotes.Text = results[0][12].ToString();
            }//end if             
        }//end event
        private void btnConfirmEdit_Click(object sender, RoutedEventArgs e) {
            if (lsbContacts.SelectedItem != null || lsbFavorites.SelectedItem != null) {
                //check to see which list box the contact is selected from
                int Id = 0;
                if (lsbContacts.SelectedItem != null) {
                    Id = _activeContacts.ElementAt(lsbContacts.SelectedIndex).id;
                }//end if
                if (lsbFavorites.SelectedItem != null) {
                    Id = _favoriteContacts.ElementAt(lsbFavorites.SelectedIndex).id;
                }//end if

                //update contacts table with the current values in the add contacts form
                string query = $"UPDATE Contacts SET firstName='{tbxFirstName.Text}', lastName='{tbxLastName.Text}', nickName='{tbxNickName.Text}', photo='{tbxPhoto.Text}', website='{tbxWebsite.Text}', company='{tbxCompany.Text}', street='{tbxStreet.Text}', city='{tbxCity.Text}', state='{tbxState.Text}', zipCode='{tbxZipCode.Text}', country='{tbxCountry.Text}', notes='{tbxNotes.Text}' WHERE Id={Id}";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    thisConnection.Close();
                }//end connection
                //get the contact phonenumber ids
                object[][] phoneNumberResults = dbContacts.ExecuteReader($"SELECT phoneNumberId FROM contactPhoneNumbers WHERE userId='{Id}'");
                //get the contact email ids
                object[][] emailResults = dbContacts.ExecuteReader($"SELECT emailId FROM contactEmails WHERE userId='{Id}'");

                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    //if there is a phone number in the textbox and there is a phonenumber present in the database it is updated else it is added
                    if ((tbxPhoneNumber1.Text != "" && phoneNumberResults.Length > 0)|| (tbxPhoneNumber1.Text == "" && phoneNumberResults.Length > 0)) {
                        string queryUpdatePN1 = $"UPDATE PhoneNumbers SET phoneNumber='{tbxPhoneNumber1.Text}' WHERE Id={phoneNumberResults[0][0]}";
                        using (SqlCommand cmd = new SqlCommand(queryUpdatePN1, thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    } else if (tbxPhoneNumber1.Text != "" && phoneNumberResults.Length == 0) {
                        CheckIfPhoneNumberInDatabase(tbxPhoneNumber1.Text);
                        object[][] resultsPhoneNumbers1 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber1.Text}'");
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{Id}','{resultsPhoneNumbers1[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }
                    //same for second phone number
                    if ((tbxPhoneNumber2.Text != "" && phoneNumberResults.Length > 1) || (tbxPhoneNumber2.Text == "" && phoneNumberResults.Length > 1)) {
                        string queryUpdatePN2 = $"UPDATE PhoneNumbers SET phoneNumber='{tbxPhoneNumber2.Text}' WHERE Id={phoneNumberResults[1][0]}";
                        using (SqlCommand cmd = new SqlCommand(queryUpdatePN2, thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    } else if (tbxPhoneNumber2.Text != "" && phoneNumberResults.Length == 1) {
                        CheckIfPhoneNumberInDatabase(tbxPhoneNumber2.Text);
                        object[][] resultsPhoneNumbers2 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber2.Text}'");
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{Id}','{resultsPhoneNumbers2[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }
                    //same for 3rd phone numnber
                    if ((tbxPhoneNumber3.Text != "" && phoneNumberResults.Length == 3)|| (tbxPhoneNumber3.Text == "" && phoneNumberResults.Length == 3)) {
                        string queryUpdatePN3 = $"UPDATE PhoneNumbers SET phoneNumber='{tbxPhoneNumber3.Text}' WHERE Id={phoneNumberResults[2][0]}";
                        using (SqlCommand cmd = new SqlCommand(queryUpdatePN3, thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    } else if (tbxPhoneNumber3.Text != "" && phoneNumberResults.Length == 2) {
                        CheckIfPhoneNumberInDatabase(tbxPhoneNumber3.Text);
                        object[][] resultsPhoneNumbers3 = dbContacts.ExecuteReader($"SELECT Id FROM PhoneNumbers WHERE phoneNumber='{tbxPhoneNumber3.Text}'");
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactPhoneNumbers (userId,phoneNumberId) VALUES ('{Id}','{resultsPhoneNumbers3[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }

                    //if there is an email in the textbox and there is an email present in the database it is updated else it is added
                    if ((tbxEmail1.Text != "" && emailResults.Length > 0)|| (tbxEmail1.Text == "" && emailResults.Length > 0)) {
                        string queryUpdateEM1 = $"UPDATE Emails SET email='{tbxEmail1.Text}' WHERE Id={emailResults[0][0]}";
                        using (SqlCommand cmd = new SqlCommand(queryUpdateEM1, thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    } else if (tbxEmail1.Text != "" && emailResults.Length == 0) {
                        CheckIfEmailInDatabase(tbxEmail1.Text);
                        object[][] resultsEmails1 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail1.Text}'");
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{Id}','{resultsEmails1[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }
                    //same for second email
                    if ((tbxEmail2.Text != "" && emailResults.Length >1)|| (tbxEmail2.Text == "" && emailResults.Length > 1)) {
                        string queryUpdateEM2 = $"UPDATE Emails SET email='{tbxEmail2.Text}' WHERE Id={emailResults[1][0]}";
                        using (SqlCommand cmd = new SqlCommand(queryUpdateEM2, thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    } else if (tbxEmail2.Text != "" && emailResults.Length == 1) {
                        CheckIfEmailInDatabase(tbxEmail2.Text);
                        object[][] resultsEmails2 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail2.Text}'");
                        using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{Id}','{resultsEmails2[0][0]}')", thisConnection)) {
                            cmd.ExecuteNonQuery();
                        }//end command
                    }
                        //same for 3rd email
                        if ((tbxEmail3.Text != "" && emailResults.Length == 3) || (tbxEmail3.Text == "" && emailResults.Length == 3)) {
                            string queryUpdateEM3 = $"UPDATE Emails SET email='{tbxEmail3.Text}' WHERE Id={emailResults[2][0]}";
                            using (SqlCommand cmd = new SqlCommand(queryUpdateEM3, thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command
                        } else if (tbxEmail3.Text != "" && emailResults.Length == 2) {
                            CheckIfEmailInDatabase(tbxEmail3.Text);
                            object[][] resultsEmails3 = dbContacts.ExecuteReader($"SELECT Id FROM Emails WHERE email='{tbxEmail3.Text}'");
                            using (SqlCommand cmd = new SqlCommand($"INSERT INTO contactEmails (userId,emailId) VALUES ('{Id}','{resultsEmails3[0][0]}')", thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command
                        }
                    thisConnection.Close();
                    GetContacts();
                    //update listboxes
                    ListFavorites();
                    ListContacts();
                }//end connection
            }//end if
        }//end event
        private void btnSearch_Click(object sender, RoutedEventArgs e) {
            lsbSearch.Items.Clear();
            lsbSearch.Visibility = Visibility.Visible;
            _searchContacts = new List<Contact>();
            //grab search results
            object[][] results = dbContacts.ExecuteReader($"SELECT * FROM Contacts WHERE firstName LIKE '%{tbxSearch.Text}%' OR lastName LIKE '%{tbxSearch.Text}%'");

            foreach (object row in results) {
                object[] tempRow = (object[])row;               
                _searchContacts.Add(new Contact((int)tempRow[0], (string)tempRow[1], (string)tempRow[2], (string)tempRow[3], (string)tempRow[4], (string)tempRow[5], (string)tempRow[6], (string)tempRow[7], (string)tempRow[8], (string)tempRow[9], (string)tempRow[10], (string)tempRow[11], (string)tempRow[12], (bool)tempRow[13], (bool)tempRow[14], (bool)tempRow[15]));
            }
            //searches for contact based on first or last name only
            string contact = "";
            
            //add search results to listbox
            for (int index = 0; index < results.Length; index++) {
                contact = $"{_searchContacts.ElementAt(index).firstName} {_searchContacts.ElementAt(index).lastName}";

                lsbSearch.Items.Add(contact);
            }//end for
        }//end event
        private void btnFavorite_Click(object sender, RoutedEventArgs e) {
            if (lsbContacts.SelectedItem != null) {
                int Id = _activeContacts.ElementAt(lsbContacts.SelectedIndex).id;
                //updates selected contact to a favorite
                string query = $"UPDATE Contacts SET favorite=1 WHERE Id='{Id}'";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {

                        cmd.ExecuteNonQuery();
                    }//end command

                    thisConnection.Close();
                }//end connection
                //lists contact in favorites box
                GetContacts();
                ListFavorites();
            }//end if         
        }//end event
        private void btnUnfavorite_Click(object sender, RoutedEventArgs e) {
            if (lsbFavorites.SelectedItem != null) {
                int Id = _favoriteContacts.ElementAt(lsbFavorites.SelectedIndex).id;
                //updates selected contact in favorites to be unfavorited
                string query = $"UPDATE Contacts SET favorite=0 WHERE Id='{Id}'";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command

                    thisConnection.Close();
                }//end connection
                //update favorites box
                GetContacts();
                ListFavorites();
            }//end if          
        }//end event
        private void btnEmptyTrash_Click(object sender, RoutedEventArgs e) {

            for (int index = 0; index <= lsbTrash.Items.Count-1; index++) {
                //selects te first contact in the trash
                int Id = _trashContacts.ElementAt(index).id;    
                //check if any phone numbers or emails are associated with the contact
                object[][] resultsPhoneNumberID;
                object[][] resultsEmailID;
                resultsPhoneNumberID = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE userId='{Id}'");
                resultsEmailID = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE userId='{Id}'");

                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    //updates the contact to nonactive
                    using (SqlCommand cmd = new SqlCommand($"UPDATE Contacts SET active=0 WHERE Id='{Id}'", thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    //deletes the nonactive contact from the database
                    using (SqlCommand cmd = new SqlCommand($"DELETE FROM Contacts WHERE active=0", thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command

                    //based on the number of phone numbers the contact has each is deleted from the database as well
                    if (resultsPhoneNumberID.Length != 0) {
                        object[][] resultsPhoneNumbers = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE phoneNumberId='{resultsPhoneNumberID[0][2]}'");
                        //if at least 1 number is present
                        if (resultsPhoneNumbers.Length > 1) {
                            //if more than one user has the specific phone number delete the reference from the child table instead of deleting the number from the database completely
                            using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactPhoneNumbers WHERE phoneNumberId={resultsPhoneNumberID[0][2].ToString()} AND userId={Id}", thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command 
                        } else {
                            using (SqlCommand cmd = new SqlCommand($"DELETE FROM PhoneNumbers WHERE Id={resultsPhoneNumberID[0][2].ToString()}", thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command 
                        }//end if                  
                        //if at least 2 numbers are present
                        if (resultsPhoneNumberID.Length == 2 || resultsPhoneNumberID.Length == 3) {
                            object[][] resultsPhoneNumbers2 = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE phoneNumberId='{resultsPhoneNumberID[1][2]}'");
                            if (resultsPhoneNumbers2.Length > 1) {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactPhoneNumbers WHERE phoneNumberId={resultsPhoneNumberID[1][2].ToString()} AND userId={Id}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command 
                            } else {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM PhoneNumbers WHERE Id={resultsPhoneNumberID[1][2].ToString()}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            }//end if
                        }//end if                                               
                        //if 3 numbers are present
                        if (resultsPhoneNumberID.Length == 3) {
                            object[][] resultsPhoneNumbers3 = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE phoneNumberId='{resultsPhoneNumberID[2][2]}'");
                            if (resultsPhoneNumbers3.Length > 1) {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactPhoneNumbers WHERE phoneNumberId={resultsPhoneNumberID[2][2].ToString()} AND userId={Id}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command 
                            } else {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM PhoneNumbers WHERE Id={resultsPhoneNumberID[2][2].ToString()}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            }//end if
                        }//end if                                                
                    }//end if
                    //based on the number of emails the contact has each is deleted from the database as well
                    if (resultsEmailID.Length != 0) {
                        object[][] resultsEmails = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE emailId='{resultsEmailID[0][2]}'");
                        //if at least 1 emails are present
                        if (resultsEmails.Length > 1) {
                            //if more than one user has the specific email delete the reference from the child table instead of deleting the number from the database completely
                            using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactEmails WHERE emailId={resultsEmailID[0][2].ToString()} AND userId={Id}", thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command
                        } else {
                            using (SqlCommand cmd = new SqlCommand($"DELETE FROM Emails WHERE Id={resultsEmailID[0][2].ToString()}", thisConnection)) {
                                cmd.ExecuteNonQuery();
                            }//end command
                        }//end if
                        //if at least 2 emails are present                     
                        if (resultsEmailID.Length == 2 || resultsEmailID.Length == 3) {
                            object[][] resultsEmails2 = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE emailId='{resultsEmailID[1][2]}'");
                            if (resultsEmails2.Length > 1) {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactEmails WHERE emailId={resultsEmailID[1][2].ToString()} AND userId={Id}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            } else {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM Emails WHERE Id={resultsEmailID[1][2].ToString()}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            }//end if
                        }//end if
                        //if 3 emails are present                                            
                        if (resultsEmailID.Length == 3) {
                            object[][] resultsEmails3 = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE emailId='{resultsEmailID[2][2]}'");
                            if (resultsEmails3.Length > 1) {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM contactEmails WHERE emailId={resultsEmailID[2][2].ToString()} AND userId={Id}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            } else {
                                using (SqlCommand cmd = new SqlCommand($"DELETE FROM Emails WHERE Id={resultsEmailID[2][2].ToString()}", thisConnection)) {
                                    cmd.ExecuteNonQuery();
                                }//end command
                            }//end if
                        }//end if
                                                
                    }//end if

                    thisConnection.Close();
                }//end connection               
                
            }//end for
            //update trash list
            lsbTrash.Items.Clear();
        }//end event
        private void btnReinstateContact_Click(object sender, RoutedEventArgs e) {
            int Id = _trashContacts.ElementAt(lsbTrash.SelectedIndex).id;
            //reinstated a conact that was set to be deleted to active status
            string query = $"UPDATE Contacts SET [delete]=0 WHERE Id='{Id}'";
            using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                thisConnection.Open();
                using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {

                    cmd.ExecuteNonQuery();
                }//end command

                thisConnection.Close();
            }//end connection
            //updates all list boxes
            GetContacts();
            ListTrash();
            ListContacts();
            ListFavorites();
        }//end event
        private void btnClearForm_Click(object sender, RoutedEventArgs e) {
            ClearForm();
        }//end event
        private void btnLoadImage_Click(object sender, RoutedEventArgs e) {
            //allows user to search computer for a picture to add as contact image
            string imageFilePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                imageFilePath = openFileDialog.FileName;
            }
            tbxPhoto.Text = imageFilePath;
        }//end event
        private void lsbContacts_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //when a contact is selected all info is displayed in a central box
            if (lsbContacts.SelectedItem != null) {
                
                int Id = _activeContacts.ElementAt(lsbContacts.SelectedIndex).id;
                lsbDisplayContact.Items.Clear();
                lsbFavorites.UnselectAll();
                DisplayAllContactInfo(Id);
            }//end if           
        }//end event
        private void lsbSearch_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //when a contact is selected all info is displayed in a central box
            if (lsbSearch.SelectedItem != null) {

                int Id = _searchContacts.ElementAt(lsbSearch.SelectedIndex).id;
                lsbDisplayContact.Items.Clear();
                lsbFavorites.UnselectAll();
                lsbContacts.UnselectAll();
                DisplayAllContactInfo(Id);
            }//end if    
        }//end event
        private void lsbFavorites_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //when a contact is selected all info is displayed in a central box
            if (lsbFavorites.SelectedItem != null) {
                int Id = _favoriteContacts.ElementAt(lsbFavorites.SelectedIndex).id;
                lsbDisplayContact.Items.Clear();
                lsbContacts.UnselectAll();
                DisplayAllContactInfo(Id);
            }//end if            
        }//end event
        private void tabAllContacts_GotFocus(object sender, RoutedEventArgs e) {
            //changes button visibility when a new tab is selected
            btnAddContact.Visibility = Visibility.Visible;
            btnConfirmEdit.Visibility = Visibility.Hidden;
            ClearForm();
        }//end event
        private void tabTrash_GotFocus(object sender, RoutedEventArgs e) {
            //changes button visibility when a new tab is selected
            btnAddContact.Visibility = Visibility.Visible;
            btnConfirmEdit.Visibility = Visibility.Hidden;
            ClearForm();
        }//end event
        private void GetContacts() {
            object[][] results = dbContacts.ExecuteReader("SELECT * FROM Contacts");
            _activeContacts = new List<Contact>();
            _favoriteContacts = new List<Contact>();
            _trashContacts = new List<Contact>();
            //add contacts to my different lists 
            foreach (object row in results) {
                object[] tempRow = (object[])row;
                if ((bool)tempRow[15] == true) {
                    _trashContacts.Add(new Contact((int)tempRow[0], (string)tempRow[1], (string)tempRow[2], (string)tempRow[3], (string)tempRow[4], (string)tempRow[5], (string)tempRow[6], (string)tempRow[7], (string)tempRow[8], (string)tempRow[9], (string)tempRow[10], (string)tempRow[11], (string)tempRow[12], (bool)tempRow[13], (bool)tempRow[14], (bool)tempRow[15]));
                }
                if ((bool)tempRow[13] == true && (bool)tempRow[15] == false) {
                    _favoriteContacts.Add(new Contact((int)tempRow[0], (string)tempRow[1], (string)tempRow[2], (string)tempRow[3], (string)tempRow[4], (string)tempRow[5], (string)tempRow[6], (string)tempRow[7], (string)tempRow[8], (string)tempRow[9], (string)tempRow[10], (string)tempRow[11], (string)tempRow[12], (bool)tempRow[13], (bool)tempRow[14], (bool)tempRow[15]));
                }
                if ((bool)tempRow[15]==false) {
                    _activeContacts.Add(new Contact((int)tempRow[0], (string)tempRow[1], (string)tempRow[2], (string)tempRow[3], (string)tempRow[4], (string)tempRow[5], (string)tempRow[6], (string)tempRow[7], (string)tempRow[8], (string)tempRow[9], (string)tempRow[10], (string)tempRow[11], (string)tempRow[12], (bool)tempRow[13], (bool)tempRow[14], (bool)tempRow[15]));
                }                
            }//end foreach
        }//end function
        private void CheckIfPhoneNumberInDatabase(string phoneNumber) {
            //check if there is already the same number in the database before trying to add it again
            object[][] results = dbContacts.ExecuteReader($"SELECT * FROM PhoneNumbers WHERE phoneNumber='{phoneNumber}'");
            if (results.Length == 0) {
                string queryPhoneNumber = $"INSERT INTO PhoneNumbers (phoneNumber) VALUES('{phoneNumber}')";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(queryPhoneNumber, thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    thisConnection.Close();
                }//end connection
            }//end if
        }//end function
        private void CheckIfEmailInDatabase(string email) {
            //check if there is already the same email in the database before trying to add it again
            object[][] results = dbContacts.ExecuteReader($"SELECT * FROM Emails WHERE email='{email}'");
            if (results.Length == 0) {
                string queryEmail = $"INSERT INTO Emails (email) VALUES('{email}')";
                using (SqlConnection thisConnection = new SqlConnection(sqlConnectionString)) {
                    thisConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(queryEmail, thisConnection)) {
                        cmd.ExecuteNonQuery();
                    }//end command
                    thisConnection.Close();
                }//end connection
            }//end if
        }//end function
        private void ListContacts() {
            string contact = "";
            lsbContacts.Items.Clear();

            for (int index = 0; index < _activeContacts.Count; index++) {
               
                if (_activeContacts.ElementAt(index).nickName == "") {
                    contact = $"{_activeContacts.ElementAt(index).firstName} {_activeContacts.ElementAt(index).lastName}";
                } else {
                    contact = $"{_activeContacts.ElementAt(index).nickName}";
                }//end if
                
                lsbContacts.Items.Add(contact);                                                
            }//end for                        
        }//end function
        private void ListFavorites() {
            string contact = "";
            lsbFavorites.Items.Clear();

            for (int index = 0; index < _favoriteContacts.Count; index++) {
               
                if (_favoriteContacts.ElementAt(index).nickName == "") {
                    contact = $"{_favoriteContacts.ElementAt(index).firstName} {_favoriteContacts.ElementAt(index).lastName}";
                } else {
                    contact = $"{_favoriteContacts.ElementAt(index).nickName}";
                }//end if

                lsbFavorites.Items.Add(contact);                             
            }//end for           
        }//end function
        private void ListTrash() {
            string contact = "";
            lsbTrash.Items.Clear();

            for (int index = 0; index < _trashContacts.Count; index++) {
                                  
                contact = $"{_trashContacts.ElementAt(index).firstName} {_trashContacts.ElementAt(index).lastName}";
               
                lsbTrash.Items.Add(contact);               
            }//end for
        }//end function
        private void DisplayAllContactInfo(int Id) {
            object[][] phoneNumb1;
            object[][] phoneNumb2;
            object[][] phoneNumb3;

            object[][] email1;
            object[][] email2;
            object[][] email3;

            //get all contact info from contact table at specific id
            object[][] results = dbContacts.ExecuteReader($"SELECT * FROM Contacts WHERE Id={Id}");

            //get phonenumber ids from child table using contact id
            object[][] resultPhoneNumbers = dbContacts.ExecuteReader($"SELECT * FROM contactPhoneNumbers WHERE userId={Id}");            
                                   
            //get email ids from child table using contact id
            object[][] resultEmails = dbContacts.ExecuteReader($"SELECT * FROM contactEmails WHERE userId={Id}");
            
            //displays contact info to central listbox           
            lsbDisplayContact.Items.Add($"First Name: {results[0][1].ToString()}");
            lsbDisplayContact.Items.Add($"Last Name: { results[0][2].ToString()}");
            lsbDisplayContact.Items.Add($"Nick Name: {results[0][3].ToString()}");

            //if there are phonenumbers to show they are shown
            if (resultPhoneNumbers.Length != 0) {             
                phoneNumb1 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[0][2].ToString()}");
                lsbDisplayContact.Items.Add($"Phone Number1: {phoneNumb1[0][1]}");

                if (resultPhoneNumbers.Length == 2 || resultPhoneNumbers.Length == 3) {                  
                    phoneNumb2 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[1][2].ToString()}");
                    lsbDisplayContact.Items.Add($"Phone Number2: {phoneNumb2[0][1]}");
                }//end if
                if (resultPhoneNumbers.Length == 3) {                  
                    phoneNumb3 = dbContacts.ExecuteReader($"SELECT * from PhoneNumbers WHERE Id={resultPhoneNumbers[2][2].ToString()}");
                    lsbDisplayContact.Items.Add($"Phone Number3: {phoneNumb3[0][1]}");
                }//end if
            }//end if

            //if there are emails to show they are shown
            if (resultEmails.Length != 0) {            
                email1 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[0][2].ToString()}");
                lsbDisplayContact.Items.Add($"Email1: {email1[0][1]}");

                if (resultEmails.Length == 2 || resultEmails.Length == 3) {                    
                    email2 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[1][2].ToString()}");
                    lsbDisplayContact.Items.Add($"Email2: {email2[0][1]}");
                }//end if
                if (resultEmails.Length == 3) {                    
                    email3 = dbContacts.ExecuteReader($"SELECT * from Emails WHERE Id={resultEmails[2][2].ToString()}");
                    lsbDisplayContact.Items.Add($"Email3: {email3[0][1]}");
                }//end if
            }//end if
            lsbDisplayContact.Items.Add($"Website: {results[0][5]}");
            lsbDisplayContact.Items.Add($"Company: {results[0][6]}");
            lsbDisplayContact.Items.Add($"Street: {results[0][7]}");
            lsbDisplayContact.Items.Add($"City: {results[0][8]}");
            lsbDisplayContact.Items.Add($"State: {results[0][9]}");
            lsbDisplayContact.Items.Add($"Zip Code: {results[0][10]}");
            lsbDisplayContact.Items.Add($"Country: {results[0][11]}");
            lsbDisplayContact.Items.Add($"Notes: {results[0][12]}");

            //displays contact photo
            if (results[0][4].ToString() != "") {
                BitmapImage contactImage = new BitmapImage(new Uri(results[0][4].ToString()));
                imgContactPic.Source = contactImage;
            } else {
                imgContactPic.Source = null;
            }//end if
            
        }//end function       
        private void ClearForm() {
            //clears all text boxes in the add contact tab
            tbxFirstName.Text="";
            tbxLastName.Text ="";
            tbxNickName.Text ="";
            tbxPhoto.Text    ="";
            tbxWebsite.Text  ="";
            tbxCompany.Text  ="";
            tbxStreet.Text   ="";
            tbxCity.Text     ="";
            tbxState.Text    ="";
            tbxZipCode.Text  ="";
            tbxCountry.Text  ="";
            tbxNotes.Text = "";

            tbxPhoneNumber1.Text = "";
            tbxPhoneNumber2.Text = "";
            tbxPhoneNumber3.Text = "";
            tbxEmail1.Text = "";
            tbxEmail2.Text = "";
            tbxEmail3.Text = "";
        }//end function      
    }//end class
}//end namespace
