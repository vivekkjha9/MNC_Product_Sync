using AX_CRT_MAge_Connector;
using ConnectCsharpToMysql;
using Microsoft.Dynamics.Commerce.Runtime;
//using Microsoft.Dynamics.Commerce.Runtime.Client;
using Microsoft.Dynamics.Commerce.Runtime.Configuration;
using Microsoft.Dynamics.Commerce.Runtime.Data;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
//using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AX_CRT_Mage_Connector
{
   static class MageToAXCustomers_Operations_Handler
    {
        private static CommerceRuntime runtime = null;
        static void MainXYZ(string[] args)
        {
            //runtime = CommerceRuntimeManager.Runtime;
            DynamicsRuntimeManager crtManager = new DynamicsRuntimeManager();
            runtime = crtManager.CommerceRuntime;
            CreateCustomer();
        }

        public static void CreateCustomer()
        {


            

            DBConnect db = new DBConnect();
            AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient mage_client = new AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient();

            string token_id = null;


            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                token_id = mage_client.login("webserviceuser", "apikey");
             

                
            }
            catch(Exception ex)
            { }

            AX_CRT_MAge_Connector.MagentoConnectService .customerCustomerEntity[] mageCustomers = mage_client.customerCustomerList(token_id, null);
            Hashtable hsc = db.fetch_Customer1Mapping();
            for (int i = 0; i < mageCustomers.Length; i++)
            {

                bool flag = false;
                {

                   // string[] a = new string[hsc.Values.Count];
                  if(  hsc.ContainsValue(mageCustomers[i].customer_id.ToString()))
                    {
                        flag = true;
                    }
                   
                   
                }
                
                if (flag==false)
                
                  {


                        long RecId = GetRecordID("CustTable");
                        long PartyRecId = GetRecordID("DirPartyTable");



                        AX_CRT_MAge_Connector.MagentoConnectService.customerAddressEntityItem[] c = mage_client.customerAddressList(token_id, mageCustomers[i].customer_id);
                        //DECLARE @Table as INT = (SELECT TABLEID FROM SQLDICTIONARY WHERE NAME = 'ÇUSTTABLE' AND FIELDID = 0)
                        //DECLARE @RecID AS BIGINT = (SELECT NEXTVAL FROM SYSTEMSEQUENCES WHERE TABID = @Table AND NAME = 'SEQNO')
                        System.Collections.Generic.List<Microsoft.Dynamics.Commerce.Runtime.DataModel.Address> addr1 = new List<Address>();
                        long EmailRecId = GetRecordID("LOGISTICSELECTRONICADDRESS");
                       for (int j = 0; j < c.Length; j++)
                        {
                        
                       // long LogisticsRecId = GetRecordID("LOGISTICSLOCATION");
                        Address addrline = new Address()
                        {
                            AddressType = AddressType.Business,
                            Name = mageCustomers[i].firstname + mageCustomers[i].lastname,
                            City = c[j].city,

                            Street = c[j].street,
                            Email = mageCustomers[i].email,
                            Phone = c[j].telephone,
                            ThreeLetterISORegionName = "IND",
                            Postbox = c[j].postcode,
                            // FullAddress = c[j].street + "," + c[j].region + "," + c[j].postcode,                                                                                   
                            EmailRecordId = EmailRecId,
                            IsAsyncAddress = true,
                          //  LogisticsLocationRecordId = LogisticsRecId,
                            ZipCode = c[j].postcode,
                            IsPrimary = true                                                               
                            

                        };

                            addr1.Add(addrline);
                        }


                    //AddressBookPartyData ss = new AddressBookPartyData();
                    //ss.StoreAddressBook = "";

                    IList<AddressBookPartyData> list = new List<AddressBookPartyData>();
                    AddressBookPartyData addr = new AddressBookPartyData();
                    addr.RecordId= 22565421421;
                    list.Add(addr);

                        Customer customer = new Microsoft.Dynamics.Commerce.Runtime.DataModel.Customer()
                        {

                            AccountNumber =   mageCustomers[i].customer_id.ToString(),
                            Email = mageCustomers[i].email,
                            FirstName = mageCustomers[i].firstname,
                            LastName = mageCustomers[i].lastname,
                            IsAsyncCustomer = true,
                            Addresses = addr1,                            
                            
                            DirectoryPartyRecordId = PartyRecId,
                            PartyNumber = mageCustomers[i].customer_id.ToString(),
                            CustomerGroup = "30",
                            Name = mageCustomers[i].firstname + mageCustomers[i].lastname,                             
                            CurrencyCode = "EUR",               
                            AddressBooks = list,                                           
                            RecordId = RecId

                        };

                   

                    var updateCustomerRequest = new CreateOrUpdateCustomerDataRequest(customer);
                    runtime.Execute<SingleEntityDataServiceResponse<Customer>>(updateCustomerRequest, new RequestContext(runtime));
                    for (int m = 0; m < addr1.Count;m++)
                    {
                        var updateCustomerAddressRequest = new CreateOrUpdateAsyncCustomerAddressDataRequest(customer, addr1[m]);
                        runtime.Execute<SingleEntityDataServiceResponse<Address>>(updateCustomerAddressRequest, new RequestContext(runtime));
                    }
                    
                        db.InsertCustomerMapping(customer.AccountNumber, mageCustomers[i].customer_id.ToString(), customer.Email);
           
                    }
            }
           

        }

        private static long GetRecordID(string tabName)
        {
            string sqlStr = "SELECT * FROM SQLDICTIONARY WHERE NAME = '" + tabName + "' AND FIELDID = 0";
            System.Data.DataTable resultTableID = new System.Data.DataTable();
            System.Data.DataTable resultRecordID = new System.Data.DataTable();

            ////Microsoft.Dynamics.Commerce.Runtime.Data.Types.DataTable results = new Microsoft.Dynamics.Commerce.Runtime.Data.Types.DataTable();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=REP-DEV-1,1433;Database=AxDB;User Id=crtconnector;Password=abc123"))
            using (SqlCommand command = new SqlCommand(sqlStr, conn))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                dataAdapter.Fill(resultTableID);

            int TabId = 0;
            foreach (DataRow row in resultTableID.Rows)
            {
                // ... Write value of first field as integer.
                TabId = row.Field<int>(0);

            }

            string sqlStrTableID = "SELECT * FROM SYSTEMSEQUENCES WHERE TABID = " + TabId + " AND NAME = 'SEQNO'";

            using (System.Data.SqlClient.SqlConnection conn1 = new System.Data.SqlClient.SqlConnection("Server=REP-DEV-1,1433;Database=AxDB;User Id=crtconnector;Password=abc123"))
            using (SqlCommand command = new SqlCommand(sqlStrTableID, conn1))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                dataAdapter.Fill(resultRecordID);

            long RecId = 0;
            foreach (DataRow row in resultRecordID.Rows)
            {
                // ... Write value of first field as integer.
                RecId = row.Field<long>(1);

            }

            string sqlUpdateSeqTable = "UPDATE  SYSTEMSEQUENCES SET NEXTVAL = NEXTVAL + 1 WHERE TABID = " + TabId + " AND NAME = 'SEQNO'";
            using (System.Data.SqlClient.SqlConnection conn2 = new System.Data.SqlClient.SqlConnection("Server=REP-DEV-1,1433;Database=AxDB;User Id=crtconnector;Password=abc123"))
            
                try
                {
                    conn2.Open();
                    SqlCommand cmd = new SqlCommand(sqlUpdateSeqTable, conn2);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn2.Close();
                   
                }
                catch(Exception ex)
                { }

            return RecId;
        }
    }
}
