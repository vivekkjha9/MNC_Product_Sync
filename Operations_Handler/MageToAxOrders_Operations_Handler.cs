using AX_CRT_MAge_Connector;
using ConnectCsharpToMysql;
using DynamicsConnectivityValidator;
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
    static class MageToAXOrders
    {
        private static CommerceRuntime runtime = null;
        static void Main(string[] args)
        {
            //runtime = CommerceRuntimeManager.Runtime;
            DynamicsRuntimeManager crtManager = new DynamicsRuntimeManager();
            runtime = crtManager.CommerceRuntime;
            CombinedWriter writer;
            var oldOut = Console.Out;
            DBConnect db = new DBConnect();
            Hashtable hs = db.fetch_CategoryMapping();

            Hashtable hsp = db.fetch_ProductMapping();
         
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
            SimpleProduct prd = null;
            foreach (DictionaryEntry pair in hs)
            {

                Microsoft.Dynamics.Commerce.Runtime.PagedResult<Microsoft.Dynamics.Commerce.Runtime.DataModel.SimpleProduct> axProducts = AX_CRT_MAge_Connector.UtilityFunctions.readProducts(crtManager, writer, long.Parse(pair.Key.ToString()));

                 prd = axProducts.Results[0];
                break;

            }

                try
                {
                writer = new CombinedWriter("./DynamicsConnectivityValidator.log", false, Encoding.Unicode, 0x400, Console.Out);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open DynamicsConnectivityValidator.log for writing");
                Console.WriteLine(e.Message);
               // return;
            }

            Console.SetOut(writer);
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (se, cert, chain, sslerror) =>
            {
                return true;
            };


            UtilityFunctions.createOrder1(crtManager, writer, prd);
        }

        public static void CreateOrder()
        {




            DBConnect db = new DBConnect();
            AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient mage_client = new AX_CRT_MAge_Connector.MagentoConnectService.PortTypeClient();

            string token_id = null;


            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                token_id = mage_client.login("webserviceuser", "apikey");

                

            }
            catch (Exception ex)
            {

            }

            //AX_CRT_MAge_Connector.MagentoConnectService.salesOrderListEntity[] mageOrders = mage_client.salesOrderList(token_id, null);
            //Hashtable hsc = db.fetch_Customer1Mapping();
            


        }

        
    }
}
