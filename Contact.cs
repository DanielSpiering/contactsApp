using System;

public class Contact
{
	public int id {get; set;}
	public string firstName {get; set;}
	public string lastName {get; set;}
	public string nickName {get; set;}
	public string photo {get; set;}
	public string website {get; set;}
	public string company {get; set;}
	public string street {get; set;}
	public string city {get; set;}
	public string state {get; set;}
	public string zipCode {get; set;}
	public string country {get; set;}
	public string notes {get; set;}
	public bool favorite {get; set;}
	public bool active {get; set;}
	public bool delete {get; set;}
	public Contact(int newId, string newFirstName="", string newLastName="", string newNickName="", string newPhoto="", string newWebsite="", string newCompany="", string newStreet="", string newCity="", string newState="", string newZipCode="", string newCountry="", string newNotes="", bool newFavorite=false, bool newActive=true, bool newDelete=false) {
		id = newId;
		firstName= newFirstName;
		lastName = newLastName;
		nickName = newNickName;
		photo 	 = newPhoto;
		website	 = newWebsite;
		company	 = newCompany;
		street 	 = newStreet;
		city 	 = newCity;
		state 	 = newState;
		zipCode	 = newZipCode;
		country	 = newCountry;
		notes 	 = newNotes;
		favorite = newFavorite;
		active = newActive;
		delete = newDelete;		
	}//end constructor
}//end class
