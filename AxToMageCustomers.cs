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

            DBConnect db = new DBConnect();
            db.InsertLog("Customer","CustomerJob", "AX to Magento Customer Job started successfully at " + DateTime.Now.ToString(), "INFORMATION");
            var crtManager = new DynamicsRuntimeManager();
           

            Microsoft.Dynamics.Commerce.Runtime.PagedResult<Microsoft.Dynamics.Commerce.Runtime.DataModel.Customer> axcustomers = UtilityFunctions.readCustomer(crtManager,writer);
            AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient mage_client = new AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient();
            
            int newCustomerCreateID = 0;
            int createAddressID = 0;
            string token_id = "";
            filters myfilter = new filters();
            token_id = mage_client.login("webserviceuser", "apikey");

            Hashtable hsc = db.fetch_CustomerMapping();
            for (int i = 0; i < axcustomers.Results.Count; i++)
            {

                bool flag = false;
                {

                    // string[] a = new string[hsc.Values.Count];
                    if (hsc.ContainsValue(axcustomers.Results[i].AccountNumber.ToString()))
                    {
                        flag = true;
                    }

                    if (hsc.ContainsKey(axcustomers.Results[i].AccountNumber.ToString()))
                    {
                        flag = true;
                    }


                }

                if (flag == false)
                {
                    // Create Customer
                    customerCustomerEntityToCreate customerCreate = new customerCustomerEntityToCreate();
                    try
                    {
                        string[] name = axcustomers.Results[i].Name.Split();
                        if (name.Length == 2)
                        { 
                            axcustomers.Results[i].FirstName = name[0];
                        axcustomers.Results[i].LastName = name[1];
                    }
                    else
                    {
                            axcustomers.Results[i].FirstName = name[0];
                            axcustomers.Results[i].LastName = ".";
                        }
                        if (axcustomers.Results[i].FirstName == "")
                        {
                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "ERROR: First Name not maintained in AX.This customer will not be integrated", "FAILED");
                            continue;
                        }
                        else
                        {
                            customerCreate.firstname = axcustomers.Results[i].FirstName;
                        }

                        if (axcustomers.Results[i].LastName == "")
                        {
                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "ERROR: Last Name not maintained in AX.This customer will not be integrated", "FAILED");
                            continue;
                        }
                        else
                        {
                            customerCreate.lastname = axcustomers.Results[i].LastName;
                        }


                        // customerCreate.lastname = axcustomers.Results[i].LastName;
                        customerCreate.middlename = "";
                        customerCreate.password = "abc123";
                        customerCreate.store_id = 1;
                        customerCreate.store_idSpecified = true;
                        customerCreate.website_id = 1;
                        customerCreate.website_idSpecified = true;
                        customerCreate.group_id = 1;
                        customerCreate.group_idSpecified = true;
                        customerCreate.suffix = "P";
                        if (axcustomers.Results[i].Email.Equals(""))
                        {

                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "ERROR: Email Id not maintained in AX.This customer will not be integrated", "FAILED");
                            continue;
                        }
                        else
                        {
                            customerCreate.email = axcustomers.Results[i].Email;
                        }
                        customerCreate.gender = 1; //1-Male;2-Female
                        customerCreate.genderSpecified = true;

                        //customer address
                        customerAddressEntityCreate customerAddress = new customerAddressEntityCreate();
                        if (axcustomers.Results[i].Addresses.Count == 0)
                        {
                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "WARNING: City not maintained in AX.For Magento System it is a mandatory field and hence system will default it to NA. You can change it later", "WARNING");
                            customerAddress.city = "NA";
                        }
                        else
                        {
                            customerAddress.city = axcustomers.Results[i].Addresses[0].City;
                        }
                        customerAddress.region_id = 2;   //CountryID
                        customerAddress.region_idSpecified = true;
                        if (axcustomers.Results[i].Addresses.Count == 0)
                        {
                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "WARNING: Country information is not maintained in AX.For Magento System it is a mandatory field and hence system will default it to NA. You can change it later", "WARNING");
                            customerAddress.country_id = "NA";
                        }
                        else
                        {
                            customerAddress.country_id = axcustomers.Results[i].Addresses[0].ThreeLetterISORegionName;
                        }
                        // customerAddress.fax = axcustomers.Results[i].fFax_No;
                        customerAddress.firstname = axcustomers.Results[i].FirstName;
                        customerAddress.lastname = axcustomers.Results[i].LastName;
                        customerAddress.middlename = axcustomers.Results[i].MiddleName;
                        //string[] streettxt;
                        if (axcustomers.Results[i].Addresses.Count == 0)
                        {
                            string[] streettxt = { "NA" };
                            customerAddress.street = streettxt;
                        }
                        else
                        {
                            string[] streettxt1 = { axcustomers.Results[i].Addresses[0].FullAddress };
                            customerAddress.street = streettxt1;
                        }

                        if (axcustomers.Results[i].Phone!=null)
                        { 
                        customerAddress.telephone = axcustomers.Results[i].Phone;
                        }
                        if (axcustomers.Results[i].Addresses.Count == 0)
                        {
                            db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "WARNING: Postal Code not maintained in AX.For Magento System it is a mandatory field and hence system will default it to 99999. You can change it later", "WARNING");
                            customerAddress.postcode = "99999";
                        }
                        else
                        {
                            customerAddress.postcode = axcustomers.Results[i].Addresses[0].ZipCode;
                        }
                        customerAddress.prefix = "M";
                        customerAddress.suffix = "P";
                        //  customerAddress.street = nv_CustomerLists[i].Address.ToString();

                        if (axcustomers.Results[i].Addresses.Count == 0)
                        {
                            // customerAddress.telephone = "NA";
                        }
                        else
                        {
                            if (axcustomers.Results[i].Addresses.Count.Equals("0"))
                            {
                                db.InsertLog("Customer", "AX Account Number: " + axcustomers.Results[i].AccountNumber, "WARNING: Phone not maintained in AX.For Magento System it is a mandatory field and hence system will default it to 999-999-999. You can change it later", "WARNING");
                                customerAddress.telephone = "999-999-999";
                            }
                            else
                            {
                               // customerAddress.telephone = axcustomers.Results[i].Addresses[0].Phone;
                            }
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
                            try { 

                              newCustomerCreateID = mage_client.customerCustomerCreate(token_id, customerCreate);
                               

                            }
                            catch(Exception ex)
                            {
                                db.InsertLog("Customer", axcustomers.Results[i].AccountNumber, ex.Message, "FAILED");
                            }
                            try
                            {

                           
                                createAddressID = mage_client.customerAddressCreate(token_id, newCustomerCreateID, customerAddress);
                            }
                            catch(Exception ex)
                            {
                                db.InsertLog("Customer", axcustomers.Results[i].AccountNumber, ex.Message, "FAILED");
                            }

                        }
                        db.InsertCustomerMapping(axcustomers.Results[i].AccountNumber, newCustomerCreateID.ToString(), axcustomers.Results[i].Email);
                        db.InsertLog("Customer", axcustomers.Results[i].AccountNumber, "AX Customer Account Number", "SUCCESS");

                    }
                    catch (Exception ex)
                    {
                        AX_CRT_MAge_Connector.ErrorLog errLog = new AX_CRT_MAge_Connector.ErrorLog();
                         if (newCustomerCreateID!=0)
                        {
                          // mage_client.customerCustomerDelete(token_id, newCustomerCreateID);
                        }
                        //errLog.LogError("C:\\MNC_Logs", "Customer :" + ex.Message);
                        db.InsertLog("Customer", axcustomers.Results[i].AccountNumber, ex.Message, "FAILED");


                    }


                    finally
                    {


                        //mage_client.endSession(token_id);
                    }
                }

            }
            mage_client.endSession(token_id);
            db.InsertLog("Customer", "CustomerJob", "AX to Magento Customer Job finished successfully at " + DateTime.Now.ToString(), "INFORMATION");
            db.InsertLog("Customer", "CustomerJob", "AX to Magento Customer Job next run time scheuled at " + DateTime.Now.AddHours(2).ToString(), "INFORMATION");
        }

    }


}
