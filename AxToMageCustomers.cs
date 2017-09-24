using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AX_CRT_MAge_Connector.MagentoConnectService;
using ConnectCsharpToMysql;
using System.Collections;
using AX_CRT_MAge_Connector;
using DynamicsConnectivityValidator;

namespace AX_CRT_Mage_Connector
{
    class AxToMageCustomers
    {




        static void Main(string[] args)
        {


            CombinedWriter writer;
            var oldOut = Console.Out;

            try
            {
                writer = new CombinedWriter("./DynamicsConnectivityValidator.log", false, Encoding.Unicode, 0x400, Console.Out);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open DynamicsConnectivityValidator.log for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(writer);
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (se, cert, chain, sslerror) =>
            {
                return true;
            };

            var success = true;
            writer.Write("Dynamics Connectivity validation starting...");
            var crtManager = new DynamicsRuntimeManager();
            //var  nv_CustomerLists = client.ReadMultiple(null, null, 10000);

            Microsoft.Dynamics.Commerce.Runtime.PagedResult<Microsoft.Dynamics.Commerce.Runtime.DataModel.Customer> axcustomers = ReadAxCustomers.readCustomer(crtManager,writer);
            AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient mage_client = new AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient();
            DBConnect db = new DBConnect();
            int newCustomerCreateID = 0;
            int createAddressID = 0;
            string token_id = "";
            filters myfilter = new filters();
            token_id = mage_client.login("webserviceuser", "apikey");

            Hashtable hsc = db.fetch_CustomerMapping();
            for (int i = 0; i < axcustomers.Results.Count; i++)
            {
                // Create Customer
                customerCustomerEntityToCreate customerCreate = new customerCustomerEntityToCreate();
                try
                {

                    customerCreate.firstname = axcustomers.Results[i].FirstName;
                    customerCreate.lastname = axcustomers.Results[i].LastName;
                    customerCreate.middlename = "";
                    customerCreate.password = "abc123";
                    customerCreate.store_id = 1;
                    customerCreate.store_idSpecified = true;
                    customerCreate.website_id = 1;
                    customerCreate.website_idSpecified = true;
                    customerCreate.group_id = 1;
                    customerCreate.group_idSpecified = true;
                    customerCreate.suffix = "P";
                    customerCreate.email = axcustomers.Results[i].Email;
                    customerCreate.gender = 1; //1-Male;2-Female
                    customerCreate.genderSpecified = true;
                    //customer address
                    customerAddressEntityCreate customerAddress = new customerAddressEntityCreate();
                    if (axcustomers.Results[i].Addresses[0].City == null)
                    {
                        customerAddress.city = "NA";
                    }
                    else
                    {
                        customerAddress.city = axcustomers.Results[i].Addresses[0].City;
                    }
                    customerAddress.region_id = 2;   //CountryID
                    customerAddress.region_idSpecified = true;
                    if (axcustomers.Results[i].Addresses[0].TwoLetterISORegionName == null)
                    {
                        customerAddress.country_id = "NA";
                    }
                    else
                    {
                        customerAddress.country_id = axcustomers.Results[i].Addresses[0].TwoLetterISORegionName;
                    }
                    // customerAddress.fax = axcustomers.Results[i].fFax_No;
                    customerAddress.firstname = axcustomers.Results[i].FirstName;
                    customerAddress.lastname = axcustomers.Results[i].LastName;
                    customerAddress.middlename = axcustomers.Results[i].MiddleName;
                    string[] streettxt = { axcustomers.Results[i].Addresses[0].StreetNumber };
                    customerAddress.street = streettxt;

                    customerAddress.postcode = axcustomers.Results[i].Addresses[0].ZipCode;
                    customerAddress.prefix = "M";
                    customerAddress.suffix = "P";
                    //  customerAddress.street = nv_CustomerLists[i].Address.ToString();
                    if (axcustomers.Results[i].Addresses[0].Phone == null)
                    {
                        customerAddress.telephone = "NA";
                    }
                    else
                    {
                        customerAddress.telephone = axcustomers.Results[i].Addresses[0].Phone;
                    }
                    //If you specify 'true' for is_default_billing and is_default_shipping. Then this address will be your default shipping and billing. Otherwise none. 
                    customerAddress.is_default_billing = true;
                    customerAddress.is_default_shipping = true;

                    if (db.fetch_Customer(axcustomers.Results[i].AccountNumber.ToString()))
                    {
                        mage_client.customerCustomerUpdate(token_id, (int)hsc[axcustomers.Results[i].AccountNumber], customerCreate);
                    }
                    else
                    {
                        newCustomerCreateID = mage_client.customerCustomerCreate(token_id, customerCreate);
                        createAddressID = mage_client.customerAddressCreate(token_id, newCustomerCreateID, customerAddress);

                    }
                    db.InsertCustomerMapping(axcustomers.Results[i].AccountNumber, newCustomerCreateID.ToString(), axcustomers.Results[i].Email);
                    db.InsertLog("Customer", axcustomers.Results[i].AccountNumber, "Navision Customer Code", "SUCCESS");
                }
                catch (Exception ex)
                {
                    AX_CRT_MAge_Connector.ErrorLog errLog = new AX_CRT_MAge_Connector.ErrorLog();

                    errLog.LogError("C:\\MNC_Logs", "Customer :" + ex.Message);
                   // db.InsertLog("Customer", nv_CustomerLists[i].No, ex.Message, "FAILED");


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
