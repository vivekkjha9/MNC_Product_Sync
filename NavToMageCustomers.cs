using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MNC_Product_Sync.MagentoConnectService;
using ConnectCsharpToMysql;
using System.Collections;

namespace MNC_Product_Sync
{
    class Customer
    {

       

         
        static void MainCus(string[] args)
        {

           
            Navision_CustomerListService.CustomerList nv_Customer = new Navision_CustomerListService.CustomerList();
            Navision_CustomerListService.CustomerList_PortClient client = new Navision_CustomerListService.CustomerList_PortClient();
            Navision_CustomerListService.CustomerList[] nv_CustomerLists;
            client.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";

            client.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

            client.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";

            nv_CustomerLists = client.ReadMultiple(null, null, 10000);

            MagentoConnectService.PortTypeClient mage_client = new MagentoConnectService.PortTypeClient();
            DBConnect db = new DBConnect();
            int newCustomerCreateID = 0;
            int createAddressID = 0;
            string token_id = "";
            filters myfilter = new filters();
            token_id = mage_client.login("webserviceuser", "apikey");

            Hashtable hsc = db.fetch_CustomerMapping();
            for (int i = 0; i < nv_CustomerLists.Length; i++)
            {
                // Create Customer
                customerCustomerEntityToCreate customerCreate = new customerCustomerEntityToCreate();
                try
                {
                   
                    customerCreate.firstname = nv_CustomerLists[i].Name;
                    customerCreate.lastname = nv_CustomerLists[i].Name;
                    customerCreate.middlename = "";
                    customerCreate.store_id = 1;
                    customerCreate.store_idSpecified = true;
                    customerCreate.website_id = 1;
                    customerCreate.website_idSpecified = true;
                    customerCreate.group_id = 1;
                    customerCreate.group_idSpecified = true;
                    customerCreate.suffix = "P";
                    customerCreate.email = nv_CustomerLists[i].E_Mail;
                    customerCreate.gender = 1; //1-Male;2-Female
                    customerCreate.genderSpecified = true;
                    //customer address
                    customerAddressEntityCreate customerAddress = new customerAddressEntityCreate();
                   if (nv_CustomerLists[i].Location_Code== null) {
                        customerAddress.city = "NA";
                    }
                    else { 
                    customerAddress.city = nv_CustomerLists[i].Location_Code;
                    }
                    customerAddress.region_id = 2;   //CountryID
                    customerAddress.region_idSpecified = true;
                    if (nv_CustomerLists[i].Country_Region_Code == null)
                    {
                        customerAddress.country_id = "NA";
                    }
                    else { 
                    customerAddress.country_id = nv_CustomerLists[i].Country_Region_Code;
                    }
                    customerAddress.fax = nv_CustomerLists[i].Fax_No;
                    customerAddress.firstname = nv_CustomerLists[i].Name;
                    customerAddress.lastname = nv_CustomerLists[i].Name;
                    customerAddress.middlename = "";
                    string[] streettxt = { nv_CustomerLists[i].Address };
                    customerAddress.street = streettxt;
                    
                    customerAddress.postcode = nv_CustomerLists[i].Post_Code;
                    customerAddress.prefix = "M";
                    customerAddress.suffix = "P";
                    //  customerAddress.street = nv_CustomerLists[i].Address.ToString();
                    if (nv_CustomerLists[i].Phone_No==null)
                    {
                        customerAddress.telephone = "NA";
                    }
                    else
                    { 
                    customerAddress.telephone = nv_CustomerLists[i].Phone_No;
                    }
                    //If you specify 'true' for is_default_billing and is_default_shipping. Then this address will be your default shipping and billing. Otherwise none. 
                    customerAddress.is_default_billing = true;
                    customerAddress.is_default_shipping = true;

                    if (db.fetch_Customer(nv_CustomerLists[i].No.ToString()))
                    {
                        mage_client.customerCustomerUpdate(token_id, (int)hsc[nv_CustomerLists[i].No], customerCreate);
                    }
                    else
                    {
                        newCustomerCreateID = mage_client.customerCustomerCreate(token_id, customerCreate);
                        createAddressID = mage_client.customerAddressCreate(token_id, newCustomerCreateID, customerAddress);

                    }
                    db.InsertCustomerMapping(nv_CustomerLists[i].No, newCustomerCreateID.ToString(), nv_CustomerLists[i].E_Mail);
                    db.InsertLog("Customer", nv_CustomerLists[i].No, "Navision Customer Code", "SUCCESS");
                }
                catch (Exception ex)
                {
                    MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();

                    errLog.LogError("C:\\MNC_Logs", "Customer :" + ex.Message);
                    db.InsertLog("Customer", nv_CustomerLists[i].No, ex.Message, "FAILED");


                }


                finally
                {
                                                

                    //mage_client.endSession(token_id);
                }

             
            }
            mage_client.endSession(token_id);
        }
         
        }

        
}
